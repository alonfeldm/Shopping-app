using System.Windows.Forms;
using Microsoft.VisualBasic.Logging;

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
            storeGrid = new DataGridView();
            NameColumn = new DataGridViewTextBoxColumn();
            PriceColumn = new DataGridViewTextBoxColumn();
            qtyToCartBoxColumn = new DataGridViewTextBoxColumn();
            addToCartButtonColumn = new DataGridViewButtonColumn();
            TotalColumn = new DataGridViewTextBoxColumn();
            QuantityColumn = new DataGridViewTextBoxColumn();
            productIdColumn = new DataGridViewTextBoxColumn();
            addressBox = new TextBox();
            creditCardBox = new TextBox();
            monthBox = new TextBox();
            yearBox = new TextBox();
            firstNameBox = new TextBox();
            CvvBox = new TextBox();
            CvvLabel = new Label();
            monthLabel = new Label();
            yearLabel = new Label();
            creditNumberLabel = new Label();
            addressLabel = new Label();
            firstNameLabel = new Label();
            lastNameBox = new TextBox();
            lastNameLabel = new Label();
            orderButton = new Button();
            logBox = new TextBox();
            ((System.ComponentModel.ISupportInitialize)storeGrid).BeginInit();
            SuspendLayout();
            // 
            // connectButton
            // 
            connectButton.BackColor = System.Drawing.Color.FromArgb(37, 99, 235);
            connectButton.FlatAppearance.BorderSize = 0;
            connectButton.FlatStyle = FlatStyle.Flat;
            connectButton.ForeColor = System.Drawing.Color.White;
            connectButton.Location = new System.Drawing.Point(20, 20);
            connectButton.Name = "connectButton";
            connectButton.Size = new System.Drawing.Size(100, 30);
            connectButton.TabIndex = 0;
            connectButton.Text = "Connect";
            connectButton.UseVisualStyleBackColor = false;
            connectButton.Click += ConnectButton_Click;
            // 
            // ipLabel
            // 
            ipLabel.Location = new System.Drawing.Point(190, 20);
            ipLabel.BackColor = System.Drawing.Color.Transparent;
            ipLabel.ForeColor = System.Drawing.Color.FromArgb(76, 55, 120);
            ipLabel.Name = "ipLabel";
            ipLabel.Size = new System.Drawing.Size(30, 30);
            ipLabel.TabIndex = 1;
            ipLabel.Text = "IP";
            // 
            // ipTextBox
            // 
            ipTextBox.AutoSize = false;
            ipTextBox.Location = new System.Drawing.Point(130, 60);
            ipTextBox.Name = "ipTextBox";
            ipTextBox.PlaceholderText = "Server IP";
            ipTextBox.Size = new System.Drawing.Size(150, 30);
            ipTextBox.TabIndex = 2;
            // 
            // portTextBox
            // 
            portTextBox.AutoSize = false;
            portTextBox.Location = new System.Drawing.Point(290, 60);
            portTextBox.Name = "portTextBox";
            portTextBox.PlaceholderText = "Port";
            portTextBox.Size = new System.Drawing.Size(150, 30);
            portTextBox.TabIndex = 3;
            // 
            // portLabel
            // 
            portLabel.Location = new System.Drawing.Point(345, 20);
            portLabel.BackColor = System.Drawing.Color.Transparent;
            portLabel.ForeColor = System.Drawing.Color.FromArgb(76, 55, 120);
            portLabel.Name = "portLabel";
            portLabel.Size = new System.Drawing.Size(40, 30);
            portLabel.TabIndex = 4;
            portLabel.Text = "Port";
            // 
            // registerButton
            // 
            registerButton.BackColor = System.Drawing.Color.FromArgb(37, 99, 235);
            registerButton.FlatAppearance.BorderSize = 0;
            registerButton.FlatStyle = FlatStyle.Flat;
            registerButton.ForeColor = System.Drawing.Color.White;
            registerButton.Location = new System.Drawing.Point(450, 20);
            registerButton.Name = "registerButton";
            registerButton.Size = new System.Drawing.Size(100, 30);
            registerButton.TabIndex = 5;
            registerButton.Text = "Register";
            registerButton.UseVisualStyleBackColor = false;
            registerButton.Click += RegisterButton_Click;
            // 
            // loginButton
            // 
            loginButton.BackColor = System.Drawing.Color.FromArgb(37, 99, 235);
            loginButton.FlatAppearance.BorderSize = 0;
            loginButton.FlatStyle = FlatStyle.Flat;
            loginButton.ForeColor = System.Drawing.Color.White;
            loginButton.Location = new System.Drawing.Point(450, 60);
            loginButton.Name = "loginButton";
            loginButton.Size = new System.Drawing.Size(100, 30);
            loginButton.TabIndex = 6;
            loginButton.Text = "Login";
            loginButton.UseVisualStyleBackColor = false;
            loginButton.Click += LoginButton_Click;
            // 
            // passwordTextBox
            // 
            passwordTextBox.Location = new System.Drawing.Point(720, 60);
            passwordTextBox.AutoSize = false;
            passwordTextBox.Name = "passwordTextBox";
            passwordTextBox.PlaceholderText = "Password";
            passwordTextBox.Size = new System.Drawing.Size(150, 30);
            passwordTextBox.TabIndex = 7;
            passwordTextBox.UseSystemPasswordChar = true;
            // 
            // usernameTextBox
            // 
            usernameTextBox.Location = new System.Drawing.Point(560, 60);
            usernameTextBox.AutoSize = false;
            usernameTextBox.Name = "usernameTextBox";
            usernameTextBox.PlaceholderText = "Username";
            usernameTextBox.Size = new System.Drawing.Size(150, 30);
            usernameTextBox.TabIndex = 8;
            // 
            // usernameLabel
            // 
            usernameLabel.Location = new System.Drawing.Point(600, 20);
            usernameLabel.BackColor = System.Drawing.Color.Transparent;
            usernameLabel.ForeColor = System.Drawing.Color.FromArgb(76, 55, 120);
            usernameLabel.Name = "usernameLabel";
            usernameLabel.Size = new System.Drawing.Size(75, 30);
            usernameLabel.TabIndex = 9;
            usernameLabel.Text = "Username";
            // 
            // passwordLabel
            // 
            passwordLabel.Location = new System.Drawing.Point(760, 20);
            passwordLabel.BackColor = System.Drawing.Color.Transparent;
            passwordLabel.ForeColor = System.Drawing.Color.FromArgb(76, 55, 120);
            passwordLabel.Name = "passwordLabel";
            passwordLabel.Size = new System.Drawing.Size(70, 30);
            passwordLabel.TabIndex = 10;
            passwordLabel.Text = "Password";
            // 
            // disconnectButton
            // 
            disconnectButton.BackColor = System.Drawing.Color.FromArgb(37, 99, 235);
            disconnectButton.FlatAppearance.BorderSize = 0;
            disconnectButton.FlatStyle = FlatStyle.Flat;
            disconnectButton.ForeColor = System.Drawing.Color.White;
            disconnectButton.Location = new System.Drawing.Point(20, 60);
            disconnectButton.Name = "disconnectButton";
            disconnectButton.Size = new System.Drawing.Size(100, 30);
            disconnectButton.TabIndex = 11;
            disconnectButton.Text = "Disconnect";
            disconnectButton.UseVisualStyleBackColor = false;
            disconnectButton.Click += DisconnectButton_Click;
            // 
            // ChatBox
            // 
            ChatBox.Location = new System.Drawing.Point(1130, 60);
            ChatBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            ChatBox.Multiline = true;
            ChatBox.Name = "ChatBox";
            ChatBox.ReadOnly = true;
            ChatBox.ScrollBars = ScrollBars.Vertical;
            ChatBox.Size = new System.Drawing.Size(450, 650);
            ChatBox.TabIndex = 12;
            // 
            // ChatLabel
            // 
            ChatLabel.Location = new System.Drawing.Point(1350, 15);
            ChatLabel.BackColor = System.Drawing.Color.Transparent;
            ChatLabel.ForeColor = System.Drawing.Color.FromArgb(76, 55, 120);
            ChatLabel.Name = "ChatLabel";
            ChatLabel.Size = new System.Drawing.Size(39, 30);
            ChatLabel.TabIndex = 13;
            ChatLabel.Text = "Chat";
            // 
            // MessageTextBox
            // 
            MessageTextBox.Location = new System.Drawing.Point(1130, 720);
            MessageTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            MessageTextBox.AutoSize = false;
            MessageTextBox.Name = "MessageTextBox";
            MessageTextBox.PlaceholderText = "Message";
            MessageTextBox.Size = new System.Drawing.Size(360, 30);
            MessageTextBox.TabIndex = 14;
            // 
            // SendButton
            // 
            SendButton.BackColor = System.Drawing.Color.FromArgb(37, 99, 235);
            SendButton.FlatAppearance.BorderSize = 0;
            SendButton.FlatStyle = FlatStyle.Flat;
            SendButton.ForeColor = System.Drawing.Color.White;
            SendButton.Location = new System.Drawing.Point(1500, 720);
            SendButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            SendButton.Name = "SendButton";
            SendButton.Size = new System.Drawing.Size(80, 30);
            SendButton.TabIndex = 15;
            SendButton.Text = "Send";
            SendButton.UseVisualStyleBackColor = false;
            SendButton.Click += SendButton_Click;
            // 
            // storeGrid
            // 
            storeGrid.AllowUserToAddRows = false;
            storeGrid.AllowUserToResizeRows = false;
            storeGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            storeGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            storeGrid.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(242, 238, 255);
            storeGrid.BackgroundColor = System.Drawing.Color.White;
            storeGrid.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(224, 214, 255);
            storeGrid.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(45, 35, 70);
            storeGrid.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(218, 205, 255);
            storeGrid.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(35, 32, 46);
            storeGrid.EnableHeadersVisualStyles = false;
            storeGrid.GridColor = System.Drawing.Color.FromArgb(205, 194, 235);
            storeGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            storeGrid.Columns.AddRange(new DataGridViewColumn[] { NameColumn, PriceColumn, qtyToCartBoxColumn, addToCartButtonColumn, TotalColumn, QuantityColumn, productIdColumn });
            storeGrid.Location = new System.Drawing.Point(20, 100);
            storeGrid.Name = "storeGrid";
            storeGrid.RowHeadersWidth = 51;
            storeGrid.Size = new System.Drawing.Size(850, 650);
            storeGrid.TabIndex = 16;
            storeGrid.CellContentClick += StoreGrid_CellContentClick;
            // 
            // NameColumn
            // 
            NameColumn.HeaderText = "Name";
            NameColumn.MinimumWidth = 6;
            NameColumn.Name = "NameColumn";
            NameColumn.ReadOnly = true;
            NameColumn.Width = 200;
            // 
            // PriceColumn
            // 
            PriceColumn.HeaderText = "Price";
            PriceColumn.MinimumWidth = 6;
            PriceColumn.Name = "PriceColumn";
            PriceColumn.ReadOnly = true;
            // 
            // qtyToCartBoxColumn
            // 
            qtyToCartBoxColumn.HeaderText = "Quantity to add to cart";
            qtyToCartBoxColumn.MinimumWidth = 6;
            qtyToCartBoxColumn.Name = "qtyToCartBoxColumn";
            qtyToCartBoxColumn.Width = 125;
            // 
            // addToCartButtonColumn
            // 
            addToCartButtonColumn.HeaderText = "Add to cart";
            addToCartButtonColumn.MinimumWidth = 6;
            addToCartButtonColumn.Name = "addToCartButtonColumn";
            // 
            // TotalColumn
            // 
            TotalColumn.HeaderText = "Total";
            TotalColumn.MinimumWidth = 6;
            TotalColumn.Name = "TotalColumn";
            TotalColumn.ReadOnly = true;
            TotalColumn.Width = 125;
            // 
            // QuantityColumn
            // 
            QuantityColumn.HeaderText = "Quantity in cart";
            QuantityColumn.MinimumWidth = 6;
            QuantityColumn.Name = "QuantityColumn";
            QuantityColumn.ReadOnly = true;
            // 
            // productIdColumn
            // 
            productIdColumn.HeaderText = "Product ID";
            productIdColumn.MinimumWidth = 6;
            productIdColumn.Name = "productIdColumn";
            productIdColumn.ReadOnly = true;
            productIdColumn.Visible = false;
            productIdColumn.Width = 125;
            // 
            // addressBox
            // 
            addressBox.Location = new System.Drawing.Point(880, 60);
            addressBox.AutoSize = false;
            addressBox.Name = "addressBox";
            addressBox.PlaceholderText = "Address";
            addressBox.Size = new System.Drawing.Size(240, 30);
            addressBox.TabIndex = 17;
            // 
            // creditCardBox
            // 
            creditCardBox.Location = new System.Drawing.Point(940, 140);
            creditCardBox.AutoSize = false;
            creditCardBox.Name = "creditCardBox";
            creditCardBox.PlaceholderText = "Card number";
            creditCardBox.Size = new System.Drawing.Size(180, 30);
            creditCardBox.TabIndex = 18;
            // 
            // monthBox
            // 
            monthBox.Location = new System.Drawing.Point(880, 300);
            monthBox.AutoSize = false;
            monthBox.Name = "monthBox";
            monthBox.PlaceholderText = "MM";
            monthBox.Size = new System.Drawing.Size(80, 30);
            monthBox.TabIndex = 19;
            // 
            // yearBox
            // 
            yearBox.Location = new System.Drawing.Point(970, 300);
            yearBox.AutoSize = false;
            yearBox.Name = "yearBox";
            yearBox.PlaceholderText = "YYYY";
            yearBox.Size = new System.Drawing.Size(80, 30);
            yearBox.TabIndex = 20;
            // 
            // firstNameBox
            // 
            firstNameBox.Location = new System.Drawing.Point(880, 220);
            firstNameBox.AutoSize = false;
            firstNameBox.Name = "firstNameBox";
            firstNameBox.PlaceholderText = "First name";
            firstNameBox.Size = new System.Drawing.Size(115, 30);
            firstNameBox.TabIndex = 21;
            // 
            // CvvBox
            // 
            CvvBox.Location = new System.Drawing.Point(880, 140);
            CvvBox.AutoSize = false;
            CvvBox.Name = "CvvBox";
            CvvBox.PlaceholderText = "CVV";
            CvvBox.Size = new System.Drawing.Size(50, 30);
            CvvBox.TabIndex = 23;
            // 
            // CvvLabel
            // 
            CvvLabel.Location = new System.Drawing.Point(885, 100);
            CvvLabel.BackColor = System.Drawing.Color.Transparent;
            CvvLabel.ForeColor = System.Drawing.Color.FromArgb(76, 55, 120);
            CvvLabel.Name = "CvvLabel";
            CvvLabel.Size = new System.Drawing.Size(40, 30);
            CvvLabel.TabIndex = 24;
            CvvLabel.Text = "CVV";
            // 
            // monthLabel
            // 
            monthLabel.Location = new System.Drawing.Point(880, 260);
            monthLabel.BackColor = System.Drawing.Color.Transparent;
            monthLabel.ForeColor = System.Drawing.Color.FromArgb(76, 55, 120);
            monthLabel.Name = "monthLabel";
            monthLabel.Size = new System.Drawing.Size(80, 30);
            monthLabel.TabIndex = 25;
            monthLabel.Text = "Exp month";
            // 
            // yearLabel
            // 
            yearLabel.Location = new System.Drawing.Point(1005, 260);
            yearLabel.BackColor = System.Drawing.Color.Transparent;
            yearLabel.ForeColor = System.Drawing.Color.FromArgb(76, 55, 120);
            yearLabel.Name = "yearLabel";
            yearLabel.Size = new System.Drawing.Size(80, 30);
            yearLabel.TabIndex = 26;
            yearLabel.Text = "Exp year";
            // 
            // creditNumberLabel
            // 
            creditNumberLabel.Location = new System.Drawing.Point(960, 100);
            creditNumberLabel.BackColor = System.Drawing.Color.Transparent;
            creditNumberLabel.ForeColor = System.Drawing.Color.FromArgb(76, 55, 120);
            creditNumberLabel.Name = "creditNumberLabel";
            creditNumberLabel.Size = new System.Drawing.Size(140, 30);
            creditNumberLabel.TabIndex = 27;
            creditNumberLabel.Text = "Credit card number";
            // 
            // addressLabel
            // 
            addressLabel.Location = new System.Drawing.Point(965, 20);
            addressLabel.BackColor = System.Drawing.Color.Transparent;
            addressLabel.ForeColor = System.Drawing.Color.FromArgb(76, 55, 120);
            addressLabel.Name = "addressLabel";
            addressLabel.Size = new System.Drawing.Size(70, 30);
            addressLabel.TabIndex = 28;
            addressLabel.Text = "Address";
            // 
            // firstNameLabel
            // 
            firstNameLabel.Location = new System.Drawing.Point(897, 180);
            firstNameLabel.BackColor = System.Drawing.Color.Transparent;
            firstNameLabel.ForeColor = System.Drawing.Color.FromArgb(76, 55, 120);
            firstNameLabel.Name = "firstNameLabel";
            firstNameLabel.Size = new System.Drawing.Size(80, 30);
            firstNameLabel.TabIndex = 29;
            firstNameLabel.Text = "First name";
            // 
            // lastNameBox
            // 
            lastNameBox.Location = new System.Drawing.Point(1005, 220);
            lastNameBox.AutoSize = false;
            lastNameBox.Name = "lastNameBox";
            lastNameBox.PlaceholderText = "Last name";
            lastNameBox.Size = new System.Drawing.Size(115, 30);
            lastNameBox.TabIndex = 30;
            // 
            // lastNameLabel
            // 
            lastNameLabel.Location = new System.Drawing.Point(1022, 180);
            lastNameLabel.BackColor = System.Drawing.Color.Transparent;
            lastNameLabel.ForeColor = System.Drawing.Color.FromArgb(76, 55, 120);
            lastNameLabel.Name = "lastNameLabel";
            lastNameLabel.Size = new System.Drawing.Size(80, 30);
            lastNameLabel.TabIndex = 31;
            lastNameLabel.Text = "Last name";
            // 
            // orderButton
            // 
            orderButton.BackColor = System.Drawing.Color.FromArgb(37, 99, 235);
            orderButton.FlatAppearance.BorderSize = 0;
            orderButton.FlatStyle = FlatStyle.Flat;
            orderButton.ForeColor = System.Drawing.Color.White;
            orderButton.Location = new System.Drawing.Point(1060, 300);
            orderButton.Name = "orderButton";
            orderButton.Size = new System.Drawing.Size(60, 30);
            orderButton.TabIndex = 32;
            orderButton.Text = "Order";
            orderButton.UseVisualStyleBackColor = false;
            orderButton.Click += OrderButton_Click;
            // 
            // logBox
            // 
            logBox.Location = new System.Drawing.Point(880, 340);
            logBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
            logBox.Multiline = true;
            logBox.Name = "logBox";
            logBox.ReadOnly = true;
            logBox.ScrollBars = ScrollBars.Vertical;
            logBox.Size = new System.Drawing.Size(240, 410);
            logBox.TabIndex = 33;
            // 
            // ConnectionWindow
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(218, 205, 255);
            ClientSize = new System.Drawing.Size(1582, 803);
            MinimumSize = new System.Drawing.Size(1200, 700);
            Controls.Add(logBox);
            Controls.Add(orderButton);
            Controls.Add(lastNameLabel);
            Controls.Add(lastNameBox);
            Controls.Add(firstNameLabel);
            Controls.Add(addressLabel);
            Controls.Add(creditNumberLabel);
            Controls.Add(yearLabel);
            Controls.Add(monthLabel);
            Controls.Add(CvvLabel);
            Controls.Add(CvvBox);
            Controls.Add(firstNameBox);
            Controls.Add(yearBox);
            Controls.Add(monthBox);
            Controls.Add(creditCardBox);
            Controls.Add(addressBox);
            Controls.Add(storeGrid);
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
            Text = "Shopping app client";
            ((System.ComponentModel.ISupportInitialize)storeGrid).EndInit();
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
        private DataGridView storeGrid;
        private TextBox addressBox;
        private TextBox creditCardBox;
        private TextBox monthBox;
        private TextBox yearBox;
        private TextBox firstNameBox;
        private TextBox CvvBox;
        private Label CvvLabel;
        private Label monthLabel;
        private Label yearLabel;
        private Label creditNumberLabel;
        private Label addressLabel;
        private Label firstNameLabel;
        private TextBox lastNameBox;
        private Label lastNameLabel;
        private Button orderButton;
        private TextBox logBox;
        private DataGridViewTextBoxColumn NameColumn;
        private DataGridViewTextBoxColumn PriceColumn;
        private DataGridViewTextBoxColumn qtyToCartBoxColumn;
        private DataGridViewButtonColumn addToCartButtonColumn;
        private DataGridViewTextBoxColumn TotalColumn;
        private DataGridViewTextBoxColumn QuantityColumn;
        private DataGridViewTextBoxColumn productIdColumn;
    }
}
