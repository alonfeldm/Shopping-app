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
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            try
            {
                int portTry = int.Parse(PortTextBox.Text);
            }
            catch (FormatException)
            {
                MessageBox.Show("Please enter a valid port number.");
            }
            int port = int.Parse(PortTextBox.Text);
            if (port < 1 || port > 65535)
            {
                MessageBox.Show("Please enter a valid port number (1-65535).");
                return;
            }
            try
            {
                Server.Start(port);
                isRunning = true;
            }
            catch
            {
                MessageBox.Show("Something went wrong while starting the server");
                return;
            }
        }
        private void StopButton_Click(object sender, EventArgs e)
        {
            try
            {
                int portTry = int.Parse(PortTextBox.Text);
            }
            catch (FormatException)
            {
                MessageBox.Show("Please enter a valid port number.");
            }
            int port = int.Parse(PortTextBox.Text);
            if (port < 1 || port > 65535)
            {
                MessageBox.Show("Please enter a valid port number (1-65535).");
                return;
            }
            if (isRunning)
            {
                try
                {
                    Server.Stop();
                    isRunning = false;
                }
                catch
                {
                    MessageBox.Show("Something went wrong while stopping the server");
                    return;
                }
            }
            else
            {
                MessageBox.Show("The server is not running.");
                return;
            }
        }

    }
}
