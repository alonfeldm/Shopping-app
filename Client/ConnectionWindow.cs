using System;
using System.Windows.Forms;

namespace Client
{
    public partial class ConnectionWindow : Form
    {
        private Client Client { get; set; } = new Client();
        public ConnectionWindow()
        {
            InitializeComponent();
            Client.PrintOut += PrintMessage;
            Client.DisplayMessage += DisplayMessage;
        }

        private void connectButton_Click(object? sender, EventArgs e)
        {
            try
            {
                int portTest = int.Parse(portTextBox.Text.Trim());
            }
            catch
            {
                MessageBox.Show("Invalid port, try again.");
                return;
            }

            string IpText = ipTextBox.Text.Trim();
            if (string.IsNullOrEmpty(IpText) || IpText.Split('.').Length != 4)
            {
                MessageBox.Show("Invalid IP, try again.");
                return;
            }
            try
            {
                int Ip1Test = int.Parse(IpText.Split('.')[0]);
                int Ip2Test = int.Parse(IpText.Split('.')[1]);
                int Ip3Test = int.Parse(IpText.Split('.')[2]);
                int Ip4Test = int.Parse(IpText.Split('.')[3]);
            }
            catch
            {
                MessageBox.Show("Invalid IP, try again.");
                return;
            }
            int Ip1 = int.Parse(IpText.Split('.')[0]);
            int Ip2 = int.Parse(IpText.Split('.')[1]);
            int Ip3 = int.Parse(IpText.Split('.')[2]);
            int Ip4 = int.Parse(IpText.Split('.')[3]);
            int port = int.Parse(portTextBox.Text.Trim());
            if (port < 0 || port > 65535 || Ip1 < 0 || Ip1 > 255 || Ip2 < 0 || Ip2 > 255 || Ip3 < 0 || Ip3 > 255 || Ip4 < 0 || Ip4 > 255)
            {
                MessageBox.Show("Invalid port or IP, try again.");
                return;
            }
            // checked port and IP, now can start to connect
            try
            {
                Client.Start(IpText, port);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to connect: {ex.Message}");
                return;
            }
        }
        private void PrintMessage(string message)
        {
            MessageBox.Show(message);
        }

        private void registerButton_Click(object? sender, EventArgs e)
        {
            if (!Client.SecureSessionConnected)
            {
                MessageBox.Show("Please connect to the server first.");
                return;
            }
            string username = usernameTextBox.Text.Trim();
            string password = passwordTextBox.Text.Trim();
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Username and password cannot be empty.");
                return;
            }
            try
            {
                Client.CreateRegisterFrame(username, password);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to register: {ex.Message}");
            }
        }

        private void loginButton_Click(object? sender, EventArgs e)
        {
            if (!Client.SecureSessionConnected)
            {
                MessageBox.Show("Please connect to the server first.");
                return;
            }
            string username = usernameTextBox.Text.Trim();
            string password = passwordTextBox.Text.Trim();
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Username and password cannot be empty.");
                return;
            }
            try
            {
                Client.CreateLoginFrame(username, password);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to login: {ex.Message}");
            }
        }
        private void DisconnectButton_Click(object? sender, EventArgs e)
        {
            if (!Client.TcpConnected)
            {
                MessageBox.Show("Not connected to the server.");
                return;
            }
            Client.Stop();
        }
        private void SendButton_Click(object? sender, EventArgs e)
        {
            if (!Client.TcpConnected)
            {
                MessageBox.Show("Not connected to the server.");
                return;
            }
            if (!Client.SecureSessionConnected)
            {
                MessageBox.Show("Please login or register first.");
                return;
            }
            if (string.IsNullOrEmpty(MessageTextBox.Text)){
                MessageBox.Show("Message cannot be empty.");
                return;
            }
            try
            {
                Client.CreateMessageFrame(MessageTextBox.Text.Trim());

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to send message: {ex.Message}");
            }
    }
        public void DisplayMessage(string username, string message)
        {
            ChatBox.AppendText($"{username}: {message}" + Environment.NewLine);
        }
}
}
