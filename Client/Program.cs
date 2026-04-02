using System;
using System.Windows.Forms;

namespace Client;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        ApplicationConfiguration.Initialize();
        ClientWindow Window = new ClientWindows();
        Application.Run(Window);
    }
}