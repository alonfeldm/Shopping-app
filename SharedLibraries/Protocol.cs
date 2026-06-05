using System;
using System.IO;
using System.Buffers.Binary;

namespace SharedLibraries;

[Flags]
public enum ProtocolEncryptionFlags : byte // used to let the frame reader know if it needs to decrypt the payload to read it
{
    UnEncrypted = 0,
    Encrypted = 1,
}
public enum ProtocolCommands : ushort// used to let the frame reader know how to handle the frame
{
    Hello = 1,// for the hello payload
    SecureSession = 2,// for the SecureSession payload
    ConnectionSuccess = 3,// for the connectionSuccess payload
    Register = 4,// for the register payload
    Login = 5,// for the login payload
    AuthenticationResult = 6,// for the authenticationResult payload 
    GetProducts = 7,// for the getProducts payload
    SendProducts = 8,// for the sendProducts payload
    PlaceOrder = 9,// for the placeOrder payload
    OrderResult = 10,// for the orderResult payload
    ChatPost = 11,// for the chatPost payload
    ChatBroadcast = 12,// for the chatBroadcast payload
    Error = 13,// for the error payload
    Heartbeat = 14,// for the heartbeat payload
    Disconnect = 15,// for the disconnect payload

}
public sealed class ProtocolFrame
{
    public ProtocolCommands Command {get;}// the command field
    public ProtocolEncryptionFlags Flags {get;}// the encyrption flag field
    public int RequestId {get;}// request id that could be used to make sure frames are sent in order and not lost
    public byte[] Payload {get;}// holds the payloads

    public ProtocolFrame(ProtocolCommands Command, ProtocolEncryptionFlags Flags, int RequestId, byte[] Payload)// constructor of a frame
    {
        this.Command = Command;
        this.Flags = Flags;
        this.RequestId = RequestId;
        this.Payload = Payload ?? Array.Empty<byte>();// if the payload is null replaces it with a blank byte array
    }
}
public static class Protocol
{
    private static readonly byte[] HeaderBuffer = new byte[] { (byte)'I', (byte)'N', (byte)'S', (byte)'P' };
    // used when reading frames to make sure the frame is in the protocol, if it has INSP its the needed protocol
    public const int HeaderSize = 16;
    public const byte ProtocolVersion = 1;// if the protocol was updated it could be known by the version
    public const int MaxPayloadSize = 64 * 1024;

public static byte[]? ReadStream(Stream stream, int Length)
    {
        byte[] Buffer = new byte[Length];// will the read data in the specified length
        int ReadCnt = 0;// bytes read 
        while(ReadCnt < Length)// while there are bytes left to read
        {
            int Read = stream.Read(Buffer, ReadCnt, Length-ReadCnt);// read them to the buffer
            if (Read == 0)// nothing read because its empty
            {
                if(ReadCnt == 0)
                {
                    return null; // stream closed and didnt read anything
                }
                throw new EndOfStreamException("Unexpected end of stream");// stream closed in the middle of reading
            }
            ReadCnt += Read;// appends
        }
    return Buffer;// returns the read bytes
    }
public static ProtocolFrame? ReadFrame(Stream stream)
    {
        byte[]? Header = ReadStream(stream, HeaderSize);//reads the header size from the stream(16 bytes)
        if (Header == null)// if it doesnt have a header then its not in the needed protocol
        {
            return null;
        // if there is nothing return null beacuse iti s not a valid frame
        }
        for (int i = 0; i < HeaderBuffer.Length; i++)/// goes over all header bytes
        {
            if(Header[i] != HeaderBuffer[i])// if the header doesnt match the needed header its not meant to be used
            {
                throw new InvalidDataException("Unexpected protocol header");
            }
        }
        if (Header[4] != ProtocolVersion)// if the protocol version is different from the one in use right now its invalid
        {
            throw new InvalidDataException($"Unsupported protocol version");
        }
        ProtocolEncryptionFlags Flags = (ProtocolEncryptionFlags)Header[5];// gets the encryption flag
        ProtocolCommands Command = (ProtocolCommands)BinaryPrimitives.ReadUInt16LittleEndian(Header.AsSpan(6, 2));// gets the command
        int RequestId = BinaryPrimitives.ReadInt32LittleEndian(Header.AsSpan(8, 4));// gets the request id
        int PayloadLength = BinaryPrimitives.ReadInt32LittleEndian(Header.AsSpan(12, 4));// gets the payload length
        if (PayloadLength < 0 || PayloadLength > MaxPayloadSize)// cant have a payload with a negative or too high length
        {
            throw new InvalidDataException($"Invalid payload length");
        }
        byte[] Payload;
        if(PayloadLength == 0)// if the payload is empty set it to an empty array
        {
            Payload = Array.Empty<byte>();
        }
        else
        {
            Payload = ReadStream(stream, PayloadLength) ?? throw new InvalidDataException("Unexpected end of stream while reading payload");
            //read the payload from the stream, if its null then theres nothing to read and then throws the exception
        }
        return new ProtocolFrame(Command, Flags, RequestId, Payload);// returns the protocol frame with the read data
    }
public static void WriteFrame(Stream stream, ProtocolFrame Frame)
    {
        byte[] Payload = Frame.Payload ?? Array.Empty<byte>();// if the payload is null it sets it to an empty byte array
        if(Payload.Length > MaxPayloadSize)//checks if the payload is too big for the protocol
        {
            throw new InvalidOperationException("Payload size too big for the protocol");
        }
        byte[] Header = new byte[HeaderSize];// sets the header length to the default header size
        HeaderBuffer.CopyTo(Header, 0);// copies the basic "INSP" to the new header
        Header[4] = ProtocolVersion;// sets the version
        Header[5] = (byte)Frame.Flags;// sets the flag based on the frame given by the user
        BinaryPrimitives.WriteUInt16LittleEndian(Header.AsSpan(6,2), (ushort)Frame.Command);// gets the command based on the frame
        BinaryPrimitives.WriteInt32LittleEndian(Header.AsSpan(8,4), Frame.RequestId);// gets the request id based on the frame
        BinaryPrimitives.WriteInt32LittleEndian(Header.AsSpan(12,4), Payload.Length);// gets the payload length based on the frame
        stream.Write(Header, 0, Header.Length);// writes the header first
        if (Payload.Length > 0)// if there is a payload to write it writes it
        {
            stream.Write(Payload, 0, Payload.Length);
        }
    }

}