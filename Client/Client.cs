using System;
using System.Security.Cryptography;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using SharedLibraries;

namespace Client;

public class Client
{
    private Aes? aes {get; set;}
    private bool Connected {get; set;} = false;
    private string? Username {get; set;}
    private readonly object SendLock = new object();
    private NetworkStream? Stream {get; set;}
    private TcpClient? TcpClient {get; set;}
    private Thread? ListenThread {get; set;}

    public void Start(string Ip,int Port)
    {
        if(!Connected)
            TcpClient = new TcpClient(Ip, Port);
            TcpClient!.NoDelay = true;
            Connected = true;
            Stream = TcpClient.GetStream();
            ListenThread = new Thread(Listen);{ListenThread.IsBackground = true;}
            ListenThread.Start();
    }
    
    public void Stop()
    {
        
    }
    public void Listen()
    {
        try
        {
            while (true)
            {
                ProtocolFrame? Frame = Protocol.ReadFrame(Stream!);
                if(Frame == null)
                {
                    
                }
            }
        }
    }
    
}