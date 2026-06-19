using System;
using System.Windows.Forms;

namespace Client;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        ApplicationConfiguration.Initialize();
        ConnectionWindow Window = new ConnectionWindow();
        Application.Run(Window);
    }
}
