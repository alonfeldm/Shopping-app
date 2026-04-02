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
using System.Drawing.Text;
using System.Data;

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
        lock(DatabaseLock)
        {
            Messages = Database.GetAllMessages();
        }
        Products = Database.GetAllProducts();
        KeyPair = RSA.Create(2048);
        ServerListener = new TcpListener(IPAddress.Any, Port);
        ServerListener.Start();
        IsRunning = true;
        ListenThread = new Thread(ListenForClients){IsBackground = true};
        ListenThread.Start();
        ServerStateChanged?.Invoke();
    }
    public void Stop()
    {
        if(!IsRunning)
        {
            return;
        }
        IsRunning = false;
        ServerListener?.Stop();
        lock(ClientLock)
        {
            var Clients = ConnectedClients.Values.ToArray();
            ConnectedClients.Clear();
            foreach(var Client in Clients)
            {
                Client.Dispose();
            }
        }
        try
        {
            if(ListenThread != null && ListenThread.IsAlive)
            {
                ListenThread.Join(1000);
            }
        }
        catch
        {
        }
        ServerListener = null;
        ListenThread = null;
        if(KeyPair != null)
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
                lock (ClientLock)
                {
                    ConnectedClients[clientSession.ClientId] = clientSession;
                }
                clientSession.Start();
                SendHello(clientSession);
                
            }
            catch (SocketException)
            {
                if (!IsRunning)
                {
                    break;
                }
            }
            catch(ObjectDisposedException)
            {
                break;
            }
        }
    }
    public void SendHello(ClientSession client)
    {
        if(KeyPair == null)
        {
            return;
        }
        HelloPayload Payload  = new HelloPayload();
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
                    HandleSecureSession(client, frame);
                    break;
                case ProtocolCommands.Register:
                    HandleRegister(client, frame);
                    break;
                case ProtocolCommands.Login:
                    HandleLogin(client, frame);
                    break;
                case ProtocolCommands.GetProducts:
                    HandleGetProducts(client, frame);
                    break;
                case ProtocolCommands.PlaceOrder:
                    HandlePlaceOrder(client, frame);
                    break;
                case ProtocolCommands.ChatPost:
                    HandleChatPost(client, frame);
                    break;
                case ProtocolCommands.Disconnect:
                    client.Dispose();
                    break;
                case ProtocolCommands.Heartbeat:
                    HandleHeartbeat(client, frame);
                    break;
                default:
                    break;
            }
        }
        catch
        {
            //general eeror catch
        }
    }
    public void OnDisconnected(ClientSession client)
    {
        lock(ClientLock)
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
        ProtocolFrame ResponseFrame  = new ProtocolFrame(ProtocolCommands.ConnectionSuccess, ProtocolEncryptionFlags.Encrypted, client.RequestCounter++, JsonSerializer.SerializeToUtf8Bytes(ResponsePayload));
        client.Send(ResponseFrame, true);
    }
    public void HandleRegister(ClientSession client, ProtocolFrame frame)
    {
        byte[] DecryptedPayload = SecurityHelpers.DecryptWithSessionKey(frame.Payload, client.aes!);
        RegisterPayload? Payload = JsonSerializer.Deserialize<RegisterPayload>(DecryptedPayload);
        lock (DatabaseLock)
        {
            User? UserExists = Database.SelectUser(Payload!.Username);
            if(UserExists == null)
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
        byte[] DecryptedPayload = SecurityHelpers.DecryptWithSessionKey(frame.Payload, client.aes!);
        LoginPayload? Payload = JsonSerializer.Deserialize<LoginPayload>(DecryptedPayload);
        lock (DatabaseLock)
        {
            User? LoggingInUser = Database.SelectUser(Payload!.Username);
            if(LoggingInUser != null && SecurityHelpers.VerifyPassword(Payload.Password, LoggingInUser.Salt, LoggingInUser.PasswordHash))
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
        if(!Success)
        {
            if(IsLogin)
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
            if(IsLogin)
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
        lock(DatabaseLock)
        {
            Payload.Products = Database.GetAllProducts();
        }
        ProtocolFrame ResponseFrame = new ProtocolFrame(ProtocolCommands.SendProducts, ProtocolEncryptionFlags.Encrypted, client.RequestCounter, JsonSerializer.SerializeToUtf8Bytes(Payload));
        client.Send(ResponseFrame, true);
    }
    public void HandlePlaceOrder(ClientSession client, ProtocolFrame frame){
        byte[] DecryptedPayload = SecurityHelpers.DecryptWithSessionKey(frame.Payload, client.aes!);
        PlaceOrderPayload? Payload = JsonSerializer.Deserialize<PlaceOrderPayload>(DecryptedPayload);
        // currently no order saving so the order details dont matter except for username validity
        lock (DatabaseLock)
        {
            User? OrderingUser = Database.SelectUser(Payload!.Username);
            if(OrderingUser != null)
            {
                OrderResultPayload ResponsePayload = new OrderResultPayload();
                ResponsePayload.Success = true;
                ProtocolFrame ResponseFrame = new ProtocolFrame(ProtocolCommands.OrderResult, ProtocolEncryptionFlags.Encrypted, client.RequestCounter, JsonSerializer.SerializeToUtf8Bytes(ResponsePayload));
                client.Send(ResponseFrame, true);
            }
        }

    }
    public void HandleChatPost(ClientSession client, ProtocolFrame frame)
    {
        byte[] DecryptedPayload = SecurityHelpers.DecryptWithSessionKey(frame.Payload, client.aes!);
        ChatPostPayload? Payload = JsonSerializer.Deserialize<ChatPostPayload>(DecryptedPayload);
        Message NewMessage = Payload!.Message;
        NewMessage.MessageId = Interlocked.Increment(ref MessageIdCounter).ToString();
        lock(DatabaseLock)
        {
            User? PostingUser = Database.SelectUser(Payload!.Message.SentBy);
            if(PostingUser != null)
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
        lock(ClientLock)
        {
            foreach(var client in ConnectedClients.Values)
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

}