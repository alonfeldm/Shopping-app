using System;
using System.Net;
using System.Net.Sockets;
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
            var IPv4 = Array.Find(Dns.GetHostAddresses(Dns.GetHostName()), x => x.AddressFamily == AddressFamily.InterNetwork);
            IPLabel.Text = IPv4?.ToString() ?? "No IPv4 address found";
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            if (isRunning)
            {
                PrintMessage("The server is already running.");
                return;
            }
            try
            {
                int portTry = int.Parse(PortTextBox.Text);
            }
            catch (FormatException)
            {
                PrintMessage("Please enter a valid port number.");
                return;
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
        private void ClearUsersButton_Click(object sender, EventArgs e)
        {
            try
            {
                Database.ClearUsers();
                PrintMessage("Users successfully cleared.");
            }
            catch(Exception ex)
            {
                PrintMessage("Error clearing the users table:" + ex.Message);
            }
        }
        private void ClearMessagesButton_Click(object sender, EventArgs e)
        {
            try
            {
                Database.ClearMessages();
                PrintMessage("Messages successfully cleared.");
            }
            catch(Exception ex)
            {
                PrintMessage("Error clearing the messages table:" + ex.Message);
            }
        }
        private void ClearOrdersButton_Click(object sender, EventArgs e)
        {
            try
            {
                Database.ClearOrders();
                PrintMessage("Orders successfully cleared.");
            }
            catch (Exception ex)
            {
                PrintMessage("Error clearing the orders table:" + ex.Message);
            }
        }
        public void PrintMessage(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(PrintMessage), message);
                return;
            }
            LogBox.AppendText(message + Environment.NewLine);
        }
    }
}
