using System;
using System.Collections.Generic;
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
            InitializeComponent();// initializes all buttons so they work
            Server.PrintOut += PrintMessage;// connects an event with a function
            Server.ServerStateChanged += RefreshConnections;
            ConnectedClientsGrid.Rows.Clear();
            var IPv4 = Array.Find(Dns.GetHostAddresses(Dns.GetHostName()), x => x.AddressFamily == AddressFamily.InterNetwork);// gets the ip of the server inside the LAN
            IPLabel.Text = IPv4?.ToString() ?? "No IPv4 address found";// sets the iplabel to the ip address if its a string, if its null then to no address found
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            if (isRunning)// cant run the server if its already running
            {
                PrintMessage("The server is already running.");// log it
                return;
            }
            try
            {
                int portTry = int.Parse(PortTextBox.Text);// validate that the port is an int
            }
            catch (FormatException)
            {
                PrintMessage("Please enter a valid port number.");// log it
                return;
            }
            int port = int.Parse(PortTextBox.Text);
            if (port < 1 || port > 65535)// check if the port is in the valid range
            {
                PrintMessage("Please enter a valid port number (1-65535).");// log it
                return;
            }
            try
            {
                Server.Start(port);// try to start the server, contain error and log them but not crash
                isRunning = true;
                PrintMessage("Server started successfully.");// log it
            }
            catch
            {
                PrintMessage("Something went wrong while starting the server");// log it
                return;
            }
        }
        private void StopButton_Click(object sender, EventArgs e)
        {
            if (isRunning)
            {
                try
                {
                    Server.Stop();// try and stop it, contain the error so it doesnt go up and crash
                    isRunning = false;
                    PrintMessage("Server stopped successfully.");// log it
                }
                catch
                {
                    PrintMessage("Something went wrong while stopping the server");// log it
                    return;
                }
            }
            else
            {
                PrintMessage("The server is not running.");// cant stop a server thats already not running, log it
                return;
            }
        }
        private void ClearUsersButton_Click(object sender, EventArgs e)//used for resetting the servers users
        {
            try
            {
                Database.ClearUsers();// tries to clear the users with a premade function, if an error rises its caught
                PrintMessage("Users successfully cleared.");// log it
            }
            catch (Exception ex)
            {
                PrintMessage("Error clearing the users table:" + ex.Message);// log it
            }
        }
        private void ClearMessagesButton_Click(object sender, EventArgs e)//used for resetting the servers messages
        {
            try
            {
                Database.ClearMessages();// tries to clear the messages with a premade function, if an error rises its caught
                PrintMessage("Messages successfully cleared.");
            }
            catch (Exception ex)
            {
                PrintMessage("Error clearing the messages table:" + ex.Message);// log it
            }
        }
        private void ClearOrdersButton_Click(object sender, EventArgs e)//used for resetting the orders users
        {
            try
            {
                Database.ClearOrders();// tries to clear the orders with a premade function, if an error rises its caught
                PrintMessage("Orders successfully cleared.");
            }
            catch (Exception ex)
            {
                PrintMessage("Error clearing the orders table:" + ex.Message);// log it
            }
        }
        public void PrintMessage(string Message)// used for logging
        {
            if (InvokeRequired)// if the ui thread isnt the one calling it switch to it so theres only one thread doing ui and no work of multiple ones at once
            {
                Invoke(new Action<string>(PrintMessage), Message);
                return;
            }
            LogBox.AppendText(Message + Environment.NewLine);// log it
        }
        private void RefreshConnections(Dictionary<int, ClientSession> ConnectedClients)
        {
            if (InvokeRequired)// if the ui thread isnt the one calling it switch to it so theres only one thread doing ui and no work of multiple ones at once
            {
                Invoke(new Action<Dictionary<int, ClientSession>>(RefreshConnections), ConnectedClients);
                return;
            }
            ConnectedClientsGrid.Rows.Clear();// first clears the grid

            foreach (var ConnectedClient in ConnectedClients)//goes over each connected client
            {
                ClientSession clientSession = ConnectedClient.Value;
                if (string.IsNullOrEmpty(clientSession.Username))// if the username is null or empty then theres no username to display
                {
                    ConnectedClientsGrid.Rows.Add("Not logged in yet", clientSession.RemoteEndpoint);
                }
                else
                {
                    ConnectedClientsGrid.Rows.Add(clientSession.Username, clientSession.RemoteEndpoint);
                }
            }

        }
    }
}
