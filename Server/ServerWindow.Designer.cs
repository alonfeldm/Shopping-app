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
            KeyTextBox = new TextBox();
            ((System.ComponentModel.ISupportInitialize)ConnectedClientsGrid).BeginInit();
            SuspendLayout();
            // 
            // PortTextBox
            // 
            PortTextBox.Location = new System.Drawing.Point(130, 60);
            PortTextBox.AutoSize = false;
            PortTextBox.Name = "PortTextBox";
            PortTextBox.PlaceholderText = "Port";
            PortTextBox.Size = new System.Drawing.Size(100, 30);
            PortTextBox.TabIndex = 0;
            // 
            // PortLabel
            // 
            PortLabel.AutoSize = true;
            PortLabel.BackColor = System.Drawing.Color.Transparent;
            PortLabel.ForeColor = System.Drawing.Color.FromArgb(76, 55, 120);
            PortLabel.Location = new System.Drawing.Point(160, 20);
            PortLabel.Name = "PortLabel";
            PortLabel.Size = new System.Drawing.Size(35, 20);
            PortLabel.TabIndex = 1;
            PortLabel.Text = "Port";
            // 
            // StartButton
            // 
            StartButton.BackColor = System.Drawing.Color.FromArgb(37, 99, 235);
            StartButton.FlatAppearance.BorderSize = 0;
            StartButton.FlatStyle = FlatStyle.Flat;
            StartButton.ForeColor = System.Drawing.Color.White;
            StartButton.Location = new System.Drawing.Point(20, 20);
            StartButton.Name = "StartButton";
            StartButton.Size = new System.Drawing.Size(100, 30);
            StartButton.TabIndex = 2;
            StartButton.Text = "Start";
            StartButton.UseVisualStyleBackColor = false;
            StartButton.Click += StartButtonClick;
            // 
            // StopButton
            // 
            StopButton.BackColor = System.Drawing.Color.FromArgb(37, 99, 235);
            StopButton.FlatAppearance.BorderSize = 0;
            StopButton.FlatStyle = FlatStyle.Flat;
            StopButton.ForeColor = System.Drawing.Color.White;
            StopButton.Location = new System.Drawing.Point(20, 60);
            StopButton.Name = "StopButton";
            StopButton.Size = new System.Drawing.Size(100, 30);
            StopButton.TabIndex = 3;
            StopButton.Text = "Stop";
            StopButton.UseVisualStyleBackColor = false;
            StopButton.Click += StopButtonClick;
            // 
            // IPLabel
            // 
            IPLabel.AutoSize = true;
            IPLabel.BackColor = System.Drawing.Color.Transparent;
            IPLabel.ForeColor = System.Drawing.Color.FromArgb(76, 55, 120);
            IPLabel.Location = new System.Drawing.Point(240, 60);
            IPLabel.Name = "IPLabel";
            IPLabel.Size = new System.Drawing.Size(85, 20);
            IPLabel.TabIndex = 4;
            IPLabel.Text = "IP loading...";
            // 
            // LogBox
            // 
            LogBox.Location = new System.Drawing.Point(20, 100);
            LogBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            LogBox.Multiline = true;
            LogBox.Name = "LogBox";
            LogBox.ReadOnly = true;
            LogBox.ScrollBars = ScrollBars.Vertical;
            LogBox.Size = new System.Drawing.Size(475, 640);
            LogBox.TabIndex = 5;
            // 
            // ClearUsersButton
            // 
            ClearUsersButton.BackColor = System.Drawing.Color.FromArgb(37, 99, 235);
            ClearUsersButton.FlatAppearance.BorderSize = 0;
            ClearUsersButton.FlatStyle = FlatStyle.Flat;
            ClearUsersButton.ForeColor = System.Drawing.Color.White;
            ClearUsersButton.Location = new System.Drawing.Point(335, 60);
            ClearUsersButton.Name = "ClearUsersButton";
            ClearUsersButton.Size = new System.Drawing.Size(200, 27);
            ClearUsersButton.TabIndex = 6;
            ClearUsersButton.Text = "Clear user database";
            ClearUsersButton.UseVisualStyleBackColor = false;
            ClearUsersButton.Click += ClearUsersButtonClick;
            // 
            // ClearOrdersButton
            // 
            ClearOrdersButton.BackColor = System.Drawing.Color.FromArgb(37, 99, 235);
            ClearOrdersButton.FlatAppearance.BorderSize = 0;
            ClearOrdersButton.FlatStyle = FlatStyle.Flat;
            ClearOrdersButton.ForeColor = System.Drawing.Color.White;
            ClearOrdersButton.Location = new System.Drawing.Point(755, 60);
            ClearOrdersButton.Name = "ClearOrdersButton";
            ClearOrdersButton.Size = new System.Drawing.Size(200, 27);
            ClearOrdersButton.TabIndex = 7;
            ClearOrdersButton.Text = "Clear order database";
            ClearOrdersButton.UseVisualStyleBackColor = false;
            ClearOrdersButton.Click += ClearOrdersButtonClick;
            // 
            // ClearMessagesButton
            // 
            ClearMessagesButton.BackColor = System.Drawing.Color.FromArgb(37, 99, 235);
            ClearMessagesButton.FlatAppearance.BorderSize = 0;
            ClearMessagesButton.FlatStyle = FlatStyle.Flat;
            ClearMessagesButton.ForeColor = System.Drawing.Color.White;
            ClearMessagesButton.Location = new System.Drawing.Point(545, 60);
            ClearMessagesButton.Name = "ClearMessagesButton";
            ClearMessagesButton.Size = new System.Drawing.Size(200, 27);
            ClearMessagesButton.TabIndex = 8;
            ClearMessagesButton.Text = "Clear message database";
            ClearMessagesButton.UseVisualStyleBackColor = false;
            ClearMessagesButton.Click += ClearMessagesButtonClick;
            // 
            // ConnectedClientsGrid
            // 
            ConnectedClientsGrid.AllowUserToAddRows = false;
            ConnectedClientsGrid.AllowUserToResizeRows = false;
            ConnectedClientsGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ConnectedClientsGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            ConnectedClientsGrid.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(242, 238, 255);
            ConnectedClientsGrid.BackgroundColor = System.Drawing.Color.White;
            ConnectedClientsGrid.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(224, 214, 255);
            ConnectedClientsGrid.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(45, 35, 70);
            ConnectedClientsGrid.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(218, 205, 255);
            ConnectedClientsGrid.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(35, 32, 46);
            ConnectedClientsGrid.EnableHeadersVisualStyles = false;
            ConnectedClientsGrid.GridColor = System.Drawing.Color.FromArgb(205, 194, 235);
            ConnectedClientsGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            ConnectedClientsGrid.Columns.AddRange(new DataGridViewColumn[] { UsernameColumn, IPColumn });
            ConnectedClientsGrid.Location = new System.Drawing.Point(505, 100);
            ConnectedClientsGrid.Name = "ConnectedClientsGrid";
            ConnectedClientsGrid.RowHeadersWidth = 51;
            ConnectedClientsGrid.Size = new System.Drawing.Size(450, 640);
            ConnectedClientsGrid.TabIndex = 9;
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
            // KeyTextBox
            // 
            KeyTextBox.Location = new System.Drawing.Point(240, 20);
            KeyTextBox.AutoSize = false;
            KeyTextBox.Name = "KeyTextBox";
            KeyTextBox.Size = new System.Drawing.Size(715, 30);
            KeyTextBox.TabIndex = 10;
            KeyTextBox.PlaceholderText = "API key";
            KeyTextBox.UseSystemPasswordChar = true;
            // 
            // ServerWindow
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(218, 205, 255);
            ClientSize = new System.Drawing.Size(1000, 800);
            MinimumSize = new System.Drawing.Size(800, 650);
            Controls.Add(KeyTextBox);
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
            Text = "Shopping Server";
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
        private System.Windows.Forms.TextBox KeyTextBox;
    }
}
