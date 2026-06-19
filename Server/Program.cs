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
        Application.Run(Window);
    }
}

