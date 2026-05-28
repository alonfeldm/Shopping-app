using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Server
{
    public partial class ServerWindow : Form
    {
        Server Server = new Server();
        private bool isRunning { get; set; } = false;
        public ServerWindow()
        {
            InitializeComponent();
            Server.PrintOut += PrintMessage;
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            try
            {
                int portTry = int.Parse(PortTextBox.Text);
            }
            catch (FormatException)
            {
                PrintMessage("Please enter a valid port number.");
            }
            int port = int.Parse(PortTextBox.Text);
            if (port < 1 || port > 65535)
            {
                PrintMessage("Please enter a valid port number (1-65535).");
                return;
            }
            try
            {
                Server.Start(port);
                isRunning = true;
                PrintMessage("Server started successfully.");
            }
            catch
            {
                PrintMessage("Something went wrong while starting the server");
                return;
            }
        }
        private void StopButton_Click(object sender, EventArgs e)
        {
            if (isRunning)
            {
                try
                {
                    Server.Stop();
                    isRunning = false;
                    PrintMessage("Server stopped successfully.");
                }
                catch
                {
                    PrintMessage("Something went wrong while stopping the server");
                    return;
                }
            }
            else
            {
                PrintMessage("The server is not running.");
                return;
            }
        }
        private void PrintMessage(string message)
        {
            LogBox.AppendText(message + Environment.NewLine);
        }
    }
}
