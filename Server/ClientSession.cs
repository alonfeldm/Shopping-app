using System;
using SharedLibraries;
using System.Net.Sockets;
using System.Threading;

namespace Server;

internal class ClientSession : IDisposable// this represents one client connected to the server
{
    private readonly TcpClient tcpClient;// used to represent the tcpclient of the client
    private readonly NetworkStream networkStream;
    private Thread? ReceiveThread; 
    private readonly object SendLock = new object();// used to not allow multiple threads sending at once
    private bool Disposed;// if the client disposed
    public event Action<ClientSession, ProtocolFrame>? FrameReceived;//is called when a frame is received
    public event Action<ClientSession>? Disconnected; // is called when the client is disconnected
    public int ClientId { get; } // the clientID
    public string? Username { get; set; }// the clients username
    public bool Authenticated { get; set; } // used to know if the user was authenticated for logged in actions
    public System.Security.Cryptography.Aes? aes { get; set; }// the aes object used for encryption
    public string RemoteEndpoint => tcpClient.Client?.RemoteEndPoint?.ToString() ?? "Not known"; // the ip of the user
    public int RequestCounter { get; set; } = 0; // counter 
    public int loginCounter {get; set;} = 0;//used to know how many times the client has tried to login

    public ClientSession(TcpClient tcpClient, int CliendId)// creates a new client session object and sets fields
    {
        this.ClientId = CliendId;
        this.tcpClient = tcpClient;
        this.tcpClient.NoDelay = true;
        networkStream = tcpClient.GetStream();
    }
    public void Start()
    {
        aes = SecurityHelpers.CreateAes(); // initializes aes , will ignore the key and IV that were generated now
        ReceiveThread = new Thread(Receive)//creates and starts a thread to receive frames
        {
            IsBackground = true,
            Name = $"{ClientId}"
        };
        ReceiveThread.Start();
    }
    public void Send(ProtocolFrame Frame, bool Encrypted)// functions to send a frame with an option to encrypt
    {
        byte[] PayloadToSend = Frame.Payload;
        ProtocolEncryptionFlags Flag = ProtocolEncryptionFlags.UnEncrypted;//starts unecnrypted, will change if the bool encrypted is true
        if (Encrypted)
        {
            if (aes == null)//cant encrypt without an aes key
            {
                throw new InvalidOperationException("doesn't have an aes key");
            }
            PayloadToSend = SecurityHelpers.EncryptWithSessionKey(PayloadToSend, aes);
            Flag = ProtocolEncryptionFlags.Encrypted;//changes the flag to match

        }

        lock (SendLock)// locks so other threads wont send at once
        {
            if (Disposed)// shouldt try to send if the client was disposed already
            {
                return;
            }
            Protocol.WriteFrame(networkStream, new ProtocolFrame(Frame.Command, Flag, RequestCounter++, PayloadToSend));//sends tbe frame
            networkStream.Flush();// flushes for faster sending
        }
    }

    private void Receive()// receive function loop
    {
        try
        {
            while (true)//always listen unless break is used
            {
                ProtocolFrame? ReadFrame = Protocol.ReadFrame(networkStream);
                if (ReadFrame == null)// if the frame is null it means the stream is closed
                {
                    break;
                }
                byte[] SentPayload = ReadFrame.Payload;
                if (ReadFrame.Flags == ProtocolEncryptionFlags.Encrypted)
                {
                    if (aes != null)// if the frame is encrypted and the server has an aes key then it can decrypt
                    {
                        SentPayload = SecurityHelpers.DecryptWithSessionKey(SentPayload, aes);
                    }
                    else
                    {
                        throw new InvalidOperationException("no aes key for decryption");// if not then the server can decrypt and process the frame payload
                    }
                }
                ProtocolFrame NewFrame = new ProtocolFrame(ReadFrame.Command, ProtocolEncryptionFlags.UnEncrypted, RequestCounter++, SentPayload);
                FrameReceived?.Invoke(this, NewFrame);// if its not encrypted/was decrypted the frame is sent to handling

            }
        }
        catch (Exception)
        {
            PrintOut?.Invoke("Error receiving data from client: " + (Username ?? RemoteEndpoint));// if there was an error logs the username of the user and if the user wasnt logged in yet it logs his ip
        }
        Dispose();// if the loop is stopped the client can be disposed as the server wont do anything with the client anymore
    }
    public void Dispose()
    {
        if (Disposed)
        {
            return; // cant dispose if already disposed
        }
        string usernameOrEndpoint = Username ?? RemoteEndpoint;
        Disposed = true;//to know the client is disposed
        networkStream.Close();
        tcpClient.Close();
        Disconnected?.Invoke(this);// to let subscribed functions know the client was disconnected
        PrintOut?.Invoke("Client disconnected: " + (usernameOrEndpoint));// logs the username of the user who disconnected, if he wasnt logged in then his ip
    }
    public event Action<string>? PrintOut;// used to log 
}