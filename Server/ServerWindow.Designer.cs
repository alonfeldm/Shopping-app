using Microsoft.VisualBasic.Logging;
using System.Net;
using System.Windows.Forms;
using System;
using System.Net.Sockets;

namespace Server
{
    partial class ServerWindow
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
            PortTextBox = new TextBox();
            PortLabel = new Label();
            StartButton = new Button();
            StopButton = new Button();
            IPLabel = new Label();
            LogBox = new TextBox();
            ClearUsersButton = new Button();
            ClearOrdersButton = new Button();
            ClearMessagesButton = new Button();
            SuspendLayout();
            // 
            // PortTextBox
            // 
            PortTextBox.Location = new System.Drawing.Point(150, 100);
            PortTextBox.Name = "PortTextBox";
            PortTextBox.Size = new System.Drawing.Size(100, 27);
            PortTextBox.TabIndex = 0;
            // 
            // PortLabel
            // 
            PortLabel.AutoSize = true;
            PortLabel.Location = new System.Drawing.Point(180, 65);
            PortLabel.Name = "PortLabel";
            PortLabel.Size = new System.Drawing.Size(35, 20);
            PortLabel.TabIndex = 1;
            PortLabel.Text = "Port";
            // 
            // StartButton
            // 
            StartButton.Location = new System.Drawing.Point(25, 71);
            StartButton.Name = "StartButton";
            StartButton.Size = new System.Drawing.Size(100, 30);
            StartButton.TabIndex = 2;
            StartButton.Text = "Start";
            StartButton.UseVisualStyleBackColor = true;
            StartButton.Click += StartButton_Click;
            // 
            // StopButton
            // 
            StopButton.Location = new System.Drawing.Point(25, 129);
            StopButton.Name = "StopButton";
            StopButton.Size = new System.Drawing.Size(100, 30);
            StopButton.TabIndex = 3;
            StopButton.Text = "Stop";
            StopButton.UseVisualStyleBackColor = true;
            StopButton.Click += StopButton_Click;
            // 
            // IPLabel
            // 
            IPLabel.AutoSize = true;
            IPLabel.Location = new System.Drawing.Point(280, 100);
            IPLabel.Name = "IPLabel";
            IPLabel.Size = new System.Drawing.Size(85, 20);
            IPLabel.TabIndex = 4;
            IPLabel.Text = "IP loading...";
            // 
            // LogBox
            // 
            LogBox.Location = new System.Drawing.Point(25, 200);
            LogBox.Multiline = true;
            LogBox.Name = "LogBox";
            LogBox.ReadOnly = true;
            LogBox.ScrollBars = ScrollBars.Vertical;
            LogBox.Size = new System.Drawing.Size(1150, 375);
            LogBox.TabIndex = 5;
            // 
            // ClearUsersButton
            // 
            ClearUsersButton.Location = new System.Drawing.Point(387, 100);
            ClearUsersButton.Name = "ClearUsersButton";
            ClearUsersButton.Size = new System.Drawing.Size(200, 27);
            ClearUsersButton.TabIndex = 6;
            ClearUsersButton.Text = "Clear user database";
            ClearUsersButton.UseVisualStyleBackColor = true;
            ClearUsersButton.Click += ClearUsersButton_Click;
            // 
            // ClearOrdersButton
            // 
            ClearOrdersButton.Location = new System.Drawing.Point(799, 100);
            ClearOrdersButton.Name = "ClearOrdersButton";
            ClearOrdersButton.Size = new System.Drawing.Size(200, 27);
            ClearOrdersButton.TabIndex = 7;
            ClearOrdersButton.Text = "Clear order database";
            ClearOrdersButton.UseVisualStyleBackColor = true;
            ClearOrdersButton.Click += ClearOrdersButton_Click;
            // 
            // ClearMessagesButton
            // 
            ClearMessagesButton.Location = new System.Drawing.Point(593, 100);
            ClearMessagesButton.Name = "ClearMessagesButton";
            ClearMessagesButton.Size = new System.Drawing.Size(200, 27);
            ClearMessagesButton.TabIndex = 8;
            ClearMessagesButton.Text = "Clear message database";
            ClearMessagesButton.UseVisualStyleBackColor = true;
            ClearMessagesButton.Click += ClearMessagesButton_Click;
            // 
            // ServerWindow
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1200, 600);
            Controls.Add(ClearMessagesButton);
            Controls.Add(ClearOrdersButton);
            Controls.Add(ClearUsersButton);
            Controls.Add(LogBox);
            Controls.Add(IPLabel);
            Controls.Add(StopButton);
            Controls.Add(StartButton);
            Controls.Add(PortLabel);
            Controls.Add(PortTextBox);
            Name = "ServerWindow";
            Text = "Server";
            ResumeLayout(false);
            PerformLayout();
        }

        private void ClearMessagesButton_Click1(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion

        private System.Windows.Forms.TextBox PortTextBox;
        private System.Windows.Forms.Label PortLabel;
        private System.Windows.Forms.Button StartButton;
        private System.Windows.Forms.Button StopButton;
        private System.Windows.Forms.Label IPLabel;
        private System.Windows.Forms.TextBox LogBox;
        private System.Windows.Forms.Button ClearUsersButton;
        private System.Windows.Forms.Button ClearOrdersButton;
        private System.Windows.Forms.Button ClearMessagesButton;
    }
}