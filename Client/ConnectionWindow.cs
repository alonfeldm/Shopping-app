using System;
using System.Windows.Forms;
using SharedLibraries.Payloads;
using System.Collections.Generic;
using SharedLibraries.ValidationHelpers;

namespace Client
{
    public partial class ConnectionWindow : Form
    {
        public List<ProductAndQuantity> cart = new List<ProductAndQuantity>();
        private Client Client { get; set; } = new Client();
        public ConnectionWindow()
        {
            InitializeComponent();
            Client.PrintOut += PrintMessage;
            Client.DisplayMessage += DisplayMessage;
            Client.AppendStore += AppendProductToStoreGrid;
            Client.ClearStore += ClearStore;
            Client.ClearMessages += ClearMessages;
        }

        private void connectButton_Click(object? sender, EventArgs e)
        {
            try
            {
                int portTest = int.Parse(portTextBox.Text.Trim());
            }
            catch
            {
                MessageBox.Show("Invalid port, try again.");
                return;
            }

            string IpText = ipTextBox.Text.Trim();
            if (string.IsNullOrEmpty(IpText) || IpText.Split('.').Length != 4)
            {
                MessageBox.Show("Invalid IP, try again.");
                return;
            }
            try
            {
                int Ip1Test = int.Parse(IpText.Split('.')[0]);
                int Ip2Test = int.Parse(IpText.Split('.')[1]);
                int Ip3Test = int.Parse(IpText.Split('.')[2]);
                int Ip4Test = int.Parse(IpText.Split('.')[3]);
            }
            catch
            {
                MessageBox.Show("Invalid IP, try again.");
                return;
            }
            int Ip1 = int.Parse(IpText.Split('.')[0]);
            int Ip2 = int.Parse(IpText.Split('.')[1]);
            int Ip3 = int.Parse(IpText.Split('.')[2]);
            int Ip4 = int.Parse(IpText.Split('.')[3]);
            int port = int.Parse(portTextBox.Text.Trim());
            if (port < 0 || port > 65535 || Ip1 < 0 || Ip1 > 255 || Ip2 < 0 || Ip2 > 255 || Ip3 < 0 || Ip3 > 255 || Ip4 < 0 || Ip4 > 255)
            {
                MessageBox.Show("Invalid port or IP, try again.");
                return;
            }
            // checked port and IP, now can start to connect
            try
            {
                Client.Start(IpText, port);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to connect: {ex.Message}");
                return;
            }
        }
        private void PrintMessage(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => MessageBox.Show(message)));
                return;
            }
            MessageBox.Show(message);
        }

        private void registerButton_Click(object? sender, EventArgs e)
        {
            if (!Client.SecureSessionConnected)
            {
                MessageBox.Show("Please connect to the server first.");
                return;
            }
            string username = usernameTextBox.Text.Trim();
            string password = passwordTextBox.Text.Trim();
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Username and password cannot be empty.");
                return;
            }
            try
            {
                Client.CreateRegisterFrame(username, password);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to register: {ex.Message}");
            }
        }

        private void loginButton_Click(object? sender, EventArgs e)
        {
            if (!Client.SecureSessionConnected)
            {
                MessageBox.Show("Please connect to the server first.");
                return;
            }
            string username = usernameTextBox.Text.Trim();
            string password = passwordTextBox.Text.Trim();
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Username and password cannot be empty.");
                return;
            }
            try
            {
                Client.CreateLoginFrame(username, password);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to login: {ex.Message}");
            }
        }
        private void DisconnectButton_Click(object? sender, EventArgs e)
        {
            if (!Client.TcpConnected)
            {
                MessageBox.Show("Not connected to the server.");
                return;
            }
            Client.Stop();
        }
        private void SendButton_Click(object? sender, EventArgs e)
        {
            if (!Client.TcpConnected)
            {
                MessageBox.Show("Not connected to the server.");
                return;
            }
            if (!Client.SecureSessionConnected)
            {
                MessageBox.Show("Please login or register first.");
                return;
            }
            if (string.IsNullOrEmpty(MessageTextBox.Text)){
                MessageBox.Show("Message cannot be empty.");
                return;
            }
            try
            {
                Client.CreateMessageFrame(MessageTextBox.Text.Trim());

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to send message: {ex.Message}");
            }
    }
        private void orderButton_Click(object? sender, EventArgs e)
        {
            if (!Client.TcpConnected)
            {
                DisplayLog("Not connected to the server.");
                return;
            }
            if(!Client.SecureSessionConnected)
            {
                DisplayLog("Please login or register first.");
                return;
            }
            if(cart.Count == 0)
            {
                DisplayLog("Cart is empty.");
                return;
            }
            if(!ValidationHelpers.ValidateCreditCardNumber(creditCardBox.Text.Trim()))
            {
                DisplayLog("Invalid credit card number.");
                return;
            }
            if(!ValidationHelpers.ValidateExpiration(monthBox.Text.Trim(), yearBox.Text.Trim()))
            {
                DisplayLog("Invalid expiration date.");
                return;
            }
            if(!ValidationHelpers.ValidateCvv(CvvBox.Text.Trim()))
            {
                DisplayLog("Invalid CVV.");
                return;
            }
            if(ValidationHelpers.ValidateNames(firstNameBox.Text.Trim()) || ValidationHelpers.ValidateNames(lastNameBox.Text.Trim()))
            {
                DisplayLog("Invalid first name or last name.");
                return;
            }
            if(ValidationHelpers.ValidateNames(addressBox.Text.Trim()))
            {
                DisplayLog("Invalid address.");
                return;
            }
            try
            {
                OrderDetails details = new OrderDetails()
                {
                    Username = Client.Username ?? "",
                    FirstName = firstNameBox.Text.Trim(),
                    LastName = lastNameBox.Text.Trim(),
                    Address = addressBox.Text.Trim(),
                    CreditCardNumber = creditCardBox.Text.Trim(),
                    ExpirationMonth = monthBox.Text.Trim(),
                    ExpirationYear = yearBox.Text.Trim(),
                    CVV = CvvBox.Text.Trim()
                };

                Client.CreateOrderFrame(details, cart);
            }
            catch (Exception ex)
            {
                DisplayLog($"Failed to place order: {ex.Message}");
            }
        }
        public void ClearStore()
        {
            if(storeGrid.InvokeRequired)
            {
                storeGrid.Invoke(new Action(() => storeGrid.Rows.Clear()));
                return;
            }
            storeGrid.Rows.Clear();
        }
        private void StoreGrid_CellContentClick(object? sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;// if the click is in an invalid area ignore its
            }
            if(e.ColumnIndex != 3)
            {
                return; // if the click is not in the add to cart button ignore it
            }
            string quantityText = storeGrid.Rows[e.RowIndex].Cells[2].Value?.ToString() ?? "";
            string priceText = storeGrid.Rows[e.RowIndex].Cells[1].Value?.ToString() ?? "";
            string productId = storeGrid.Rows[e.RowIndex].Cells[6].Value?.ToString() ?? "";
            string name = storeGrid.Rows[e.RowIndex].Cells[0].Value?.ToString() ?? "";
            if(!int.TryParse(quantityText, out int quantity) || !decimal.TryParse(priceText, out decimal price) || string.IsNullOrEmpty(productId) || string.IsNullOrEmpty(name))
            {
                DisplayLog("Invalid product information.");
                return;
            }
            if(quantity < 1)
            {
                DisplayLog("Quantity must be at least 1.");
                return;
            }
            ProductAndQuantity productAndQuantity = new ProductAndQuantity()
            {
                ProductID = productId,
                Name = name,
                Description = "",
                Price = price,
                Quantity = quantity
            };
            int qtyInCart = 0;
            for(int i = 0; i < cart.Count; i++)
            {
                if(productId == cart[i].ProductID)
                {
                    qtyInCart += cart[i].Quantity;
                    break;
                }
            }
            qtyInCart += quantity;
            for(int i = 0; i < cart.Count; i++)
            {
                if(productId == cart[i].ProductID)
                {
                    cart[i].Quantity += quantity;
                    DisplayLog($"Added {quantity} of {productAndQuantity.Name} to cart.");
                    updateCell(e.RowIndex, 5, qtyInCart.ToString());
                    updateCell(e.RowIndex, 4, (qtyInCart * price).ToString());
                    updateCell(e.RowIndex, 2, "");
                    return;
                }
            }
            cart.Add(productAndQuantity);
            DisplayLog($"Added {quantity} of {productAndQuantity.Name} to cart.");
            updateCell(e.RowIndex, 2, "");
            updateCell(e.RowIndex, 5, qtyInCart.ToString());
            updateCell(e.RowIndex, 4, (qtyInCart * price).ToString());

        }
        private void updateCell(int row, int column, string value)
        {
            if(row < 0 || row >= storeGrid.Rows.Count || column < 0 || column >= storeGrid.Columns.Count)
            {
                return;// checks if the row and column that are being updated are valid
            }
            if(column == 3)
            {
                return; // you cant change the add to cart button
            }
            storeGrid.Rows[row].Cells[column].Value = value;
        }
        public void AppendProductToStoreGrid(ProductWithDetails product)
        {
            if(storeGrid.InvokeRequired)
            {
                storeGrid.Invoke(new Action(() => storeGrid.Rows.Add(product.Name, product.Price, "", "Add to cart", 0, 0, product.ProductID)));
                return;
            }
            storeGrid.Rows.Add(product.Name, product.Price, "", "Add to cart", 0, 0, product.ProductID);
        }
        public void DisplayMessage(string username, string message)
        {
            if(ChatBox.InvokeRequired)
            {
                ChatBox.Invoke(new Action(() => ChatBox.AppendText($"{username}: {message}" + Environment.NewLine)));
                return;
            }
            ChatBox.AppendText($"{username}: {message}" + Environment.NewLine);
        }
        public void DisplayLog(string log)
        {
            if(logBox.InvokeRequired)
            {
                logBox.Invoke(new Action(() => logBox.AppendText($"{log}" + Environment.NewLine)));
                return;
            }
            logBox.AppendText($"{log}" + Environment.NewLine);
        }
        public void ClearMessages()
        {
            if(ChatBox.InvokeRequired)
            {
                ChatBox.Invoke(new Action(() => ChatBox.Clear()));
                return;
            }
            ChatBox.Clear();
}
    }
}
