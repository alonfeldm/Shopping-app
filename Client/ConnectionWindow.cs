using System;
using System.Windows.Forms;

namespace Client
{
    public partial class ConnectionWindow : Form
    {
        private readonly Client client = new Client();

        public ConnectionWindow()
        {
            InitializeComponent();
            client.MessageRaised += Client_MessageRaised;
        }

        private void connectButton_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(portTextBox.Text, out int port))
            {
                MessageBox.Show("Port must be a valid number.", "Invalid Port", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                client.Start(ipTextBox.Text.Trim(), port);
                MessageBox.Show("Connection started.", "Client", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Connect Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void registerButton_Click(object? sender, EventArgs e)
        {
            client.CreateRegisterFrame(usernameTextBox.Text.Trim(), passwordTextBox.Text);
        }

        private void loginButton_Click(object? sender, EventArgs e)
        {
            client.CreateLoginFrame(usernameTextBox.Text.Trim(), passwordTextBox.Text);
        }

        private void Client_MessageRaised(string message)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(Client_MessageRaised), message);
                return;
            }

            outputTextBox.AppendText(message + Environment.NewLine);
        }
    }
}
