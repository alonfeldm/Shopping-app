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
            ConnectedClientsGrid = new DataGridView();
            UsernameColumn = new DataGridViewTextBoxColumn();
            IPColumn = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)ConnectedClientsGrid).BeginInit();
            SuspendLayout();
            // 
            // PortTextBox
            // 
            PortTextBox.Location = new System.Drawing.Point(130, 60);
            PortTextBox.Name = "PortTextBox";
            PortTextBox.Size = new System.Drawing.Size(100, 27);
            PortTextBox.TabIndex = 0;
            // 
            // PortLabel
            // 
            PortLabel.AutoSize = true;
            PortLabel.Location = new System.Drawing.Point(160, 20);
            PortLabel.Name = "PortLabel";
            PortLabel.Size = new System.Drawing.Size(35, 20);
            PortLabel.TabIndex = 1;
            PortLabel.Text = "Port";
            // 
            // StartButton
            // 
            StartButton.Location = new System.Drawing.Point(20, 20);
            StartButton.Name = "StartButton";
            StartButton.Size = new System.Drawing.Size(100, 30);
            StartButton.TabIndex = 2;
            StartButton.Text = "Start";
            StartButton.UseVisualStyleBackColor = true;
            StartButton.Click += StartButton_Click;
            // 
            // StopButton
            // 
            StopButton.Location = new System.Drawing.Point(20, 60);
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
            IPLabel.Location = new System.Drawing.Point(240, 60);
            IPLabel.Name = "IPLabel";
            IPLabel.Size = new System.Drawing.Size(85, 20);
            IPLabel.TabIndex = 4;
            IPLabel.Text = "IP loading...";
            // 
            // LogBox
            // 
            LogBox.Location = new System.Drawing.Point(20, 100);
            LogBox.Multiline = true;
            LogBox.Name = "LogBox";
            LogBox.ReadOnly = true;
            LogBox.ScrollBars = ScrollBars.Vertical;
            LogBox.Size = new System.Drawing.Size(475, 640);
            LogBox.TabIndex = 5;
            // 
            // ClearUsersButton
            // 
            ClearUsersButton.Location = new System.Drawing.Point(335, 60);
            ClearUsersButton.Name = "ClearUsersButton";
            ClearUsersButton.Size = new System.Drawing.Size(200, 27);
            ClearUsersButton.TabIndex = 6;
            ClearUsersButton.Text = "Clear user database";
            ClearUsersButton.UseVisualStyleBackColor = true;
            ClearUsersButton.Click += ClearUsersButton_Click;
            // 
            // ClearOrdersButton
            // 
            ClearOrdersButton.Location = new System.Drawing.Point(755, 60);
            ClearOrdersButton.Name = "ClearOrdersButton";
            ClearOrdersButton.Size = new System.Drawing.Size(200, 27);
            ClearOrdersButton.TabIndex = 7;
            ClearOrdersButton.Text = "Clear order database";
            ClearOrdersButton.UseVisualStyleBackColor = true;
            ClearOrdersButton.Click += ClearOrdersButton_Click;
            // 
            // ClearMessagesButton
            // 
            ClearMessagesButton.Location = new System.Drawing.Point(545, 60);
            ClearMessagesButton.Name = "ClearMessagesButton";
            ClearMessagesButton.Size = new System.Drawing.Size(200, 27);
            ClearMessagesButton.TabIndex = 8;
            ClearMessagesButton.Text = "Clear message database";
            ClearMessagesButton.UseVisualStyleBackColor = true;
            ClearMessagesButton.Click += ClearMessagesButton_Click;
            // 
            // ConnectedClientsGrid
            // 
            ConnectedClientsGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            ConnectedClientsGrid.Columns.AddRange(new DataGridViewColumn[] { UsernameColumn, IPColumn });
            ConnectedClientsGrid.Location = new System.Drawing.Point(505, 100);
            ConnectedClientsGrid.Name = "dataGridView1";
            ConnectedClientsGrid.RowHeadersWidth = 51;
            ConnectedClientsGrid.Size = new System.Drawing.Size(450, 640);
            ConnectedClientsGrid.TabIndex = 9;
            ConnectedClientsGrid.AllowUserToAddRows = false;
            // 
            // UsernameColumn
            // 
            UsernameColumn.HeaderText = "Username";
            UsernameColumn.MinimumWidth = 6;
            UsernameColumn.Name = "UsernameColumn";
            UsernameColumn.ReadOnly = true;
            UsernameColumn.Width = 175;
            // 
            // IPColumn
            // 
            IPColumn.HeaderText = "IP";
            IPColumn.MinimumWidth = 6;
            IPColumn.Name = "IPColumn";
            IPColumn.ReadOnly = true;
            IPColumn.Width = 250;
            // 
            // ServerWindow
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1000, 800);
            Controls.Add(ConnectedClientsGrid);
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
            ((System.ComponentModel.ISupportInitialize)ConnectedClientsGrid).EndInit();
            ResumeLayout(false);
            PerformLayout();
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
        private System.Windows.Forms.DataGridView ConnectedClientsGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn UsernameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn IPColumn;
    }
}