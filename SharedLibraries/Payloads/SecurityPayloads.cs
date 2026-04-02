using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace SharedLibraries.Payloads;

public class User
{
    public string Username { get; set;} = string.Empty;
    public string PasswordHash { get; set;} = string.Empty;
    public string Salt { get; set;} = string.Empty;
}
public sealed partial class HelloPayload
{
    public string ServerName { get; set; } = "Shop local(ly)";
    public byte[] PublicKey { get; set; } = Array.Empty<byte>();
}

public sealed partial class SecureSessionPayload
{
    public SecureSessionPayload(Aes aes)
    {
        AesKey = aes.Key;
        AesIV = aes.IV;
    }
    public byte[] AesKey{get; set;}
    public byte[] AesIV{get; set;}
    
}

public sealed partial class ConnectionSuccessPayload
{
    public bool Success { get; set; }
}

public sealed partial class RegisterPayload
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public sealed partial class LoginPayload
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public sealed partial class AuthenticationResultPayload
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public List<ProductWithDetails> Products { get; set; } = new List<ProductWithDetails>();
    public List<Message> Messages { get; set; } = new List<Message>();
}
