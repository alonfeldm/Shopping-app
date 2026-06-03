using System;
using System.IO;
using System.Buffers.Binary;

namespace SharedLibraries;

[Flags]
public enum ProtocolEncryptionFlags : byte
{
    UnEncrypted = 0,
    Encrypted = 1,
}
public enum ProtocolCommands : ushort
{
    Hello = 1,
    SecureSession = 2,
    ConnectionSuccess = 3,
    Register = 4,
    Login = 5,
    AuthenticationResult = 6,
    GetProducts = 7,
    SendProducts = 8,
    PlaceOrder = 9,
    OrderResult = 10,
    ChatPost = 11,
    ChatBroadcast = 12,
    Error = 13,
    Heartbeat = 14,
    Disconnect = 15,

}
public sealed class ProtocolFrame
{
    public ProtocolCommands Command {get;}
    public ProtocolEncryptionFlags Flags {get;}
    public int RequestId {get;}
    public byte[] Payload {get;}

    public ProtocolFrame(ProtocolCommands Command, ProtocolEncryptionFlags Flags, int RequestId, byte[] Payload)
    {
        this.Command = Command;
        this.Flags = Flags;
        this.RequestId = RequestId;
        this.Payload = Payload ?? Array.Empty<byte>();
    }
}
public static class Protocol
{
    private static readonly byte[] HeaderBuffer = new byte[] { (byte)'I', (byte)'N', (byte)'S', (byte)'P' };
    public const int HeaderSize = 16;
    public const byte ProtocolVersion = 1;
    public const int MaxPayloadSize = 64 * 1024;

public static byte[]? ReadStream(Stream stream, int Length)
    {
        byte[] Buffer = new byte[Length];// holds the read data
        int ReadCnt = 0;// bytes read 
        while(ReadCnt < Length)
        {
            int Read = stream.Read(Buffer, ReadCnt, Length-ReadCnt);
            if (Read == 0)
            {
                if(ReadCnt == 0)
                {
                    return null; // stream closed and didnt read anything
                }
                throw new EndOfStreamException("Unexpected end of stream");// stream closed in the middle of reading
            }
            ReadCnt += Read;
        }
    return Buffer;
    }
public static ProtocolFrame? ReadFrame(Stream stream)
    {
        byte[]? Header = ReadStream(stream, HeaderSize);//beacause it can pickup nothing, should be nullable
        if (Header == null)
        {
            return null;
        // if there is nothing return null, beacuse its not a valid frame
        }
        for (int i = 0; i < HeaderBuffer.Length; i++)
        {
            if(Header[i] != HeaderBuffer[i])
            {
                throw new InvalidDataException("Unexpected protocol header");
            }
        }
        if (Header[4] != ProtocolVersion)
        {
            throw new InvalidDataException($"Unsupported protocol version");
        }
        ProtocolEncryptionFlags Flags = (ProtocolEncryptionFlags)Header[5];
        ProtocolCommands Command = (ProtocolCommands)BinaryPrimitives.ReadUInt16LittleEndian(Header.AsSpan(6, 2));
        //int 16 beacause 2 bytes is 16 bits, reads from byte 6 and 7, casts to protocolCommand
        int RequestId = BinaryPrimitives.ReadInt32LittleEndian(Header.AsSpan(8, 4));
        // int 32 beacause 4 bytes is 32 bits, reads from byte 8 to 11
        int PayloadLength = BinaryPrimitives.ReadInt32LittleEndian(Header.AsSpan(12, 4));
        // int 32 beacause 4 bytes is 32 bits, reads from byte 12 to 15
        if (PayloadLength < 0 || PayloadLength > MaxPayloadSize)
        {
            throw new InvalidDataException($"Invalid payload length");
        }
        byte[] Payload;
        if( PayloadLength == 0)
        {
            Payload = Array.Empty<byte>();
        }
        else
        {
            Payload = ReadStream(stream, PayloadLength) ?? throw new InvalidDataException("Unexpected end of stream while reading payload");
        }
        return new ProtocolFrame(Command, Flags, RequestId, Payload);
    }
public static void WriteFrame(Stream stream, ProtocolFrame Frame)
    {
        byte[] Payload = Frame.Payload ?? Array.Empty<byte>();// if the payload is null, use an empty byte array
        if(Payload.Length > MaxPayloadSize)
        {
            throw new InvalidOperationException("Payload size too big for the protocol");//checks if the payload is too big
        }
        byte[] Header = new byte[HeaderSize];
        HeaderBuffer.CopyTo(Header, 0);
        Header[4] = ProtocolVersion;
        Header[5] = (byte)Frame.Flags;
        BinaryPrimitives.WriteUInt16LittleEndian(Header.AsSpan(6,2), (ushort)Frame.Command);
        BinaryPrimitives.WriteInt32LittleEndian(Header.AsSpan(8,4), Frame.RequestId);
        BinaryPrimitives.WriteInt32LittleEndian(Header.AsSpan(12,4), Payload.Length);
        stream.Write(Header, 0, Header.Length);
        if (Payload.Length > 0)
        {
            stream.Write(Payload, 0, Payload.Length);
        }
    }

}