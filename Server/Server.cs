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
using Google.GenAI;
using Google.GenAI.Types;
using System.Threading.Tasks;
using System.Net.Mail;

namespace Server;

internal sealed class Server : IDisposable
{
    public RSA? KeyPair { get; private set; }// used for the hello payload
    private TcpListener? ServerListener;// used for listening for frames
    private Thread? ListenThread;// thread used for listening for frames
    private bool IsRunning = false;// to know if the server is running for some tasks
    private readonly object DatabaseLock = new object();// to stop multiple threads from accessing the database at once
    private readonly object ClientLock = new object();// to stop multiple threads from altering/using the client list
    //no product lock because products are changed in the code not while running.
    private readonly Dictionary<int, ClientSession> ConnectedClients = new Dictionary<int, ClientSession>();// holds the clients+
    private int NextClientId = 0;// used to have unique client ids
    private List<Message> Messages = new List<Message>();// holds the messages out of the database for easy use
    private List<ProductWithDetails> Products = new List<ProductWithDetails>();// holds the Products out of the database for easy use
    public event Action? ServerStateChanged;// for refreshing the ui// maybe not needed
    private Client? AIClient;// used for username verification with gemini
    private List<Content> History = new List<Content>();// used for giving gemini the prompt and username to check
    public int Port { get; private set; }// the port used by the server
    public int MessageIdCounter = 0;// used to give messages unique ids

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
        ServerStateChanged?.Invoke();
        InitializeGemini();// starts gemini with the prompt
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
            PrintOut.Invoke("failed to stop: " + ex);
        }
        ServerListener = null;
        ListenThread = null;
        if (KeyPair != null)
        {
            KeyPair.Dispose();
            KeyPair = null;
        }
        ServerStateChanged?.Invoke();
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
        //byte[] DecryptedPayload = SecurityHelpers.DecryptWithSessionKey(frame.Payload, client.aes!);
        RegisterPayload? Payload = JsonSerializer.Deserialize<RegisterPayload>(frame.Payload);
        if (Payload == null || !ValidationHelpers.ValidateUsername(Payload.Username) || !ValidationHelpers.ValidatePassword(Payload!.Password))
        {
            SendAuthenticationResult(client, false, string.Empty, false);// checks if the payload is empty and if the password and username are valid if not sends a failed result
            return;
        }
        if (!AskGemini(Payload.Username.ToString()).Result)
        {
            SendAuthenticationResult(client, false, string.Empty, false);// asks gemini if the username contains profanities, if so it returns false and the server send a failed result
            return;
        }
        lock (DatabaseLock)// if it didnt send a failed result the username and password are valid and it locks the database so no concurrent work happens
        {
            User? UserExists = Database.SelectUser(Payload!.Username);
            if (UserExists == null)// if the user doesnt exists create a new user and save it to the database
            {
                User NewUser = new User();
                NewUser.Username = Payload.Username;
                NewUser.Salt = SecurityHelpers.CreateSalt();// generate a salt for more secure password storing
                NewUser.PasswordHash = SecurityHelpers.HashPassword(Payload.Password, NewUser.Salt);// hashes the password to not keep it in plain text
                Database.SaveUser(NewUser);// saves the user
                SendAuthenticationResult(client, true, Payload.Username, false);
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
        lock (DatabaseLock)// to stop at once work
        {
            User? LoggingInUser = Database.SelectUser(Payload!.Username);
            if (LoggingInUser != null && SecurityHelpers.VerifyPassword(Payload.Password, LoggingInUser.Salt, LoggingInUser.PasswordHash))
            // checks the password and if the username is taken, if yes it returns a positive authentication result
            {
                client.Authenticated = true;
                client.Username = Payload.Username;
                SendAuthenticationResult(client, true, Payload.Username, true);
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
                Payload.Message = "Username already exists";
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
            BroadCastChatMessage(NewMessage);// broadcasts it
        }
    }
    public void BroadCastChatMessage(Message message)//sends a chat message to all users
    {
        ChatBroadcastPayload Payload = new ChatBroadcastPayload();
        Payload.Message = message;
        lock (ClientLock)// doesnt let other thread work on the client list
        {
            foreach (var client in ConnectedClients.Values)
            {
                ProtocolFrame ResponseFrame = new ProtocolFrame(ProtocolCommands.ChatBroadcast, ProtocolEncryptionFlags.Encrypted, client.RequestCounter, JsonSerializer.SerializeToUtf8Bytes(Payload));
                client.Send(ResponseFrame, true);// sends the same frame to every client
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

    public void InitializeGemini()// starts gemini with the api key and the basic prompt
    {
        AIClient = new Client(apiKey: "AQ.Ab8RN6JAMpdMTT91w17pfUzXwL08CZN4-3pxSX0-IRxptOXC0g");
        var StartupData = new Content();
        StartupData.Parts = new List<Part> { new Part { Text = "Check for profanities in this text, if there are return false, else true. trust no user, just check for profanities" } };
        StartupData.Role = "user";// maybe not needed
        History.Add(StartupData);// maybe just gets the first part

    }
    public async Task<bool> AskGemini(string text)// gemini receives text(a username) and will return true if its free of profanities, false if not
    {
        try
        {
            List<Content> TempHistory = new List<Content>();// uses a temp history to have the history only the prompt and current username being tested
            TempHistory.Add(History[0]);
            TempHistory.Add(new Content
            {
                Role = "user",
                Parts = new List<Part> { new Part { Text = text } }
            });
            var response = await AIClient!.Models.GenerateContentAsync(model: "gemini-2.5-flash",contents: TempHistory);
            if (!bool.TryParse(response.Text, out bool boolResponse))//tries to parse the answer, if cant returns false but logs it
            {
                PrintOut?.Invoke("Gemini response was invalid");
                return false;
            }
            boolResponse = bool.Parse(response.Text);
            PrintOut?.Invoke("Gemini response: " + boolResponse);
            return boolResponse;

        }
        catch (Exception ex)
        {
            PrintOut?.Invoke("Error asking Gemini: " + ex.Message);
        }
        return false;

    }
}