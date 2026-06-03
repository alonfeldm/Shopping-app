using System;
using System.Security.Cryptography;
using System.Net.Sockets;
using System.Threading;
using SharedLibraries;
using SharedLibraries.Payloads;
using System.Text.Json;
using System.Collections.Generic;

namespace Client;

public class Client
{
    private Aes? aes { get; set; }
    public bool SecureSessionConnected { get; set; } = false;
    public string? Username { get; set; }
    private readonly object SendLock = new object();
    private NetworkStream? NetworkStream { get; set; }
    private TcpClient? TcpClient { get; set; }
    private Thread? ListenThread { get; set; }
    private int NextRequestId { get; set; } = 0;
    private bool Disposed { get; set; } = false;
    private List<ProductWithDetails> Products { get; set; } = new List<ProductWithDetails>();
    private List<Message> Messages { get; set; } = new List<Message>();
    public bool TcpConnected { get; set; } = false;
    public bool LoggedIn { get; set; } = false;
    public void Start(string Ip, int Port)
    {
        if (!TcpConnected)
        {
            SecureSessionConnected = false;
            TcpClient = new TcpClient(Ip, Port);
            TcpClient!.NoDelay = true;
            NetworkStream = TcpClient.GetStream();
            ListenThread = new Thread(Listen);
            { ListenThread.IsBackground = true; }
            ListenThread.Start();
            Disposed = false;
            TcpConnected = true;
        }
    }

    public void Stop()
    {
        if(aes != null)
        {
            ProtocolFrame DisconnectFrame = new ProtocolFrame(ProtocolCommands.Disconnect, ProtocolEncryptionFlags.UnEncrypted, NextRequestId++, Array.Empty<byte>());
            Send(DisconnectFrame, true);
        }
        try
        {
            Disposed = true;
            NetworkStream?.Close();
            TcpClient?.Close();
            SecureSessionConnected = false;
            aes = null;
            Username = null;
            TcpConnected = false;
        }
        catch (Exception ex)
        {
            DisplayLog?.Invoke("Failed to handle disconnect frame: " + ex.Message);
        }
    }
    public void Listen()
    {
        if (TcpConnected)
        {
            try
            {
                while (!Disposed)
                {
                    ProtocolFrame? Frame = Protocol.ReadFrame(NetworkStream!);
                    if (Frame == null)
                    {
                        DisplayLog?.Invoke("Frame couldn't be read, connection might be lost");
                        break;
                    }
                    HandleFrame(Frame);
                }
            }
            catch (Exception ex)
            {
                DisplayLog?.Invoke("Frame couldn't be read, connection might be lost: " + ex.Message);
            }
        }
    }
    public void Send(ProtocolFrame Frame, bool Encrypted)
    {
        byte[] PayloadToSend = Frame.Payload;
        ProtocolEncryptionFlags Flag = ProtocolEncryptionFlags.UnEncrypted;
        if (Encrypted)
        {
            if (aes == null)
            {
                throw new InvalidOperationException("doesn't have an aes key");
            }
            PayloadToSend = SecurityHelpers.EncryptWithSessionKey(PayloadToSend, aes);
            Flag = ProtocolEncryptionFlags.Encrypted;

        }

        lock (SendLock)
        {
            if (Disposed)
            {
                return;
            }
            Protocol.WriteFrame(NetworkStream!, new ProtocolFrame(Frame.Command, Flag, NextRequestId++, PayloadToSend));
            NetworkStream!.Flush();
        }
    }
    public void HandleFrame(ProtocolFrame Frame)
    {
        try
        {
            switch (Frame.Command)
            {
                case ProtocolCommands.Hello:
                    try
                    {
                        HandleHello(Frame);
                    }
                    catch (Exception ex)
                    {
                        DisplayLog?.Invoke("Failed to handle hello frame: " + ex.Message);
                    }
                    break;
                case ProtocolCommands.ConnectionSuccess:
                    try
                    {
                        HandleConnectionSuccess(Frame);
                    }
                    catch (Exception ex)
                    {
                        DisplayLog?.Invoke("Failed to handle connection success frame: " + ex.Message);
                    }
                    break;
                case ProtocolCommands.AuthenticationResult:
                    try
                    {
                        HandleAuthenticationResult(Frame);
                    }
                    catch (Exception ex)
                    {
                        DisplayLog?.Invoke("Failed to handle authentication result frame: " + ex.Message);
                    }
                    break;
                case ProtocolCommands.SendProducts:
                    try
                    {
                        HandleSendProducts(Frame);
                    }
                    catch (Exception ex)
                    {
                        DisplayLog?.Invoke("Failed to handle send products frame: " + ex.Message);
                    }
                    break;
                case ProtocolCommands.OrderResult:
                    try
                    {
                        HandleOrderResult(Frame);
                    }
                    catch (Exception ex)
                    {
                        DisplayLog?.Invoke("Failed to handle order result frame: " + ex.Message);
                    }
                    break;
                case ProtocolCommands.ChatBroadcast:
                    try
                    {
                        HandleChatBroadcast(Frame);
                    }
                    catch (Exception ex)
                    {
                        DisplayLog?.Invoke("Failed to handle chat broadcast frame: " + ex.Message);
                    }
                    break;
                case ProtocolCommands.Error:
                    try
                    {
                        HandleError(Frame);
                    }
                    catch (Exception ex)
                    {
                        DisplayLog?.Invoke("Failed to handle error frame: " + ex.Message);
                    }
                    break;
                // case ProtocolCommands.Disconnect:
                //     try
                //     {
                //         Disposed = true;
                //         NetworkStream?.Close();
                //         TcpClient?.Close();
                //         SecureSessionConnected = false;
                //         aes = null;
                //         Username = null;
                //         TcpConnected = false;
                //     }
                //     catch (Exception ex)
                //     {
                //         DisplayLog?.Invoke("Failed to handle disconnect frame: " + ex.Message);
                //     }
                //     break;
                default:
                    DisplayLog?.Invoke("Received unknown command: " + Frame.Command);
                    break;
            }
        }
        catch (Exception ex)
        {
            DisplayLog?.Invoke("Error occurred while handling frame: " + ex.Message);
        }
    }
    public void HandleHello(ProtocolFrame Frame)
    {
        HelloPayload? Payload = JsonSerializer.Deserialize<HelloPayload>(Frame.Payload);
        RSA rsa = RSA.Create();
        rsa.ImportRSAPublicKey(Payload!.PublicKey, out _);
        aes = SecurityHelpers.CreateAes();
        SecureSessionPayload secureSessionPayload = new SecureSessionPayload(aes);
        byte[] PayloadToSend = JsonSerializer.SerializeToUtf8Bytes(secureSessionPayload);
        PayloadToSend = SecurityHelpers.EncryptWithPublicKey(PayloadToSend, rsa);
        ProtocolFrame ResponseFrame = new ProtocolFrame(ProtocolCommands.SecureSession, ProtocolEncryptionFlags.UnEncrypted, NextRequestId++, PayloadToSend);
        Send(ResponseFrame, false);
    }
    public void HandleConnectionSuccess(ProtocolFrame Frame)
    {
        byte[] DecryptedPayload = SecurityHelpers.DecryptWithSessionKey(Frame.Payload, aes!);
        ConnectionSuccessPayload? Payload = JsonSerializer.Deserialize<ConnectionSuccessPayload>(DecryptedPayload);
        if (Payload!.Success)
        {
            DisplayLog?.Invoke("Connected");
            SecureSessionConnected = true;
        }
        else
        {
            DisplayLog?.Invoke("Failed to connect, try again");
        }
    }
    public void HandleAuthenticationResult(ProtocolFrame Frame)
    {
        byte[] DecryptedPayload = SecurityHelpers.DecryptWithSessionKey(Frame.Payload, aes!);
        AuthenticationResultPayload? Payload = JsonSerializer.Deserialize<AuthenticationResultPayload>(DecryptedPayload);
        if (Payload!.Success)
        {
            Username = Payload.Username;
            DisplayLog?.Invoke("Authenticated as " + Payload.Username);
            LoggedIn = true;
            Products = Payload.Products;
            Messages = Payload.Messages;
            ClearStore?.Invoke();
            for (int i = 0; i < Products.Count; i++)
            {
                AppendStore?.Invoke(Products[i]);
            }
            ClearMessages?.Invoke();
            for (int i = 0; i < Messages.Count; i++)
            {
                DisplayMessage?.Invoke(Messages[i].SentBy, Messages[i].Text);
            }
        }
        else
        {
            DisplayLog?.Invoke("Authentication failed");
        }
    }
    public void HandleSendProducts(ProtocolFrame Frame)
    {
        byte[] DecryptedPayload = SecurityHelpers.DecryptWithSessionKey(Frame.Payload, aes!);
        SendProductsPayload? Payload = JsonSerializer.Deserialize<SendProductsPayload>(DecryptedPayload);
        Products = Payload!.Products;
        ClearStore?.Invoke();
        for (int i = 0; i < Products.Count; i++)
        {
            AppendStore?.Invoke(Products[i]);
        }
    }
    public void HandleOrderResult(ProtocolFrame Frame)
    {
        byte[] DecryptedPayload = SecurityHelpers.DecryptWithSessionKey(Frame.Payload, aes!);
        OrderResultPayload? Payload = JsonSerializer.Deserialize<OrderResultPayload>(DecryptedPayload);
        if (Payload!.Success)
        {
            DisplayLog?.Invoke("Order successful");//maybe add to order list
        }
        else
        {
            DisplayLog?.Invoke("Order failed ");
        }
    }
    public void HandleChatBroadcast(ProtocolFrame Frame)
    {
        byte[] DecryptedPayload = SecurityHelpers.DecryptWithSessionKey(Frame.Payload, aes!);
        ChatBroadcastPayload? Payload = JsonSerializer.Deserialize<ChatBroadcastPayload>(DecryptedPayload);
        Messages.Add(Payload!.Message);
        DisplayMessage?.Invoke(Payload.Message.SentBy, Payload.Message.Text);

    }
    public void HandleError(ProtocolFrame Frame)
    {
        byte[] DecryptedPayload = SecurityHelpers.DecryptWithSessionKey(Frame.Payload, aes!);
        ErrorPayload? Payload = JsonSerializer.Deserialize<ErrorPayload>(DecryptedPayload);
        DisplayLog?.Invoke("Error from server: " + Payload!.ErrorMessage);
    }
    public event Action<string>? PrintOut;
    public event Action<string>? DisplayLog;
    public void CreateRegisterFrame(string Username, string Password)
    {
        if (SecureSessionConnected == false)
        {
            DisplayLog?.Invoke("Not connected to server");
            return;
        }
        RegisterPayload Payload = new RegisterPayload();
        Payload.Username = Username;
        Payload.Password = Password;
        byte[] PayloadToSend = JsonSerializer.SerializeToUtf8Bytes(Payload);
        Send(new ProtocolFrame(ProtocolCommands.Register, ProtocolEncryptionFlags.UnEncrypted, NextRequestId++, PayloadToSend), true);
    }
    public void CreateLoginFrame(string Username, string Password)
    {
        if (SecureSessionConnected == false)
        {
            DisplayLog?.Invoke("Not connected to server");
            return;
        }
        LoginPayload Payload = new LoginPayload();
        Payload.Username = Username;
        Payload.Password = Password;
        byte[] PayloadToSend = JsonSerializer.SerializeToUtf8Bytes(Payload);
        Send(new ProtocolFrame(ProtocolCommands.Login, ProtocolEncryptionFlags.UnEncrypted, NextRequestId++, PayloadToSend), true);
    }
    public void CreateMessageFrame(string message)
    {
        ChatPostPayload Payload = new ChatPostPayload();
        Message Message = new Message();
        Message.Text = message;
        Message.SentBy = Username ?? "Unknown";
        Message.Timestamp = DateTime.Now;
        Message.MessageId = 0.ToString(); //server will change this to a real id
        Payload.Message = Message;
        byte[] PayloadToSend = JsonSerializer.SerializeToUtf8Bytes(Payload);
        Send(new ProtocolFrame(ProtocolCommands.ChatPost, ProtocolEncryptionFlags.UnEncrypted, NextRequestId++, PayloadToSend), true);
    }
    public void CreateOrderFrame(OrderDetails details, List<ProductAndQuantity> cart)
    {
        PlaceOrderPayload Payload = new PlaceOrderPayload();
        Payload.Products = cart;
        Payload.Details = details;
        byte[] PayloadToSend = JsonSerializer.SerializeToUtf8Bytes(Payload);
        Send(new ProtocolFrame(ProtocolCommands.PlaceOrder, ProtocolEncryptionFlags.UnEncrypted, NextRequestId++, PayloadToSend), true);
    }
    public event Action<string, string>? DisplayMessage;
    public event Action<ProductWithDetails>? AppendStore;
    public event Action? ClearStore;
    public event Action? ClearMessages;
}
