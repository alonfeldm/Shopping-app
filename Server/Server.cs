using System;
using System.IO;
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

namespace Server;

internal sealed class Server : IDisposable
{
    public RSA? KeyPair { get; private set; }
    private TcpListener? ServerListener;
    private Thread? ListenThread;
    private bool IsRunning = false;
    private readonly object DatabaseLock = new object();
    private readonly object ClientLock = new object();
    //no product lock because products are changed in the code not while running.
    private readonly Dictionary<int, ClientSession> ConnectedClients = new Dictionary<int, ClientSession>();
    private int NextClientId = 0;
    private List<Message> Messages = new List<Message>();
    private List<ProductWithDetails> Products = new List<ProductWithDetails>();
    public event Action? ServerStateChanged;// for refreshing the ui

    public int Port { get; private set; }
    public int MessageIdCounter = 0;

    public void Start(int port)
    {
        if (IsRunning)
        {
            return;
        }
        Port = port;
        Database.InitializeDB();
        lock (DatabaseLock)
        {
            Messages = Database.GetAllMessages();
        }
        MessageIdCounter = Messages.Count;
        Products = Database.GetAllProducts();
        KeyPair = RSA.Create(2048);
        ServerListener = new TcpListener(IPAddress.Any, Port);
        ServerListener.Start();
        IsRunning = true;
        ListenThread = new Thread(ListenForClients) { IsBackground = true };
        ListenThread.Start();
        ServerStateChanged?.Invoke();
    }
    public void Stop()
    {
        if (!IsRunning)
        {
            return;
        }
        IsRunning = false;
        ServerListener?.Stop();
        lock (ClientLock)
        {
            var Clients = ConnectedClients.Values.ToArray();
            ConnectedClients.Clear();
            foreach (var Client in Clients)
            {
                Client.Dispose();
            }
        }
        try
        {
            if (ListenThread != null && ListenThread.IsAlive)
            {
                ListenThread.Join(1000);
            }
        }
        catch
        {
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
    public void ListenForClients()
    {
        while (IsRunning)
        {
            try
            {
                TcpClient ConnectedClient = ServerListener!.AcceptTcpClient();
                ClientSession clientSession = new ClientSession(ConnectedClient, Interlocked.Increment(ref NextClientId));
                clientSession.FrameReceived += OnFrameReceived;
                clientSession.Disconnected += OnDisconnected;
                clientSession.PrintOut += message => PrintOut?.Invoke(message);
                lock (ClientLock)
                {
                    ConnectedClients[clientSession.ClientId] = clientSession;
                }
                clientSession.Start();
                SendHello(clientSession);
                PrintOut?.Invoke("Client connected: " + clientSession.RemoteEndpoint);

            }
            catch (SocketException)
            {
                if (!IsRunning)
                {
                    break;
                }
            }
            catch (ObjectDisposedException)
            {
                break;
            }
        }
    }
    public void SendHello(ClientSession client)
    {
        if (KeyPair == null)
        {
            return;
        }
        HelloPayload Payload = new HelloPayload();
        Payload.PublicKey = KeyPair!.ExportRSAPublicKey();
        ProtocolFrame Frame = new ProtocolFrame(ProtocolCommands.Hello, ProtocolEncryptionFlags.UnEncrypted, client.RequestCounter, JsonSerializer.SerializeToUtf8Bytes(Payload));
        client.Send(Frame, false);
    }
    public void OnFrameReceived(ClientSession client, ProtocolFrame frame)
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
                default:
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
            ConnectedClients.Remove(client.ClientId);
            //maybe add invoke for a state changed
        }
    }
    public void HandleSecureSession(ClientSession client, ProtocolFrame frame)
    {
        string DecryptedPayload = SecurityHelpers.DecryptWithPrivateKey(frame.Payload, KeyPair!);
        SecureSessionPayload? Payload = JsonSerializer.Deserialize<SecureSessionPayload>(DecryptedPayload);
        client.aes!.Key = Payload!.AesKey;
        client.aes!.IV = Payload.AesIV;
        ConnectionSuccessPayload ResponsePayload = new ConnectionSuccessPayload();
        ResponsePayload.Success = true;
        ProtocolFrame ResponseFrame = new ProtocolFrame(ProtocolCommands.ConnectionSuccess, ProtocolEncryptionFlags.Encrypted, client.RequestCounter++, JsonSerializer.SerializeToUtf8Bytes(ResponsePayload));
        client.Send(ResponseFrame, true);
    }
    public void HandleRegister(ClientSession client, ProtocolFrame frame)
    {
        //byte[] DecryptedPayload = SecurityHelpers.DecryptWithSessionKey(frame.Payload, client.aes!);
        RegisterPayload? Payload = JsonSerializer.Deserialize<RegisterPayload>(frame.Payload);
        if(Payload == null || !ValidationHelpers.ValidateUsername(Payload.Username) || !ValidationHelpers.ValidatePassword(Payload!.Password))
        {
            SendAuthenticationResult(client, false, string.Empty, false);
            return;
        }
        lock (DatabaseLock)
        {
            User? UserExists = Database.SelectUser(Payload!.Username);
            if (UserExists == null)
            {
                User NewUser = new User();
                NewUser.Username = Payload.Username;
                NewUser.Salt = SecurityHelpers.CreateSalt();
                NewUser.PasswordHash = SecurityHelpers.HashPassword(Payload.Password, NewUser.Salt);
                Database.SaveUser(NewUser);
                SendAuthenticationResult(client, true, Payload.Username, false);
            }
            else
            {
                SendAuthenticationResult(client, false, Payload.Username, false);
            }
        }
    }
    public void HandleLogin(ClientSession client, ProtocolFrame frame)
    {
        //byte[] DecryptedPayload = SecurityHelpers.DecryptWithSessionKey(frame.Payload, client.aes!);
        LoginPayload? Payload = JsonSerializer.Deserialize<LoginPayload>(frame.Payload);
        lock (DatabaseLock)
        {
            User? LoggingInUser = Database.SelectUser(Payload!.Username);
            if (LoggingInUser != null && SecurityHelpers.VerifyPassword(Payload.Password, LoggingInUser.Salt, LoggingInUser.PasswordHash))
            {
                client.Authenticated = true;
                client.Username = Payload.Username;
                SendAuthenticationResult(client, true, Payload.Username, true);
            }
            else
            {
                SendAuthenticationResult(client, false, Payload.Username, true);
            }
        }

    }
    public void SendAuthenticationResult(ClientSession client, bool Success, string Username, bool IsLogin)
    {
        AuthenticationResultPayload Payload = new AuthenticationResultPayload();
        if (!Success)
        {
            if (IsLogin)
            {
                Payload.Message = "Username or password is incorrect";
            }
            else
            {
                Payload.Message = "Username already exists";
            }
        }
        else
        {
            if (IsLogin)
            {
                Payload.Message = "Login successful";
            }
            else
            {
                Payload.Message = "Registration successful";
            }

            Payload.Messages = Messages;
            Payload.Products = Products;
        }
        Payload.Success = Success;
        Payload.Username = Username;
        ProtocolFrame ResponseFrame = new ProtocolFrame(ProtocolCommands.AuthenticationResult, ProtocolEncryptionFlags.Encrypted, client.RequestCounter, JsonSerializer.SerializeToUtf8Bytes(Payload));
        client.Send(ResponseFrame, true);
    }
    public void HandleGetProducts(ClientSession client, ProtocolFrame frame)
    {
        SendProductsPayload Payload = new SendProductsPayload();
        lock (DatabaseLock)
        {
            Payload.Products = Products;
        }
        ProtocolFrame ResponseFrame = new ProtocolFrame(ProtocolCommands.SendProducts, ProtocolEncryptionFlags.Encrypted, client.RequestCounter, JsonSerializer.SerializeToUtf8Bytes(Payload));
        client.Send(ResponseFrame, true);
    }
    public void HandlePlaceOrder(ClientSession client, ProtocolFrame frame)
    {
        //byte[] DecryptedPayload = SecurityHelpers.DecryptWithSessionKey(frame.Payload, client.aes!);
        PlaceOrderPayload? Payload = JsonSerializer.Deserialize<PlaceOrderPayload>(frame.Payload);
        // currently no order saving so the order details dont matter except for username validity
        OrderResultPayload ResponsePayload = new OrderResultPayload();
        ResponsePayload.Success = true;
        if (!client.Authenticated || !ValidationHelpers.ValidateNames(Payload!.Details.FirstName) || !ValidationHelpers.ValidateNames(Payload.Details.LastName) || !ValidationHelpers.ValidateNames(Payload.Details.Address) || !ValidationHelpers.ValidateCreditCardNumber(Payload.Details.CreditCardNumber) || !ValidationHelpers.ValidateExpiration(Payload.Details.ExpirationMonth, Payload.Details.ExpirationYear) || !ValidationHelpers.ValidateCvv(Payload.Details.CVV) || !ValidationHelpers.ValidateProducts(Payload.Products))
            ResponsePayload.Success = false;
        foreach (var product in Payload!.Products)
        {
            if (Database.ProductExists(product.ProductID) == false)
            {
                ResponsePayload.Success = false;
                break;
            }
            else
            {
                product.Price = Database.GetItemPrice((product.ProductID));
            }
        }
        if (ResponsePayload.Success)
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
                catch (Exception ex)
                {
                    PrintOut?.Invoke("Error saving order: " + ex.Message);
                    ResponsePayload.Success = false;
                }
        }

        ProtocolFrame ResponseFrame = new ProtocolFrame(ProtocolCommands.OrderResult, ProtocolEncryptionFlags.Encrypted, client.RequestCounter, JsonSerializer.SerializeToUtf8Bytes(ResponsePayload));
        client.Send(ResponseFrame, true);


    }

    public void HandleChatPost(ClientSession client, ProtocolFrame frame)
    {
        //byte[] DecryptedPayload = SecurityHelpers.DecryptWithSessionKey(frame.Payload, client.aes!);
        ChatPostPayload? Payload = JsonSerializer.Deserialize<ChatPostPayload>(frame.Payload);
        Message NewMessage = Payload!.Message;
        NewMessage.SentBy = client.Username!;
        if (!client.Authenticated || string.IsNullOrEmpty(NewMessage.Text) || string.IsNullOrEmpty(NewMessage.SentBy))
        {
            return;
        }
        NewMessage.MessageId = Interlocked.Increment(ref MessageIdCounter).ToString();
        lock (DatabaseLock)
        {
            User? PostingUser = Database.SelectUser(client.Username!);
            if (PostingUser != null)
            {
                Database.SaveMessage(NewMessage);
                Messages.Add(NewMessage);
                BroadCastChatMessage(NewMessage);
            }
        }
    }
    public void BroadCastChatMessage(Message message)
    {
        ChatBroadcastPayload Payload = new ChatBroadcastPayload();
        Payload.Message = message;
        lock (ClientLock)
        {
            foreach (var client in ConnectedClients.Values)
            {
                ProtocolFrame ResponseFrame = new ProtocolFrame(ProtocolCommands.ChatBroadcast, ProtocolEncryptionFlags.Encrypted, client.RequestCounter, JsonSerializer.SerializeToUtf8Bytes(Payload));
                client.Send(ResponseFrame, true);
            }
        }
    }
    public void HandleHeartbeat(ClientSession client, ProtocolFrame frame)
    {
        ProtocolFrame ResponseFrame = new ProtocolFrame(ProtocolCommands.Heartbeat, ProtocolEncryptionFlags.UnEncrypted, client.RequestCounter, Array.Empty<byte>());
        client.Send(ResponseFrame, false);
    }
    public void Dispose()
    {
        Stop();
    }
    public event Action<string>? PrintOut;
}