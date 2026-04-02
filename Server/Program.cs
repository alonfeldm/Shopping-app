using System;
using System.Windows.Forms; // for apps

namespace Server;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        ApplicationConfiguration.Initialize();
        ServerWindow Window = new ServerWindow();
        Server server = new Server();
        server.Start(1337);
        Application.Run(Window);
    }
}

