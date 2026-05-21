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
            connectButton = new System.Windows.Forms.Button();
            ipLabel = new System.Windows.Forms.Label();
            ipTextBox = new System.Windows.Forms.TextBox();
            portTextBox = new System.Windows.Forms.TextBox();
            portLabel = new System.Windows.Forms.Label();
            registerButton = new System.Windows.Forms.Button();
            loginButton = new System.Windows.Forms.Button();
            passwordTextBox = new System.Windows.Forms.TextBox();
            usernameTextBox = new System.Windows.Forms.TextBox();
            usernameLabel = new System.Windows.Forms.Label();
            passwordLabel = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // connectButton
            // 
            connectButton.Location = new System.Drawing.Point(45, 45);
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
            ipLabel.Location = new System.Drawing.Point(225, 27);
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
            portLabel.Location = new System.Drawing.Point(380, 27);
            portLabel.Name = "portLabel";
            portLabel.Size = new System.Drawing.Size(35, 20);
            portLabel.TabIndex = 4;
            portLabel.Text = "Port";
            // 
            // registerButton
            // 
            registerButton.Location = new System.Drawing.Point(45, 130);
            registerButton.Name = "registerButton";
            registerButton.Size = new System.Drawing.Size(100, 32);
            registerButton.TabIndex = 5;
            registerButton.Text = "Register";
            registerButton.UseVisualStyleBackColor = true;
            registerButton.Click += registerButton_Click;
            // 
            // loginButton
            // 
            loginButton.Location = new System.Drawing.Point(45, 180);
            loginButton.Name = "loginButton";
            loginButton.Size = new System.Drawing.Size(100, 32);
            loginButton.TabIndex = 6;
            loginButton.Text = "Login";
            loginButton.UseVisualStyleBackColor = true;
            loginButton.Click += loginButton_Click;
            // 
            // passwordTextBox
            // 
            passwordTextBox.Location = new System.Drawing.Point(325, 155);
            passwordTextBox.Name = "passwordTextBox";
            passwordTextBox.PasswordChar = '*';
            passwordTextBox.Size = new System.Drawing.Size(150, 27);
            passwordTextBox.TabIndex = 7;
            // 
            // usernameTextBox
            // 
            usernameTextBox.Location = new System.Drawing.Point(160, 155);
            usernameTextBox.Name = "usernameTextBox";
            usernameTextBox.Size = new System.Drawing.Size(150, 27);
            usernameTextBox.TabIndex = 8;
            // 
            // usernameLabel
            // 
            usernameLabel.AutoSize = true;
            usernameLabel.Location = new System.Drawing.Point(195, 114);
            usernameLabel.Name = "usernameLabel";
            usernameLabel.Size = new System.Drawing.Size(75, 20);
            usernameLabel.TabIndex = 9;
            usernameLabel.Text = "Username";
            // 
            // passwordLabel
            // 
            passwordLabel.AutoSize = true;
            passwordLabel.Location = new System.Drawing.Point(365, 114);
            passwordLabel.Name = "passwordLabel";
            passwordLabel.Size = new System.Drawing.Size(70, 20);
            passwordLabel.TabIndex = 10;
            passwordLabel.Text = "Password";
            // 
            // ConnectionWindow
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 450);
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
    }
}
