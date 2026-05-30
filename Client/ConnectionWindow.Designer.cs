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
            productIdColumn = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)storeGrid).BeginInit();
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
            MessageTextBox.Size = new System.Drawing.Size(300, 27);
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
            SendButton.Click += SendButton_Click;
            // 
            // storeGrid
            // 
            storeGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            storeGrid.Columns.AddRange(new DataGridViewColumn[] { NameColumn, PriceColumn, qtyToCartBoxColumn, addToCartButtonColumn, TotalColumn, QuantityColumn, productIdColumn });
            storeGrid.Location = new System.Drawing.Point(45, 107);
            storeGrid.Name = "storeGrid";
            storeGrid.RowHeadersWidth = 51;
            storeGrid.Size = new System.Drawing.Size(869, 635);
            storeGrid.TabIndex = 16;
            storeGrid.AllowUserToAddRows = false;
            storeGrid.CellContentClick += StoreGrid_CellContentClick;
            // 
            // NameColumn
            // 
            NameColumn.HeaderText = "Name";
            NameColumn.MinimumWidth = 6;
            NameColumn.Name = "NameColumn";
            NameColumn.ReadOnly = true;
            NameColumn.Width = 125;
            // 
            // PriceColumn
            // 
            PriceColumn.HeaderText = "Price";
            PriceColumn.MinimumWidth = 6;
            PriceColumn.Name = "PriceColumn";
            PriceColumn.ReadOnly = true;
            PriceColumn.Width = 125;
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
            addToCartButtonColumn.Width = 125;
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
            QuantityColumn.Width = 125;
            // 
            // addressBox
            // 
            addressBox.Location = new System.Drawing.Point(932, 50);
            addressBox.Name = "addressBox";
            addressBox.Size = new System.Drawing.Size(232, 27);
            addressBox.TabIndex = 17;
            // 
            // creditCardBox
            // 
            creditCardBox.Location = new System.Drawing.Point(932, 176);
            creditCardBox.Name = "creditCardBox";
            creditCardBox.Size = new System.Drawing.Size(232, 27);
            creditCardBox.TabIndex = 18;
            // 
            // monthBox
            // 
            monthBox.Location = new System.Drawing.Point(994, 107);
            monthBox.Name = "monthBox";
            monthBox.Size = new System.Drawing.Size(80, 27);
            monthBox.TabIndex = 19;
            // 
            // yearBox
            // 
            yearBox.Location = new System.Drawing.Point(1099, 107);
            yearBox.Name = "yearBox";
            yearBox.Size = new System.Drawing.Size(65, 27);
            yearBox.TabIndex = 20;
            // 
            // firstNameBox
            // 
            firstNameBox.Location = new System.Drawing.Point(932, 242);
            firstNameBox.Name = "firstNameBox";
            firstNameBox.Size = new System.Drawing.Size(99, 27);
            firstNameBox.TabIndex = 21;
            // 
            // CvvBox
            // 
            CvvBox.Location = new System.Drawing.Point(932, 107);
            CvvBox.Name = "CvvBox";
            CvvBox.Size = new System.Drawing.Size(36, 27);
            CvvBox.TabIndex = 23;
            // 
            // CvvLabel
            // 
            CvvLabel.AutoSize = true;
            CvvLabel.Location = new System.Drawing.Point(932, 84);
            CvvLabel.Name = "CvvLabel";
            CvvLabel.Size = new System.Drawing.Size(36, 20);
            CvvLabel.TabIndex = 24;
            CvvLabel.Text = "CVV";
            // 
            // monthLabel
            // 
            monthLabel.AutoSize = true;
            monthLabel.Location = new System.Drawing.Point(994, 84);
            monthLabel.Name = "monthLabel";
            monthLabel.Size = new System.Drawing.Size(80, 20);
            monthLabel.TabIndex = 25;
            monthLabel.Text = "Exp month";
            // 
            // yearLabel
            // 
            yearLabel.AutoSize = true;
            yearLabel.Location = new System.Drawing.Point(1099, 84);
            yearLabel.Name = "yearLabel";
            yearLabel.Size = new System.Drawing.Size(65, 20);
            yearLabel.TabIndex = 26;
            yearLabel.Text = "Exp year";
            // 
            // creditNumberLabel
            // 
            creditNumberLabel.AutoSize = true;
            creditNumberLabel.Location = new System.Drawing.Point(981, 153);
            creditNumberLabel.Name = "creditNumberLabel";
            creditNumberLabel.Size = new System.Drawing.Size(137, 20);
            creditNumberLabel.TabIndex = 27;
            creditNumberLabel.Text = "Credit card number";
            // 
            // addressLabel
            // 
            addressLabel.AutoSize = true;
            addressLabel.Location = new System.Drawing.Point(1012, 15);
            addressLabel.Name = "addressLabel";
            addressLabel.Size = new System.Drawing.Size(62, 20);
            addressLabel.TabIndex = 28;
            addressLabel.Text = "Address";
            // 
            // firstNameLabel
            // 
            firstNameLabel.AutoSize = true;
            firstNameLabel.Location = new System.Drawing.Point(932, 219);
            firstNameLabel.Name = "firstNameLabel";
            firstNameLabel.Size = new System.Drawing.Size(77, 20);
            firstNameLabel.TabIndex = 29;
            firstNameLabel.Text = "First name";
            // 
            // lastNameBox
            // 
            lastNameBox.Location = new System.Drawing.Point(1065, 242);
            lastNameBox.Name = "lastNameBox";
            lastNameBox.Size = new System.Drawing.Size(99, 27);
            lastNameBox.TabIndex = 30;
            // 
            // lastNameLabel
            // 
            lastNameLabel.AutoSize = true;
            lastNameLabel.Location = new System.Drawing.Point(1065, 219);
            lastNameLabel.Name = "lastNameLabel";
            lastNameLabel.Size = new System.Drawing.Size(76, 20);
            lastNameLabel.TabIndex = 31;
            lastNameLabel.Text = "Last name";
            // 
            // orderButton
            // 
            orderButton.Location = new System.Drawing.Point(932, 287);
            orderButton.Name = "orderButton";
            orderButton.Size = new System.Drawing.Size(57, 31);
            orderButton.TabIndex = 32;
            orderButton.Text = "Order";
            orderButton.UseVisualStyleBackColor = true;
            orderButton.Click += orderButton_Click;
            // 
            // logBox
            // 
            logBox.Location = new System.Drawing.Point(932, 324);
            logBox.Multiline = true;
            logBox.Name = "logBox";
            logBox.ReadOnly = true;
            logBox.ScrollBars = ScrollBars.Vertical;
            logBox.Size = new System.Drawing.Size(227, 418);
            logBox.TabIndex = 33;
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
            // ConnectionWindow
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1600, 800);
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
