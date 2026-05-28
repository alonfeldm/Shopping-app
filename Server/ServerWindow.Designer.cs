using Microsoft.VisualBasic.Logging;
using System.Windows.Forms;

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
            PortTextBox = new System.Windows.Forms.TextBox();
            PortLabel = new System.Windows.Forms.Label();
            StartButton = new System.Windows.Forms.Button();
            StopButton = new System.Windows.Forms.Button();
            IPLabel = new System.Windows.Forms.Label();
            LogBox = new System.Windows.Forms.TextBox();
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
            IPLabel.Size = new System.Drawing.Size(104, 20);
            IPLabel.TabIndex = 4;
            IPLabel.Text = "IP placeholder";
            // 
            // textBox1
            // 
            LogBox.Location = new System.Drawing.Point(25, 206);
            LogBox.Name = "LogBox";
            LogBox.Size = new System.Drawing.Size(1144, 27);
            LogBox.TabIndex = 5;
            LogBox.ReadOnly = true;
            LogBox.Multiline = true;
            LogBox.ScrollBars = ScrollBars.Vertical;
            
            LogBox.Text = "Logs will appear here";  
            // 
            // ServerWindow
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1200, 600);
            Controls.Add(LogBox);
            Controls.Add(IPLabel);
            Controls.Add(StopButton);
            Controls.Add(StartButton);
            Controls.Add(PortLabel);
            Controls.Add(PortTextBox);
            Name = "ServerWindow";
            Text = "Form1";
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
    }
}