using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using SharedLibraries;
using SharedLibraries.Payloads;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System.Security.Cryptography;
using SharedLibraries.ValidationHelpers;
//using Google.GenAI; used for gemini, replaced with openRouter
//using Google.GenAI.Types; used for gemini, replaced with openRouter
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text;
using Google.GenAI;


namespace Server;

internal sealed class Server : IDisposable
{
    public RSA? KeyPair { get; private set; }// used for the hello payload
    private TcpListener? ServerListener;// used for listening for frames
    private Thread? ListenThread;// thread used for listening for frames
    private bool IsRunning = false;// to know if the server is running for some tasks
    private readonly object DatabaseLock = new object();// to stop multiple threads from accessing the database at once
    private readonly object ClientLock = new object();// to stop multiple threads from altering/using the client list
    //no product lock because products are not changed in the code while running.
    private readonly Dictionary<int, ClientSession> ConnectedClients = new Dictionary<int, ClientSession>();// holds the clients+
    private int NextClientId = 0;// used to have unique client ids
    private List<Message> Messages = new List<Message>();// holds the messages out of the database for easy use
    private List<ProductWithDetails> Products = new List<ProductWithDetails>();// holds the Products out of the database for easy use
    public event Action<Dictionary<int, ClientSession>>? ServerStateChanged;// for refreshing the connected clients 
    //private Client? AIClient;// used for username verification with gemini
    //private List<Content> History = new List<Content>();// used for giving gemini the prompt and username to check
    private readonly HttpClient OpenRouterClient = new HttpClient();//used for sending to and receiving data from the openRouter API
    private string OpenRouterAPIKey = "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";// API key used for the requests to openRouter
    public int Port { get; private set; }// the port used by the server
    public int MessageIdCounter = 0;// used to give messages unique ids
    public string Pepper { private set; get; } = "";

    public void Start(int port)
    {
        if (IsRunning)
        {
            return;//cant run if the server is already running
        }
        Port = port;
        Database.InitializeDB();// starts the database, creates a new file if needed
        lock (DatabaseLock)// doesnt let other threads use the database
        {
            Messages = Database.GetAllMessages();
            Products = Database.GetAllProducts();
        }
        MessageIdCounter = Messages.Count;// to continue from the last messageId
        KeyPair = RSA.Create(2048);// creates a keypair for the hello payload
        ServerListener = new TcpListener(IPAddress.Any, Port);
        ServerListener.Start();// starts listening for connections
        IsRunning = true;// to let other functions know the server is running
        ListenThread = new Thread(ListenForClients) { IsBackground = true };
        ListenThread.Start();// listening for frames
        //InitializeGemini();// starts gemini with the prompt
        InitializeOpenRouter();
        Pepper = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    }
    public void Stop()
    {
        if (!IsRunning)
        {
            return;//cant stop if already not running
        }
        IsRunning = false;
        ServerListener?.Stop();// stops the listening
        lock (ClientLock)
        {
            var Clients = ConnectedClients.Values.ToArray();
            ConnectedClients.Clear();
            foreach (var Client in Clients)
            {
                Client.Dispose();// disconnects from each client
            }
        }
        try
        {
            if (ListenThread != null && ListenThread.IsAlive)
            {
                ListenThread.Join(1000);
            }
        }
        catch (Exception ex)
        {
            PrintOut?.Invoke("failed to stop: " + ex);
        }
        ServerListener = null;
        ListenThread = null;
        if (KeyPair != null)
        {
            KeyPair.Dispose();
            KeyPair = null;
        }
    }
    public void ListenForClients()// listening for client connections
    {
        while (IsRunning)
        {
            try
            {
                TcpClient ConnectedClient = ServerListener!.AcceptTcpClient();// accepts a new client and creates an object for it
                ClientSession clientSession = new ClientSession(ConnectedClient, Interlocked.Increment(ref NextClientId));// gives it an id
                clientSession.FrameReceived += OnFrameReceived;
                clientSession.Disconnected += OnDisconnected;
                clientSession.PrintOut += message => PrintOut?.Invoke(message);
                lock (ClientLock)// to stop other new client acceptences from working at once
                {
                    ConnectedClients[clientSession.ClientId] = clientSession;
                    ServerStateChanged!.Invoke(ConnectedClients);
                }
                clientSession.Start();// starts handling the user with a unique thread
                SendHello(clientSession);// starts the encryption with the user
                PrintOut?.Invoke("Client connected: " + clientSession.RemoteEndpoint);

            }
            catch (SocketException)
            {
                if (!IsRunning)
                {
                    break;// to stop crashing
                }
            }
            catch (ObjectDisposedException)
            {
                break;// to stop crashing
            }
        }
    }
    public void SendHello(ClientSession client)// first step in the secure session handshake
    {
        if (KeyPair == null)
        {
            return;// cant encrypt with rsa if has no rsa keypair
        }
        HelloPayload Payload = new HelloPayload();
        Payload.PublicKey = KeyPair!.ExportRSAPublicKey();// gets the public key
        ProtocolFrame Frame = new ProtocolFrame(ProtocolCommands.Hello, ProtocolEncryptionFlags.UnEncrypted, client.RequestCounter, JsonSerializer.SerializeToUtf8Bytes(Payload));
        client.Send(Frame, false);// sends it unencrypted
    }
    public void OnFrameReceived(ClientSession client, ProtocolFrame frame)// switch to handle each frame payload
    {
        try
        {
            switch (frame.Command)
            {
                case ProtocolCommands.SecureSession:
                    try
                    {
                        HandleSecureSession(client, frame);
                    }
                    catch (Exception ex)
                    {
                        PrintOut?.Invoke("Failed to handle secure session frame: " + ex.Message);
                    }
                    break;
                case ProtocolCommands.Register:
                    try
                    {
                        HandleRegister(client, frame);
                    }
                    catch (Exception ex)
                    {
                        PrintOut?.Invoke("Failed to handle register frame: " + ex.Message);
                    }
                    break;
                case ProtocolCommands.Login:
                    try
                    {
                        HandleLogin(client, frame);
                    }
                    catch (Exception ex)
                    {
                        PrintOut?.Invoke("Failed to handle login frame: " + ex.Message);
                    }
                    break;
                case ProtocolCommands.GetProducts:
                    try
                    {
                        HandleGetProducts(client, frame);
                    }
                    catch (Exception ex)
                    {
                        PrintOut?.Invoke("Failed to handle get products frame: " + ex.Message);
                    }
                    break;
                case ProtocolCommands.PlaceOrder:
                    try
                    {
                        HandlePlaceOrder(client, frame);
                    }
                    catch (Exception ex)
                    {
                        PrintOut?.Invoke("Failed to handle place order frame: " + ex.Message);
                    }
                    break;
                case ProtocolCommands.ChatPost:
                    try
                    {
                        HandleChatPost(client, frame);
                    }
                    catch (Exception ex)
                    {
                        PrintOut?.Invoke("Failed to handle chat post frame: " + ex.Message);
                    }
                    break;
                case ProtocolCommands.Disconnect:
                    try
                    {
                        try
                        {
                            client.Dispose();
                        }
                        catch (Exception ex)
                        {
                            PrintOut?.Invoke("Error disposing client: " + ex.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        PrintOut?.Invoke("Failed to handle disconnect frame: " + ex.Message);
                    }
                    break;
                case ProtocolCommands.Heartbeat:
                    try
                    {
                        HandleHeartbeat(client, frame);
                    }
                    catch (Exception ex)
                    {
                        PrintOut?.Invoke("Failed to handle heartbeat frame: " + ex.Message);
                    }
                    break;
                default:// if the coommand is not one of the ones before its not meant for the server and it wont handle it
                    PrintOut?.Invoke("Received unknown command: " + frame.Command);
                    break;
            }
        }
        catch (Exception ex)
        {
            PrintOut?.Invoke("Failed to handle frame: " + ex.Message);
        }
    }
    public void OnDisconnected(ClientSession client)
    {
        lock (ClientLock)
        {
            ConnectedClients.Remove(client.ClientId);// removes the client after locking so no threads work at once
            ServerStateChanged!.Invoke(ConnectedClients);
        }
    }
    public void HandleSecureSession(ClientSession client, ProtocolFrame frame)// handles the aes key sent by the user
    {
        string DecryptedPayload = SecurityHelpers.DecryptWithPrivateKey(frame.Payload, KeyPair!);// decrypts with the private key
        SecureSessionPayload? Payload = JsonSerializer.Deserialize<SecureSessionPayload>(DecryptedPayload);
        client.aes!.Key = Payload!.AesKey;// gets the key and IV
        client.aes!.IV = Payload.AesIV;
        ConnectionSuccessPayload ResponsePayload = new ConnectionSuccessPayload();
        ResponsePayload.Success = true;// because the server got the aes key it sends a connection success payload
        ProtocolFrame ResponseFrame = new ProtocolFrame(ProtocolCommands.ConnectionSuccess, ProtocolEncryptionFlags.Encrypted, client.RequestCounter++, JsonSerializer.SerializeToUtf8Bytes(ResponsePayload));
        client.Send(ResponseFrame, true);// sends the payload encrypted with the new aes key and iv

    }
    public void HandleRegister(ClientSession client, ProtocolFrame frame)// handles a register payload from the user
    {
        RegisterPayload? Payload = JsonSerializer.Deserialize<RegisterPayload>(frame.Payload);
        if (client.loginCounter >= 10)
        {
            SendAuthenticationResult(client, false, Payload!.Username, true);// return false
            lock (ClientLock)
            {
                ConnectedClients.Remove(client.ClientId);// removes the client after locking so no threads work at once
                ServerStateChanged!.Invoke(ConnectedClients);
            }
            client.Dispose();
            return;
        }
        else
        {
            client.loginCounter += 1;
        }
        if (Payload == null || !ValidationHelpers.ValidateUsername(Payload.Username) || !ValidationHelpers.ValidatePassword(Payload!.Password))
        {
            SendAuthenticationResult(client, false, string.Empty, false);// checks if the payload is empty and if the password and username are valid if not sends a failed result
            return;
        }
        lock (DatabaseLock)
        {
            User? UserExists = Database.SelectUser(Payload!.Username);
            if (UserExists == null)// if the user doesnt exists create a new user and save it to the database
            {
                if (!AskOpenRouter(Payload.Username.ToString()).Result)
                {
                    SendAuthenticationResult(client, false, string.Empty, false);// asks the picked openRouter model if the username contains profanities, if so it returns false and the server send a failed result
                    return;
                }
                User NewUser = new User();
                NewUser.Username = Payload.Username;
                NewUser.Salt = SecurityHelpers.CreateSalt();// generate a salt for more secure password storing
                NewUser.PasswordHash = SecurityHelpers.HashPassword(Payload.Password, NewUser.Salt, Pepper);// hashes the password to not keep it in plain text
                Database.SaveUser(NewUser);// saves the user
                client.Username = Payload.Username;
                client.Authenticated = true;
                SendAuthenticationResult(client, true, Payload.Username, false);
                ServerStateChanged!.Invoke(ConnectedClients);
            }
            else// if else triggers then the username is taken and the user cant register that username
            {
                SendAuthenticationResult(client, false, Payload.Username, false);
            }
        }
    }
    public void HandleLogin(ClientSession client, ProtocolFrame frame)// handles a login request
    {
        //byte[] DecryptedPayload = SecurityHelpers.DecryptWithSessionKey(frame.Payload, client.aes!);
        LoginPayload? Payload = JsonSerializer.Deserialize<LoginPayload>(frame.Payload);
        if (client.loginCounter >= 10)
        {
            SendAuthenticationResult(client, false, Payload!.Username, true);// return false
            lock (ClientLock)
            {
                ConnectedClients.Remove(client.ClientId);// removes the client after locking so no threads work at once
                ServerStateChanged!.Invoke(ConnectedClients);
            }
            client.Dispose();
            return;
        }
        else
        {
            client.loginCounter += 1;
        }
        lock (DatabaseLock)// to stop at once work
        {
            User? LoggingInUser = Database.SelectUser(Payload!.Username);
            foreach (var ClientSession in ConnectedClients)// goes over all of the connected clients to check if the user isnt already logged in
            {
                if (Payload.Username == ClientSession.Value.Username)// if a user with the same username is already connected
                {
                    SendAuthenticationResult(client, false, Payload.Username, true);// return false
                    return;
                }
            }
            if (LoggingInUser != null && SecurityHelpers.VerifyPassword(Payload.Password, LoggingInUser.Salt, Pepper, LoggingInUser.PasswordHash))
            // checks the password and if the username is taken, if yes it returns a positive authentication result
            {
                client.Authenticated = true;
                client.Username = Payload.Username;
                SendAuthenticationResult(client, true, Payload.Username, true);
                ServerStateChanged!.Invoke(ConnectedClients);
            }
            else// if else triggers then the username isnt taken
            {
                SendAuthenticationResult(client, false, Payload.Username, true);
            }
        }

    }
    public void SendAuthenticationResult(ClientSession client, bool Success, string Username, bool IsLogin)
    {
        AuthenticationResultPayload Payload = new AuthenticationResultPayload();
        if (!Success)// if the request failed
        {
            if (IsLogin)// if the message is about a login request
            {
                Payload.Message = "Username or password is incorrect";
            }
            else// the message is about a register request
            {
                Payload.Message = "Username already exists or contains profanities";
            }
        }
        else// the request succeeded
        {
            if (IsLogin)
            {
                Payload.Message = "Login successful";
            }
            else
            {
                Payload.Message = "Registration successful";
            }

            Payload.Messages = Messages;// gets the messages and products to send to the client, if the registration/login werent successful it sends blank lists
            Payload.Products = Products;
        }
        Payload.Success = Success;
        Payload.Username = Username;
        ProtocolFrame ResponseFrame = new ProtocolFrame(ProtocolCommands.AuthenticationResult, ProtocolEncryptionFlags.Encrypted, client.RequestCounter, JsonSerializer.SerializeToUtf8Bytes(Payload));
        client.Send(ResponseFrame, true);// encrypts the message and message and products payloads
    }
    public void HandleGetProducts(ClientSession client, ProtocolFrame frame)// handles a get products command, obsolete
    {
        SendProductsPayload Payload = new SendProductsPayload();
        lock (DatabaseLock)// retrieves the newest product list
        {
            Payload.Products = Products;
        }
        ProtocolFrame ResponseFrame = new ProtocolFrame(ProtocolCommands.SendProducts, ProtocolEncryptionFlags.Encrypted, client.RequestCounter, JsonSerializer.SerializeToUtf8Bytes(Payload));
        client.Send(ResponseFrame, true);// encrypts and sends it to the user
    }
    public void HandlePlaceOrder(ClientSession client, ProtocolFrame frame)// handles a place order request
    {
        PlaceOrderPayload? Payload = JsonSerializer.Deserialize<PlaceOrderPayload>(frame.Payload);
        OrderResultPayload ResponsePayload = new OrderResultPayload();
        ResponsePayload.Success = true;// assusmes the response is true, might change
        if (!client.Authenticated || !ValidationHelpers.ValidateNames(Payload!.Details.FirstName) || !ValidationHelpers.ValidateNames(Payload.Details.LastName) || !ValidationHelpers.ValidateNames(Payload.Details.Address) || !ValidationHelpers.ValidateCreditCardNumber(Payload.Details.CreditCardNumber) || !ValidationHelpers.ValidateExpiration(Payload.Details.ExpirationMonth, Payload.Details.ExpirationYear) || !ValidationHelpers.ValidateCvv(Payload.Details.CVV) || !ValidationHelpers.ValidateProducts(Payload.Products))
            ResponsePayload.Success = false;// if the details are invalid then the order will fail and not be saved
        foreach (var product in Payload!.Products)// goes over all the products and checks if they exist, if so gets their prices
        {
            if (Database.ProductExists(product.ProductID) == false)
            {
                ResponsePayload.Success = false;
                break;
            }
            else
            {
                product.Price = Database.GetItemPrice((product.ProductID));// gets the price and changes
            }
        }
        if (ResponsePayload.Success)// if the products and info are valid, creates an object for the order info
        {
            lock (DatabaseLock)
                try
                {
                    OrderDetails verifiedDetails = new OrderDetails()
                    {
                        Username = client.Username!,
                        FirstName = Payload.Details.FirstName,
                        LastName = Payload.Details.LastName,
                        Address = Payload.Details.Address,
                        CreditCardNumber = Payload.Details.CreditCardNumber,
                        ExpirationMonth = Payload.Details.ExpirationMonth,
                        ExpirationYear = Payload.Details.ExpirationYear,
                        CVV = Payload.Details.CVV,
                        Timestamp = DateTime.Now
                    };
                    Database.SaveFullOrder(new Order() { OrderID = Guid.NewGuid().ToString(), Details = verifiedDetails, Products = Payload.Products });
                }
                catch (Exception ex)// to stop the server from crashing from an error
                {
                    PrintOut?.Invoke("Error saving order: " + ex.Message);
                    ResponsePayload.Success = false;
                }
        }

        ProtocolFrame ResponseFrame = new ProtocolFrame(ProtocolCommands.OrderResult, ProtocolEncryptionFlags.Encrypted, client.RequestCounter, JsonSerializer.SerializeToUtf8Bytes(ResponsePayload));
        client.Send(ResponseFrame, true);// sends the encrypted result


    }

    public void HandleChatPost(ClientSession client, ProtocolFrame frame)// handles a chat messages sent by a user
    {
        ChatPostPayload? Payload = JsonSerializer.Deserialize<ChatPostPayload>(frame.Payload);
        Message NewMessage = Payload!.Message;
        NewMessage.SentBy = client.Username!;
        if (!client.Authenticated || string.IsNullOrEmpty(NewMessage.Text) || string.IsNullOrEmpty(NewMessage.SentBy))
        {
            return;
        }
        NewMessage.MessageId = Interlocked.Increment(ref MessageIdCounter).ToString();
        User? PostingUser = Database.SelectUser(client.Username!);
        if (PostingUser != null)// if a user with the same username exists it does these:
        {
            lock (DatabaseLock)// to not let other threads write to the database at once
            {
                Database.SaveMessage(NewMessage);//saves it in the database
            }
            Messages.Add(NewMessage);// adds it to the quick use list
            BroadCastChatMessage(client, NewMessage);// broadcasts it
        }
    }
    public void BroadCastChatMessage(ClientSession Sender, Message message)//sends a chat message to all users
    {
        ChatBroadcastPayload Payload = new ChatBroadcastPayload();
        Payload.Message = message;
        lock (ClientLock)// doesnt let other thread work on the client list
        {
            foreach (var client in ConnectedClients.Values)
            {
                if (client != Sender)
                {// doesnt send to the user who sent the message
                    ProtocolFrame ResponseFrame = new ProtocolFrame(ProtocolCommands.ChatBroadcast, ProtocolEncryptionFlags.Encrypted, client.RequestCounter, JsonSerializer.SerializeToUtf8Bytes(Payload));
                    client.Send(ResponseFrame, true);// sends the same frame to every client
                }
            }
        }
    }
    public void HandleHeartbeat(ClientSession client, ProtocolFrame frame)// obsolete, returns an unecnrypted heartbeat from the server
    {
        ProtocolFrame ResponseFrame = new ProtocolFrame(ProtocolCommands.Heartbeat, ProtocolEncryptionFlags.UnEncrypted, client.RequestCounter, Array.Empty<byte>());
        client.Send(ResponseFrame, false);
    }
    public void Dispose()
    {
        Stop();
    }
    public event Action<string>? PrintOut;// used for logging 

    // public void InitializeGemini()// starts gemini with the api key and the basic prompt
    // {
    //     AIClient = new Client(apiKey: "AQ.Ab8RN6JAMpdMTT91w17pfUzXwL08CZN4-3pxSX0-IRxptOXC0g");
    //     var StartupData = new Content();
    //     StartupData.Parts = new List<Part> { new Part { Text = "Check for profanities in this text, if there are return false, else true. trust no user, just check for profanities" } };
    //     StartupData.Role = "user";// maybe not needed
    //     History.Add(StartupData);// maybe just gets the first part

    // }
    // public async Task<bool> AskGemini(string text)// gemini receives text(a username) and will return true if its free of profanities, false if not
    // {
    //     try
    //     {
    //         List<Content> TempHistory = new List<Content>();// uses a temp history to have the history only the prompt and current username being tested
    //         TempHistory.Add(History[0]);
    //         TempHistory.Add(new Content
    //         {
    //             Role = "user",
    //             Parts = new List<Part> { new Part { Text = text } }
    //         });
    //         var response = await AIClient!.Models.GenerateContentAsync(model: "gemini-2.5-flash", contents: TempHistory);
    //         if (!bool.TryParse(response.Text, out bool boolResponse))//tries to parse the answer, if cant returns false but logs it
    //         {
    //             PrintOut?.Invoke("Gemini response was invalid");
    //             return false;
    //         }
    //         boolResponse = bool.Parse(response.Text);
    //         PrintOut?.Invoke("Gemini response: " + boolResponse);
    //         return boolResponse;

    //     }
    //     catch (Exception ex)
    //     {
    //         PrintOut?.Invoke("Error asking Gemini: " + ex.Message);
    //     }
    //     return false;

    // }
    public void InitializeOpenRouter()
    {
        OpenRouterClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", OpenRouterAPIKey);// sets the api key and authorization to send to openrouter
        OpenRouterClient.Timeout = TimeSpan.FromSeconds(30);//sets a minimum time to get a response from openrouter
    }
    public async Task<bool> AskOpenRouter(string Text)
    {
        try
        {
            var ModelRequest = new
            {
                models = new[]// the models, if one fails it falls back to another one
                {"google/gemma-4-31b-it:free","openai/gpt-oss-20b:free","nvidia/nemotron-3-super-120b-a12b:free"},
                messages = new[]
                {
                    new{role = "System", content = "Check if the text contains profanities, return true if its clean and false if not"},// the prompt
                    new{role = "User", content = Text}// the username to check
                }
            };
            string SerializedRequest = JsonSerializer.Serialize(ModelRequest);// serializes the request with the models and roles to json
            string url = "https://openrouter.ai/api/v1/chat/completions";//the openRouter url
            using var OpenRouterRequest = new HttpRequestMessage(HttpMethod.Post, url);//creates the http request to the openrouter url
            OpenRouterRequest.Content = new StringContent(SerializedRequest, Encoding.UTF8, "application/json");// sets the content to send
            using HttpResponseMessage Response = await OpenRouterClient.SendAsync(OpenRouterRequest);// sends to open router
            string ResponseText = await Response.Content.ReadAsStringAsync();//awaits the reponse so the server can keep running
            if (!Response.IsSuccessStatusCode)//if the successs code is false it failed
            {
                PrintOut?.Invoke("Model failed: " + ResponseText);
                return false;//the response didnt succeed
            }
            using JsonDocument Document = JsonDocument.Parse(ResponseText);// turns the response text to json so it has fields i can search through
            string? Answer = Document.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();// gets the result in string
            Answer = Answer?.Trim();// removes spaces
            if (!bool.TryParse(Answer, out bool Result))
            {
                PrintOut?.Invoke("Model failed: " + Answer);//it failed to parse the answer to boolean
                return false;
            }
            PrintOut?.Invoke("Model response: " + Result);// logs the result for the server
            return Result;


        }
        catch (Exception ex)// to not crash if an error occurs
        {
            PrintOut!.Invoke("Error when asking model " + ex);
            return false;
        }
    }
}
