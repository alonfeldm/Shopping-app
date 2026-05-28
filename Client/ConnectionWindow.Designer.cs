using System.Windows.Forms;

namespace Client
{
    partial class ConnectionWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            connectButton = new Button();
            ipLabel = new Label();
            ipTextBox = new TextBox();
            portTextBox = new TextBox();
            portLabel = new Label();
            registerButton = new Button();
            loginButton = new Button();
            passwordTextBox = new TextBox();
            usernameTextBox = new TextBox();
            usernameLabel = new Label();
            passwordLabel = new Label();
            disconnectButton = new Button();
            ChatBox = new TextBox();
            ChatLabel = new Label();
            MessageTextBox = new TextBox();
            SendButton = new Button();
            SuspendLayout();
            // 
            // connectButton
            // 
            connectButton.Location = new System.Drawing.Point(45, 12);
            connectButton.Name = "connectButton";
            connectButton.Size = new System.Drawing.Size(100, 32);
            connectButton.TabIndex = 0;
            connectButton.Text = "Connect";
            connectButton.UseVisualStyleBackColor = true;
            connectButton.Click += connectButton_Click;
            // 
            // ipLabel
            // 
            ipLabel.AutoSize = true;
            ipLabel.Location = new System.Drawing.Point(227, 12);
            ipLabel.Name = "ipLabel";
            ipLabel.Size = new System.Drawing.Size(21, 20);
            ipLabel.TabIndex = 1;
            ipLabel.Text = "IP";
            // 
            // ipTextBox
            // 
            ipTextBox.Location = new System.Drawing.Point(160, 50);
            ipTextBox.Name = "ipTextBox";
            ipTextBox.Size = new System.Drawing.Size(150, 27);
            ipTextBox.TabIndex = 2;
            // 
            // portTextBox
            // 
            portTextBox.Location = new System.Drawing.Point(325, 50);
            portTextBox.Name = "portTextBox";
            portTextBox.Size = new System.Drawing.Size(150, 27);
            portTextBox.TabIndex = 3;
            // 
            // portLabel
            // 
            portLabel.AutoSize = true;
            portLabel.Location = new System.Drawing.Point(379, 9);
            portLabel.Name = "portLabel";
            portLabel.Size = new System.Drawing.Size(35, 20);
            portLabel.TabIndex = 4;
            portLabel.Text = "Port";
            // 
            // registerButton
            // 
            registerButton.Location = new System.Drawing.Point(486, 12);
            registerButton.Name = "registerButton";
            registerButton.Size = new System.Drawing.Size(100, 32);
            registerButton.TabIndex = 5;
            registerButton.Text = "Register";
            registerButton.UseVisualStyleBackColor = true;
            registerButton.Click += registerButton_Click;
            // 
            // loginButton
            // 
            loginButton.Location = new System.Drawing.Point(486, 50);
            loginButton.Name = "loginButton";
            loginButton.Size = new System.Drawing.Size(100, 32);
            loginButton.TabIndex = 6;
            loginButton.Text = "Login";
            loginButton.UseVisualStyleBackColor = true;
            loginButton.Click += loginButton_Click;
            // 
            // passwordTextBox
            // 
            passwordTextBox.Location = new System.Drawing.Point(764, 50);
            passwordTextBox.Name = "passwordTextBox";
            passwordTextBox.PasswordChar = '*';
            passwordTextBox.Size = new System.Drawing.Size(150, 27);
            passwordTextBox.TabIndex = 7;
            // 
            // usernameTextBox
            // 
            usernameTextBox.Location = new System.Drawing.Point(592, 50);
            usernameTextBox.Name = "usernameTextBox";
            usernameTextBox.Size = new System.Drawing.Size(150, 27);
            usernameTextBox.TabIndex = 8;
            // 
            // usernameLabel
            // 
            usernameLabel.AutoSize = true;
            usernameLabel.Location = new System.Drawing.Point(627, 12);
            usernameLabel.Name = "usernameLabel";
            usernameLabel.Size = new System.Drawing.Size(75, 20);
            usernameLabel.TabIndex = 9;
            usernameLabel.Text = "Username";
            // 
            // passwordLabel
            // 
            passwordLabel.AutoSize = true;
            passwordLabel.Location = new System.Drawing.Point(803, 12);
            passwordLabel.Name = "passwordLabel";
            passwordLabel.Size = new System.Drawing.Size(70, 20);
            passwordLabel.TabIndex = 10;
            passwordLabel.Text = "Password";
            // 
            // disconnectButton
            // 
            disconnectButton.Location = new System.Drawing.Point(45, 50);
            disconnectButton.Name = "disconnectButton";
            disconnectButton.Size = new System.Drawing.Size(100, 32);
            disconnectButton.TabIndex = 11;
            disconnectButton.Text = "Disconnect";
            disconnectButton.UseVisualStyleBackColor = true;
            disconnectButton.Click += DisconnectButton_Click;
            // 
            // ChatBox
            // 
            ChatBox.Location = new System.Drawing.Point(1200, 50);
            ChatBox.Multiline = true;
            ChatBox.Name = "ChatBox";
            ChatBox.ReadOnly = true;
            ChatBox.ScrollBars = ScrollBars.Vertical;
            ChatBox.Size = new System.Drawing.Size(350, 650);
            ChatBox.TabIndex = 12;
            // 
            // ChatLabel
            // 
            ChatLabel.AutoSize = true;
            ChatLabel.Location = new System.Drawing.Point(1350, 15);
            ChatLabel.Name = "ChatLabel";
            ChatLabel.Size = new System.Drawing.Size(39, 20);
            ChatLabel.TabIndex = 13;
            ChatLabel.Text = "Chat";
            // 
            // MessageTextBox
            // 
            MessageTextBox.Location = new System.Drawing.Point(1200, 715);
            MessageTextBox.Name = "MessageTextBox";
            MessageTextBox.Size = new System.Drawing.Size(300, 30);
            MessageTextBox.TabIndex = 14;
            // 
            // SendButton
            // 
            SendButton.Location = new System.Drawing.Point(1500, 715);
            SendButton.Name = "SendButton";
            SendButton.Size = new System.Drawing.Size(50, 30);
            SendButton.TabIndex = 15;
            SendButton.Text = "Send";
            SendButton.UseVisualStyleBackColor = true;
            // 
            // ConnectionWindow
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1600, 800);
            Controls.Add(SendButton);
            Controls.Add(MessageTextBox);
            Controls.Add(ChatLabel);
            Controls.Add(ChatBox);
            Controls.Add(disconnectButton);
            Controls.Add(passwordLabel);
            Controls.Add(usernameLabel);
            Controls.Add(usernameTextBox);
            Controls.Add(passwordTextBox);
            Controls.Add(loginButton);
            Controls.Add(registerButton);
            Controls.Add(portLabel);
            Controls.Add(portTextBox);
            Controls.Add(ipTextBox);
            Controls.Add(ipLabel);
            Controls.Add(connectButton);
            Name = "ConnectionWindow";
            Text = "Connect And Authenticate";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.Label ipLabel;
        private System.Windows.Forms.TextBox ipTextBox;
        private System.Windows.Forms.TextBox portTextBox;
        private System.Windows.Forms.Label portLabel;
        private System.Windows.Forms.Button registerButton;
        private System.Windows.Forms.Button loginButton;
        private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.TextBox usernameTextBox;
        private System.Windows.Forms.Label usernameLabel;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.Button disconnectButton;
        private System.Windows.Forms.TextBox ChatBox;
        private System.Windows.Forms.Label ChatLabel;
        private System.Windows.Forms.TextBox MessageTextBox;
        private System.Windows.Forms.Button SendButton;
    }
}
