using System;
using System.Net.Quic;
using System.Windows.Forms;

namespace Client;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        ApplicationConfiguration.Initialize();
        ServerWindow Window = new ServerWindow();
        Client Client = new Client();
        
        Application.Run(Window);
    }
}