using System;
using SharedLibraries;
using System.Net.Sockets;
using System.Threading;

namespace Server;

internal class ClientSession : IDisposable
{
    private readonly TcpClient tcpClient;
    private readonly NetworkStream networkStream;
    private Thread? ReceiveThread;
    public bool DisconnectRaised = false;
    private readonly object SendLock = new object();
    private bool Disposed;
    public event Action<ClientSession, ProtocolFrame>? FrameReceived;//change name
    public event Action<ClientSession>? Disconnected;
    public int ClientId {get;}
    public string? Username {get; set;}
    public bool Authenticated {get; set;}
    public System.Security.Cryptography.Aes? aes {get; set;}
    public string RemoteEndpoint => tcpClient.Client.RemoteEndPoint?.ToString()??"Not known";
    public int RequestCounter {get; set;} = 0;

    public ClientSession(TcpClient tcpClient, int CliendId)
    {
        this.ClientId = CliendId;
        this.tcpClient = tcpClient;
        this.tcpClient.NoDelay = true;
        networkStream = tcpClient.GetStream();
    }
    public void Start()
    {
        aes = SecurityHelpers.CreateAes();
        ReceiveThread = new Thread(Receive)
        {
            IsBackground = true,
            Name = $"{ClientId}"
        };
        ReceiveThread.Start();
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
            Protocol.WriteFrame(networkStream, new ProtocolFrame(Frame.Command, Flag, RequestCounter++, PayloadToSend));
            networkStream.Flush();
        }
    }

    private void Receive()
    {
        try
        {
            while (true)
            {
                ProtocolFrame? ReadFrame = Protocol.ReadFrame(networkStream);
                if(ReadFrame == null)
                {
                    break;
                }
                byte[] SentPayload = ReadFrame.Payload;
                if(ReadFrame.Flags == ProtocolEncryptionFlags.Encrypted)
                {
                    if (aes != null)
                    {
                        SentPayload = SecurityHelpers.DecryptWithSessionKey(SentPayload, aes);
                    }
                    else
                    {
                        throw new InvalidOperationException("no aes key for decryption");
                    }
                }
                ProtocolFrame NewFrame = new ProtocolFrame(ReadFrame.Command, ProtocolEncryptionFlags.UnEncrypted, RequestCounter++, SentPayload);
                FrameReceived?.Invoke(this, NewFrame);

            }
        }
        catch (Exception)
        {
            PrintOut?.Invoke("Error receiving data from client: " + (Username ?? RemoteEndpoint));
        }
        Dispose();
    }
    public void Dispose()
    {   
        if (Disposed)
        {
            return;
        }
        Disposed = true;
        networkStream.Close();
        tcpClient.Close();
        Disconnected?.Invoke(this);
        PrintOut?.Invoke("Client disconnected: " + (Username ?? RemoteEndpoint));
    }
    public event Action<string>? PrintOut;
}