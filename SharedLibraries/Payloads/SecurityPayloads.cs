using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace SharedLibraries.Payloads;

public class User
{
    public string Username { get; set;} = string.Empty;//the username
    public string PasswordHash { get; set;} = string.Empty;// doesnt hold the plain password for security reasons
    public string Salt { get; set;} = string.Empty;//used for testing if a new password matches the old one
}
public sealed partial class HelloPayload
{
    public string ServerName { get; set; } = "Shop local(ly)";//might be used to differentiate between multiple hosts
    public byte[] PublicKey { get; set; } = Array.Empty<byte>();// needed to start safe symetric encryption between the client and server
}

public sealed partial class SecureSessionPayload
{
    public SecureSessionPayload()// one consturctor for an empty aes
    {
        AesKey = Array.Empty<byte>();
        AesIV = Array.Empty<byte>();
    }
    public SecureSessionPayload(Aes aes)// another one with automatically filled values for convenience
    {
        AesKey = aes.Key;
        AesIV = aes.IV;
    }
    public byte[] AesKey{get; set;}
    public byte[] AesIV{get; set;}
    
}

public sealed partial class ConnectionSuccessPayload
{
    public bool Success { get; set; }//used to let the client know if it can register/login now
}

public sealed partial class RegisterPayload
{
    public string Username { get; set; } = string.Empty;//the basic fields needed for a register
    public string Password { get; set; } = string.Empty;
}

public sealed partial class LoginPayload
{
    public string Username { get; set; } = string.Empty;//the basic fields needed for a register
    public string Password { get; set; } = string.Empty;
}

public sealed partial class AuthenticationResultPayload
{
    public bool Success { get; set; }// to let the user know if the login/register was successful
    public string Message { get; set; } = string.Empty;// might get used to let the user have a more detailed error message
    public string Username { get; set; } = string.Empty;// might get used later
    public List<ProductWithDetails> Products { get; set; } = new List<ProductWithDetails>();// if it was successful then all messages and products are sent with the success message
    public List<Message> Messages { get; set; } = new List<Message>();
}
