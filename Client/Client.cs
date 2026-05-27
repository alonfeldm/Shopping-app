using System;
using System.Security.Cryptography;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using SharedLibraries;
using SharedLibraries.Payloads;
using System.Text.Json;
using System.Configuration;
using System.Collections.Generic;

namespace Client;

public class Client
{
    private Aes? aes {get; set;}
    private bool Connected {get; set;} = false;
    private string? Username {get; set;}
    private readonly object SendLock = new object();
    private NetworkStream? NetworkStream {get; set;}
    private TcpClient? TcpClient {get; set;}
    private Thread? ListenThread {get; set;}
    private int NextRequestId {get; set;} = 0;
    private bool Disposed {get; set;} = false;
    private List<ProductWithDetails> Products {get; set;} = new List<ProductWithDetails>();
    private List<Message> Messages {get; set;} = new List<Message>();
    private bool TcpConnected {get; set;} = false;

    public void Start(string Ip,int Port)
    {
        if(!Connected){
            TcpClient = new TcpClient(Ip, Port);
            TcpClient!.NoDelay = true;
            Connected = true;
            NetworkStream = TcpClient.GetStream();
            ListenThread = new Thread(Listen);{ListenThread.IsBackground = true;}
            ListenThread.Start();
            Disposed = false;
            TcpConnected = false;
        }
    }
    
    public void Stop()
    {
        Disposed = true;
        NetworkStream?.Close();
        TcpClient?.Close();
        Connected = false;
        aes = null;
        Username = null;
        TcpConnected = false;
    }
    public void Listen()
    {
        if(Connected){
            try
            {
                while (!Disposed)
                {
                    ProtocolFrame? Frame = Protocol.ReadFrame(NetworkStream!);
                    if(Frame == null)
                    {
                        break;
                    }
                    HandleFrame(Frame);
                }
            }
            catch
            {
            //do something  
            }
        }
    }
    public void Send(ProtocolFrame Frame, bool Encrypted)
    {
        byte[] PayloadToSend = Frame.Payload;
        ProtocolEncryptionFlags Flag = ProtocolEncryptionFlags.UnEncrypted;
        if (Encrypted)
        {
            if(aes == null)
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
        try{
            switch(Frame.Command){
                case ProtocolCommands.Hello:
                    HandleHello(Frame);
                    break;
                case ProtocolCommands.ConnectionSuccess:
                    HandleConnectionSuccess(Frame);
                    break;
                case ProtocolCommands.AuthenticationResult:
                    HandleAuthenticationResult(Frame);
                    break;
                case ProtocolCommands.SendProducts:
                    HandleSendProducts(Frame);
                    break;
                case ProtocolCommands.OrderResult:
                    HandleOrderResult(Frame);
                    break;
                case ProtocolCommands.ChatBroadcast:
                    HandleChatBroadcast(Frame);
                    break;
                case ProtocolCommands.Error:
                    HandleError(Frame);
                    break;
                default:
                    // needs to return a bad command to the server
                    break;
        }
        }
        catch
        {
            
        }
    }
    public void HandleHello(ProtocolFrame Frame)
    {
        HelloPayload? Payload =JsonSerializer.Deserialize<HelloPayload>(Frame.Payload);
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
        if(Payload!.Success)
        {
            PrintOut.Invoke("Connected");
            TcpConnected = true;
        }
        else
        {
            PrintOut.Invoke("Failed to connect, try again");
        }
    }
    public void HandleAuthenticationResult(ProtocolFrame Frame)
    {
        byte[] DecryptedPayload = SecurityHelpers.DecryptWithSessionKey(Frame.Payload, aes!);
        AuthenticationResultPayload? Payload = JsonSerializer.Deserialize<AuthenticationResultPayload>(DecryptedPayload);
        if(Payload!.Success)
        {
            Username = Payload.Username;
            PrintOut.Invoke("Authenticated as " + Payload.Username);
            Products = Payload.Products;
            Messages = Payload.Messages;
        }
        else
        {
            PrintOut.Invoke("Authentication failed");
        }
    }
    public void HandleSendProducts(ProtocolFrame Frame)
    {
        byte[] DecryptedPayload = SecurityHelpers.DecryptWithSessionKey(Frame.Payload, aes!);
        SendProductsPayload? Payload = JsonSerializer.Deserialize<SendProductsPayload>(DecryptedPayload);
        //Refresh screen function
    }
    public void HandleOrderResult(ProtocolFrame Frame)
    {
        byte[] DecryptedPayload = SecurityHelpers.DecryptWithSessionKey(Frame.Payload, aes!);
        OrderResultPayload? Payload = JsonSerializer.Deserialize<OrderResultPayload>(DecryptedPayload);
        if(Payload!.Success)
        {
            PrintOut.Invoke("Order successful");//maybe add to order list
        }
        else
        {
            PrintOut.Invoke("Order failed ");
        }
    }
    public void HandleChatBroadcast(ProtocolFrame Frame)
    {
        byte[] DecryptedPayload = SecurityHelpers.DecryptWithSessionKey(Frame.Payload, aes!);
        ChatBroadcastPayload? Payload = JsonSerializer.Deserialize<ChatBroadcastPayload>(DecryptedPayload);
        Messages.Add(Payload!.Message);
    }
    public void HandleError(ProtocolFrame Frame)
    {
        byte[] DecryptedPayload = SecurityHelpers.DecryptWithSessionKey(Frame.Payload, aes!);
        ErrorPayload? Payload = JsonSerializer.Deserialize<ErrorPayload>(DecryptedPayload);
        PrintOut.Invoke("Error from server: " + Payload!.ErrorMessage);
    }
    public event Action<string>? PrintOut;
    public void RefreshScreen()
    {
        //displays new info, idk if needed
    }
    public void CreateRegisterFrame(string Username, string Password)
    {
        if(TcpConnected == false)
        {
            PrintOut.Invoke("Not connected to server");
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
        if(TcpConnected == false)
        {
            PrintOut.Invoke("Not connected to server");
            return;
        }
        LoginPayload Payload = new LoginPayload();
        Payload.Username = Username;
        Payload.Password = Password;
        byte[] PayloadToSend = JsonSerializer.SerializeToUtf8Bytes(Payload);
        Send(new ProtocolFrame(ProtocolCommands.Login, ProtocolEncryptionFlags.UnEncrypted, NextRequestId++, PayloadToSend), true);
    }

}
