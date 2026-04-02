using System;
using System.Security.Cryptography;

namespace Client;

public class Client
{
    public Aes? aes {get; set;}
    public bool Connected {get; private set;} = false;
    public string? Username {get; set;}
    
}