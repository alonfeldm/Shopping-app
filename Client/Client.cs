using System;
using System.Security.Cryptography;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using SharedLibraries;
using SharedLibraries.Payloads;
using System.Text.Json;
using System.Configuration;

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

    public void Start(string Ip,int Port)
    {
        if(!Connected)
            TcpClient = new TcpClient(Ip, Port);
            TcpClient!.NoDelay = true;
            Connected = true;
            NetworkStream = TcpClient.GetStream();
            ListenThread = new Thread(Listen);{ListenThread.IsBackground = true;}
            ListenThread.Start();
    }
    
    public void Stop()
    {
        Disposed = true;
        NetworkStream?.Close();
        TcpClient?.Close();
    }
    public void Listen()
    {
        if(Connected){
            try
            {
                while (true)
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
        ProtocolFrame ResponseFrame = new ProtocolFrame(ProtocolCommands.SecureSession, ProtocolEncryptionFlags.Encrypted, NextRequestId++, PayloadToSend);
        Send(ResponseFrame, false);
    }
    public void HandleConnectionSuccess(ProtocolFrame Frame)
    {
        //needs to print out a message to the user
    }
    public void HandleAuthenticationResult(ProtocolFrame Frame)
    {
        //needs to print out a message to the user
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
        //prints out order success, or failure, if success maybe adds to list
    }
    public void HandleChatBroadcast(ProtocolFrame Frame)
    {
        byte[] DecryptedPayload = SecurityHelpers.DecryptWithSessionKey(Frame.Payload, aes!);
        ChatBroadcastPayload? Payload = JsonSerializer.Deserialize<ChatBroadcastPayload>(DecryptedPayload);
        //prints out the chat message to the user
    }
    public void HandleError(ProtocolFrame Frame)
    {
        byte[] DecryptedPayload = SecurityHelpers.DecryptWithSessionKey(Frame.Payload, aes!);
        ErrorPayload? Payload = JsonSerializer.Deserialize<ErrorPayload>(DecryptedPayload);
        //prints out the error message to the user
    }
    
}