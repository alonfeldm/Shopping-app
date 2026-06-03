using System;// used for basic functions
using System.Security.Cryptography;// used for rsa and aes encryption
using System.Net.Sockets;// used for tcp
using System.Threading;// used for threads
using SharedLibraries;// used for the protocol, protocol helpers and security helpers
using SharedLibraries.Payloads;// used for the protocol payloads
using System.Text.Json;// used for using json and json helpers
using System.Collections.Generic;// used for lists

namespace Client;

public class Client
{
    private Aes? Aes { get; set; }// the aes key for symmetric secure session encryption
    public bool SecureSessionConnected { get; set; } = false;// to know if the secure session handshake if complete
    public string? Username { get; set; }
    private readonly object sendLock = new object();//to stop multiple sendings at once
    private NetworkStream? NetworkStream { get; set; }
    private TcpClient? TcpClient { get; set; }
    private Thread? ListenThread { get; set; }
    private int NextRequestId { get; set; } = 0;
    private bool Disposed { get; set; } = false;
    public List<ProductWithDetails> Products { get; private set; } = new List<ProductWithDetails>();//easy storing for the products
    private List<Message> Messages { get; set; } = new List<Message>();//easy storing for the chat messages
    public bool TcpConnected { get; set; } = false;//to know if the tcp handshake is complete
    public bool LoggedIn { get; set; } = false;// to know if the user is logged in
    public void Start(string Ip, int Port)
    {
        if (!TcpConnected)
        {
            SecureSessionConnected = false;// makes sure that other functions know that the client isnt fully connected
            TcpClient = new TcpClient(Ip, Port);// to facilitate the tcp connection
            TcpClient!.NoDelay = true;
            NetworkStream = TcpClient.GetStream();// to get the network stream for sending and receiving data
            ListenThread = new Thread(Listen);// to start the listen thread that will handle receiving frames
            { ListenThread.IsBackground = true; }// to make it close when the main thread closes
            ListenThread.Start();// to start listening
            Disposed = false;// makes sure that functions know that the client is not disposed
            TcpConnected = true;// now the client is tcp connected but not secure session connected, that is later
            ClearStore?.Invoke();// clears the store to make sure there are no duplicates
            ClearMessages?.Invoke();// clears the messages to make sure there are no duplicates
            ClearLogs?.Invoke();// clears the logs to make sure there are no duplicates
        }
    }

    public void Stop()
    {
        try// trying to disconnect, if something fails the client wont crash
        {
            Disposed = true;// now functions know that the client is disposed
            NetworkStream?.Close();// closing the network stream
            TcpClient?.Close();// closing the tcp client
            SecureSessionConnected = false;// now the client is not secure session connected
            Aes = null; // wipes the aes, now a new connection will have a new aes
            Username = null; // now a new user can login
            TcpConnected = false;// now the client is not tcp connected
        }
        catch (Exception ex)// if failed to disconnect properly, log it but dont crash
        {
            DisplayLog?.Invoke("Failed to handle disconnect frame: " + ex.Message);
        }
    }
    public void Listen()// listening for frames in a loop 
    {
        if (TcpConnected)// if the client is tcp connected, then it can listen for frames, otherwise it cant
        {
            try
            {
                while (!Disposed)// while the client is not disposed, keep listening for frames, if it is disposed then a user pressed disconnect and listening will stop
                {
                    ProtocolFrame? Frame = Protocol.ReadFrame(NetworkStream!);
                    if (Frame == null)
                    {
                        DisplayLog?.Invoke("Frame couldn't be read, connection might be lost");
                        break;
                    }
                    HandleFrame(Frame);// if a frame was read move it to the handler which will sort it 
                }
            }
            catch (Exception ex)// if it failed log it but dont crash
            {
                DisplayLog?.Invoke("Frame couldn't be read, connection might be lost: " + ex.Message);
            }
        }
    }
    public void Send(ProtocolFrame Frame, bool Encrypted)// the functions for sending frames
    {
        byte[] PayloadToSend = Frame.Payload; // the payload
        ProtocolEncryptionFlags Flag = ProtocolEncryptionFlags.UnEncrypted;// currently unencrypted, it might change
        if (Encrypted)// if the frame needs to be encrypted
        {
            if (Aes == null)// if there is no aes key, then it cant be encrypted
            {
                throw new InvalidOperationException("doesn't have an aes key");
            }
            PayloadToSend = SecurityHelpers.EncryptWithSessionKey(PayloadToSend, Aes);// encrypting with the aes key
            Flag = ProtocolEncryptionFlags.Encrypted;// change the flag to reflect the change

        }

        lock (sendLock)// to stop multiple sendings at once
        {
            if (Disposed)// if the client is disposed, then it cant send frames
            {
                return;
            }
            Protocol.WriteFrame(NetworkStream!, new ProtocolFrame(Frame.Command, Flag, NextRequestId++, PayloadToSend));// sends the frame
            NetworkStream!.Flush();// flushes the stream to make sure the frame is sent
        }
    }
    public void HandleFrame(ProtocolFrame Frame)// handles the frame based on the command
    {
        try// if something fails it doesnt crash
        {
            switch (Frame.Command)
            {
                case ProtocolCommands.Hello:
                    try
                    {
                        HandleHello(Frame);// using handler functions to maintain organization
                    }
                    catch (Exception ex)
                    {
                        DisplayLog?.Invoke("Failed to handle hello frame: " + ex.Message);// basic error log message
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
                default:// if the command isnt one of the ones i want to handle then i log it
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
        RSA rsa = RSA.Create();// creates the rsa object to import the public key to
        rsa.ImportRSAPublicKey(Payload!.PublicKey, out _);
        Aes = SecurityHelpers.CreateAes();// create the aes key for the secure session
        SecureSessionPayload secureSessionPayload = new SecureSessionPayload(Aes);// creates the payload
        byte[] PayloadToSend = JsonSerializer.SerializeToUtf8Bytes(secureSessionPayload);
        PayloadToSend = SecurityHelpers.EncryptWithPublicKey(PayloadToSend, rsa);
        ProtocolFrame ResponseFrame = new ProtocolFrame(ProtocolCommands.SecureSession, ProtocolEncryptionFlags.UnEncrypted, NextRequestId++, PayloadToSend);
        Send(ResponseFrame, false);
        // encrypts the secure session payload with the rsa public key but doesnt encrypt with aes as the server doesnt have it yet
    }
    public void HandleConnectionSuccess(ProtocolFrame Frame)
    {
        byte[] DecryptedPayload = SecurityHelpers.DecryptWithSessionKey(Frame.Payload, Aes!);// decrypts with the aes key that the server uses now
        ConnectionSuccessPayload? Payload = JsonSerializer.Deserialize<ConnectionSuccessPayload>(DecryptedPayload);
        if (Payload!.Success)// if the connection was successful
        {
            DisplayLog?.Invoke("Connected");
            SecureSessionConnected = true;// to knnow that the secure session handshake is complete
        }
        else
        {
            DisplayLog?.Invoke("Failed to connect, try again");//if the payload was false then it the secure session failed
        }
    }
    public void HandleAuthenticationResult(ProtocolFrame Frame)
    {
        byte[] DecryptedPayload = SecurityHelpers.DecryptWithSessionKey(Frame.Payload, Aes!);
        AuthenticationResultPayload? Payload = JsonSerializer.Deserialize<AuthenticationResultPayload>(DecryptedPayload);
        if (Payload!.Success)
        {
            Username = Payload.Username;
            DisplayLog?.Invoke("Authenticated as " + Payload.Username);// tells the user they are logged in under that username
            LoggedIn = true; // to know that the user is logged in
            Products = Payload.Products;// gets the products
            Messages = Payload.Messages;// gets the messages
            ClearStore?.Invoke();// wipes the store to make sure there are no duplicates
            for (int i = 0; i < Products.Count; i++)
            {
                AppendStore?.Invoke(Products[i]);// loops to add the products
            }
            ClearMessages?.Invoke();// clears the messages to avoid duplicates
            for (int i = 0; i < Messages.Count; i++)
            {
                DisplayMessage?.Invoke(Messages[i].SentBy, Messages[i].Text);// loops to add the messages
            }
        }
        else
        {
            DisplayLog?.Invoke("Authentication failed");
            // if the payload was false then the authentication failed and there are no products or messages to display
        }
    }
    public void HandleSendProducts(ProtocolFrame Frame)
    // this is obsolete as the products are hardcoded into the server and cannot be changed without stopping the server
    {
        byte[] DecryptedPayload = SecurityHelpers.DecryptWithSessionKey(Frame.Payload, Aes!);
        SendProductsPayload? Payload = JsonSerializer.Deserialize<SendProductsPayload>(DecryptedPayload);
        Products = Payload!.Products;// gets the products
        ClearStore?.Invoke();//clears the store to avoid duplicates
        for (int i = 0; i < Products.Count; i++)
        {
            AppendStore?.Invoke(Products[i]);// loops to add the products
        }
    }
    public void HandleOrderResult(ProtocolFrame Frame)
    {
        byte[] DecryptedPayload = SecurityHelpers.DecryptWithSessionKey(Frame.Payload, Aes!);
        OrderResultPayload? Payload = JsonSerializer.Deserialize<OrderResultPayload>(DecryptedPayload);
        if (Payload!.Success)
        {
            DisplayLog?.Invoke("Order successful");
        }
        else
        {
            DisplayLog?.Invoke("Order failed ");
        }
    }
    public void HandleChatBroadcast(ProtocolFrame Frame)
    {
        byte[] DecryptedPayload = SecurityHelpers.DecryptWithSessionKey(Frame.Payload, Aes!);
        ChatBroadcastPayload? Payload = JsonSerializer.Deserialize<ChatBroadcastPayload>(DecryptedPayload);
        Messages.Add(Payload!.Message);// adds the message to the messages list
        DisplayMessage?.Invoke(Payload.Message.SentBy, Payload.Message.Text);// displays the message with the username of the sender

    }
    public void HandleError(ProtocolFrame Frame)
    {
        byte[] DecryptedPayload = SecurityHelpers.DecryptWithSessionKey(Frame.Payload, Aes!);
        ErrorPayload? Payload = JsonSerializer.Deserialize<ErrorPayload>(DecryptedPayload);
        DisplayLog?.Invoke("Error from server: " + Payload!.ErrorMessage);// logs the error message sent by the server
    }
    public event Action<string>? PrintOut;
    // used for messagebox.show, opens a small window with the error, less user friendly than the log but might be needed for important messages
    public event Action<string>? DisplayLog;
    //used for the log, more user friendly than printout but might not be noticed
    public void CreateRegisterFrame(string Username, string Password)
    {
        if (!SecureSessionConnected || !TcpConnected) // cant register if the connection isnt fully established
        {
            DisplayLog?.Invoke("Not connected to server");
            return;
        }
        RegisterPayload Payload = new RegisterPayload();
        Payload.Username = Username;
        Payload.Password = Password;
        byte[] PayloadToSend = JsonSerializer.SerializeToUtf8Bytes(Payload);
        ProtocolFrame FrameToSend = new ProtocolFrame(ProtocolCommands.Register, ProtocolEncryptionFlags.UnEncrypted, NextRequestId++, PayloadToSend);
        Send(FrameToSend, true);// send the frame with the username and password
    }
    public void CreateLoginFrame(string Username, string Password)
    {
        if (!SecureSessionConnected || !TcpConnected) // cant login if the connection isnt fully established
        {
            DisplayLog?.Invoke("Not connected to server");
            return;
        }
        LoginPayload Payload = new LoginPayload();
        Payload.Username = Username;
        Payload.Password = Password;
        byte[] PayloadToSend = JsonSerializer.SerializeToUtf8Bytes(Payload);
        ProtocolFrame FrameToSend = new ProtocolFrame(ProtocolCommands.Login, ProtocolEncryptionFlags.UnEncrypted, NextRequestId++, PayloadToSend);
        Send(FrameToSend, true);// send the frame with the username and password
    }
    public void CreateMessageFrame(string message)
    {
        ChatPostPayload Payload = new ChatPostPayload();
        Message Message = new Message();
        Message.Text = message;
        Message.SentBy = Username ?? "Unknown";// the username shouldnt be null but just in case, it will display as unknown
        Message.Timestamp = DateTime.Now;
        Message.MessageId = 0.ToString(); //the server will assign this a real id, this is just a placeholder
        Payload.Message = Message;
        byte[] PayloadToSend = JsonSerializer.SerializeToUtf8Bytes(Payload);
        ProtocolFrame FrameToSend = new ProtocolFrame(ProtocolCommands.ChatPost, ProtocolEncryptionFlags.UnEncrypted, NextRequestId++, PayloadToSend);
        Send(FrameToSend, true); // send the frame encrypted
    }
    public void CreateOrderFrame(OrderDetails details, List<ProductAndQuantity> cart)
    {
        PlaceOrderPayload Payload = new PlaceOrderPayload();
        Payload.Products = cart; // puts the cart items in the payload
        Payload.Details = details; // puts the order details in the payload
        byte[] PayloadToSend = JsonSerializer.SerializeToUtf8Bytes(Payload);
        ProtocolFrame FrameToSend = new ProtocolFrame(ProtocolCommands.PlaceOrder, ProtocolEncryptionFlags.UnEncrypted, NextRequestId++, PayloadToSend);
        Send(FrameToSend, true); // send the frame encrypted
    }
    public event Action<string, string>? DisplayMessage;// displays a message in the chat
    public event Action<ProductWithDetails>? AppendStore;//adds a product row to the store
    public event Action? ClearStore;//clears the store
    public event Action? ClearMessages;// clears the messages in the chat
    public event Action? ClearLogs;// clears the logs
}

