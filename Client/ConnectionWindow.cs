using System;
using System.Windows.Forms;
using SharedLibraries.Payloads;
using System.Collections.Generic;
using SharedLibraries.ValidationHelpers;

namespace Client
{
    public partial class ConnectionWindow : Form
    {
        public List<ProductAndQuantity> cart = new List<ProductAndQuantity>(); // the cart that holds the products
        private Client Client { get; set; } = new Client();// the client that handles the connection and communication with the server
        public ConnectionWindow()
        {
            InitializeComponent();
            //Client.PrintOut += PrintMessage;// wires the functions to the events in client.cs
            Client.DisplayMessage += DisplayMessage;
            Client.AppendStore += AppendProductToStoreGrid;
            Client.ClearStore += ClearStore;
            Client.ClearMessages += ClearMessages;
            Client.DisplayLog += DisplayLog;
            Client.ClearLogs += ClearLogs;
        }
        public void ClearDetails()// resets the fields and the cart after an order is placed
        {
            firstNameBox.Clear();
            lastNameBox.Clear();
            addressBox.Clear();
            creditCardBox.Clear();
            monthBox.Clear();
            yearBox.Clear();
            CvvBox.Clear();
            ClearStoreColumns();
            cart = new List<ProductAndQuantity>();
        }

        private void ConnectButton_Click(object? sender, EventArgs e)
        {
            try
            {
                int portTest = int.Parse(portTextBox.Text.Trim());// trys to convert the port to an int
            }
            catch
            {
                DisplayLog("Invalid port, try again.");
                return;
            }

            string IpText = ipTextBox.Text.Trim();// gets the IP
            if (string.IsNullOrEmpty(IpText) || IpText.Split('.').Length != 4)// if the IP is empty or doesn't have 4 parts separated by dots, its invalid
            {
                DisplayLog("Invalid IP, try again.");
                return;
            }
            try
            {
                int Ip1Test = int.Parse(IpText.Split('.')[0]);
                int Ip2Test = int.Parse(IpText.Split('.')[1]);
                int Ip3Test = int.Parse(IpText.Split('.')[2]);
                int Ip4Test = int.Parse(IpText.Split('.')[3]);// trys to convert the 4 parts of the IP to ints
            }
            catch
            {
                DisplayLog("Invalid IP, try again.");
                return;
            }
            int Ip1 = int.Parse(IpText.Split('.')[0]);
            int Ip2 = int.Parse(IpText.Split('.')[1]);
            int Ip3 = int.Parse(IpText.Split('.')[2]);
            int Ip4 = int.Parse(IpText.Split('.')[3]);
            int port = int.Parse(portTextBox.Text.Trim());
            if (port < 0 || port > 65535 || Ip1 < 0 || Ip1 > 255 || Ip2 < 0 || Ip2 > 255 || Ip3 < 0 || Ip3 > 255 || Ip4 < 0 || Ip4 > 255)
            { // checks if the port and the 4 parts of the IP are in their valid ranges
                DisplayLog("Invalid port or IP, try again.");
                return;
            }
            // checked port and IP, now can start to connect
            try
            {
                Client.Start(IpText, port);
            }
            catch (Exception ex)//something failed, log it
            {
                DisplayLog($"Failed to connect: {ex.Message}");
                return;
            }
        }
        private void PrintMessage(string Message)//this if used to display with the messagebox
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => MessageBox.Show(Message)));
                return;// if the call is from a different thread we invoke it with the UI thread
            }
            MessageBox.Show(Message);
        }

        private void RegisterButton_Click(object? sender, EventArgs e)
        {
            if (!Client.SecureSessionConnected || !Client.TcpConnected) // cant register if the connection isnt fully established
            {
                DisplayLog("Please connect to the server first.");
                return;
            }
            if (Client.LoggedIn) // cant register if already logged in
            {
                DisplayLog("Already logged in.");
                return;
            }
            string username = usernameTextBox.Text.Trim();
            string password = passwordTextBox.Text.Trim();
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                DisplayLog("Username and password cannot be empty.");
                return;
            }
            try
            {
                Client.CreateRegisterFrame(username, password);//will create and send a register frame 
            }
            catch (Exception ex)
            {
                DisplayLog($"Failed to register: {ex.Message}");
            }
        }

        private void LoginButton_Click(object? sender, EventArgs e)
        {
            if (!Client.SecureSessionConnected || !Client.TcpConnected) // cant login if the connection isnt fully established
            {
                DisplayLog("Please connect to the server first.");
                return;
            }
            if (Client.LoggedIn)// cant login if already logged in
            {
                DisplayLog("Already logged in.");
                return;
            }
            string username = usernameTextBox.Text.Trim();
            string password = passwordTextBox.Text.Trim();
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                DisplayLog("Username and password cannot be empty.");
                return;
            }
            try
            {
                Client.CreateLoginFrame(username, password);// will create and send a login frame
            }
            catch (Exception ex)
            {
                DisplayLog($"Failed to login: {ex.Message}");
            }
        }
        private void DisconnectButton_Click(object? sender, EventArgs e)
        {
            if (!Client.TcpConnected)//cant disconnect if not connected
            {
                DisplayLog("Not connected to the server.");
                return;
            }
            Client.Stop();
            ClearLogs();
            ClearMessages();
            ClearStore();
            DisplayLog("Disconnected from the server.");
        }
        private void SendButton_Click(object? sender, EventArgs e)
        {
            if (!Client.TcpConnected)//cant send if not connected
            {
                DisplayLog("Not connected to the server.");
                return;
            }
            if (!Client.LoggedIn)//cant send if the user isnt logged in
            {
                DisplayLog("Please login or register first.");
                return;
            }
            if (string.IsNullOrEmpty(MessageTextBox.Text))
            {
                DisplayLog("Message cannot be empty.");
                return;
            }
            try
            {
                Client.CreateMessageFrame(MessageTextBox.Text.Trim());

            }
            catch (Exception ex)
            {
                DisplayLog($"Failed to send message: {ex.Message}");
            }
        }
        private void OrderButton_Click(object? sender, EventArgs e)
        {
            if (!Client.TcpConnected)//cant place order if not connected
            {
                DisplayLog("Not connected to the server.");
                return;
            }
            if (!Client.LoggedIn)//cant place order if the user isnt logged in
            {
                DisplayLog("Please login or register first.");
                return;
            }
            if (cart.Count == 0)//cant place order if the cart is empty
            {
                DisplayLog("Cart is empty.");
                return;
            }
            if (!ValidationHelpers.ValidateCreditCardNumber(creditCardBox.Text.Trim()))//validates the credit card number
            {
                DisplayLog("Invalid credit card number.");
                return;
            }
            if (!ValidationHelpers.ValidateExpiration(monthBox.Text.Trim(), yearBox.Text.Trim()))// validates the expiration date
            {
                DisplayLog("Invalid expiration date.");
                return;
            }
            if (!ValidationHelpers.ValidateCvv(CvvBox.Text.Trim()))// validates the CVV
            {
                DisplayLog("Invalid CVV.");
                return;
            }
            if (!ValidationHelpers.ValidateNames(firstNameBox.Text.Trim()) || !ValidationHelpers.ValidateNames(lastNameBox.Text.Trim()))// validates the first and last name
            {
                DisplayLog("Invalid first name or last name.");
                return;
            }
            if (!ValidationHelpers.ValidateNames(addressBox.Text.Trim()))// validates the address
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
                };// creates an object for the order details and fills the fields

                Client.CreateOrderFrame(details, cart);// creates and sends an order frame with the order details and the cart
                ClearDetails();// clears the details and the cart after placing the order

            }
            catch (Exception ex)
            {
                DisplayLog($"Failed to place order: {ex.Message}");
            }
        }
        public void ClearStore()// clears the store grid
        {
            if (storeGrid.InvokeRequired)
            {
                storeGrid.Invoke(new Action(() => storeGrid.Rows.Clear()));
                return;// if the call is from a different thread we invoke it with the UI thread
            }
            storeGrid.Rows.Clear();
        }
        private void StoreGrid_CellContentClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;// if the click is in an invalid area ignore its
            }
            if (e.ColumnIndex != 3)
            {
                return; // if the click is not in the add to cart button ignore it
            }
            string quantityText = storeGrid.Rows[e.RowIndex].Cells[2].Value?.ToString() ?? "";//reads the quantity
            string priceText = storeGrid.Rows[e.RowIndex].Cells[1].Value?.ToString() ?? "";// reads the price
            string productId = storeGrid.Rows[e.RowIndex].Cells[6].Value?.ToString() ?? "";// reads the product id from the hidden column
            string name = storeGrid.Rows[e.RowIndex].Cells[0].Value?.ToString() ?? "";// reads the name of the product
            if (!int.TryParse(quantityText, out int quantity) || !decimal.TryParse(priceText, out decimal price) || string.IsNullOrEmpty(productId) || string.IsNullOrEmpty(name))// validates the quantity, price, product id and name
            {
                DisplayLog("Invalid product information.");
                return;
            }
            if (quantity < 1)
            {
                DisplayLog("Quantity must be at least 1.");
                return;
            }
            ProductAndQuantity productAndQuantity = new ProductAndQuantity()// puts the data in a product and quantity object
            {
                ProductID = productId,
                Name = name,
                Description = "",
                Price = price,
                Quantity = quantity
            };
            for (int i = 0; i < cart.Count; i++)// goes over the cart to see if the product is already there
            {
                if (productId == cart[i].ProductID)//if its there
                {
                    cart[i].Quantity += quantity;// add to the quantity
                    DisplayLog($"Added {quantity} of {productAndQuantity.Name} to cart.");//log it
                    UpdateCell(e.RowIndex, 2, "");//clear the quantity cell
                    UpdateCell(e.RowIndex, 4, (cart[i].Quantity * price).ToString());//update the total price of the product
                    UpdateCell(e.RowIndex, 5, cart[i].Quantity.ToString());//update the quantity of that product in the cart
                    return;//now the product is added and we can end the function
                }
            }
            cart.Add(productAndQuantity);// if the product wasnt in the cart add it
            DisplayLog($"Added {quantity} of {productAndQuantity.Name} to cart.");// log it
            UpdateCell(e.RowIndex, 2, "");// clear the quantity cell
            UpdateCell(e.RowIndex, 4, (quantity * price).ToString()); // update the total price for that product
            UpdateCell(e.RowIndex, 5, quantity.ToString()); // update the quantity of that product in the cart

        }
        private void UpdateCell(int row, int column, string value)//replace the current value of a cell with a new one
        {
            if (row < 0 || row >= storeGrid.Rows.Count || column < 0 || column >= storeGrid.Columns.Count)
            {
                return;// checks if the row and column that are being updated are valid
            }
            if (column == 3)
            {
                return; // you cant change the add to cart button
            }
            storeGrid.Rows[row].Cells[column].Value = value;// does the change
        }
        public void AppendProductToStoreGrid(ProductWithDetails product)//adds a row of a product
        {
            if (storeGrid.InvokeRequired)
            {
                storeGrid.Invoke(new Action(() => storeGrid.Rows.Add(product.Name, product.Price, "", "Add to cart", 0, 0, product.ProductID)));
                return;//if the call is from a different thread we invoke it with the UI thread
            }
            storeGrid.Rows.Add(product.Name, product.Price, "", "Add to cart", 0, 0, product.ProductID);
        }
        public void DisplayMessage(string username, string message)// adds a message to the chatbox
        {
            if (ChatBox.InvokeRequired)
            {
                Invoke(new Action<string, string>(DisplayMessage), username, message);
                return;// if the call is from a different thread we invoke it with the UI thread
            }
            ChatBox.AppendText("(" + DateTime.Now.Hour +":"+ DateTime.Now.Minute +":"+ DateTime.Now.Second +"): "+ username + ": " +message+ System.Environment.NewLine);
        }
        public void DisplayLog(string log)// adds a log to the log box
        {
            if (logBox.InvokeRequired)
            {
                Invoke(new Action<string>(DisplayLog),log);
                return;// if the call is from a different thread we invoke it with the UI thread
            }
            logBox.AppendText("(" + DateTime.Now.Hour +":"+ DateTime.Now.Minute +":"+ DateTime.Now.Second +"): " + log + System.Environment.NewLine);
        }
        public void ClearMessages()// clears the chatbox
        {
            if (ChatBox.InvokeRequired)
            {
                ChatBox.Invoke(new Action(() => ChatBox.Clear()));
                return;// if the call is from a different thread we invoke it with the UI thread
            }
            ChatBox.Clear();
        }
        public void ClearLogs()// clears the log box
        {
            if (logBox.InvokeRequired)
            {
                logBox.Invoke(new Action(() => logBox.Clear()));
                return;// if the call is from a different thread we invoke it with the UI thread
            }
            logBox.Clear();
        }
        public void ClearStoreColumns()
        {
            if (InvokeRequired)// if the call is from a different thread we invoke it with the UI thread
            {
                Invoke(new Action(ClearStoreColumns));
                return;
            }
            foreach(DataGridViewRow row in storeGrid.Rows)// goes over all of the rows
            {
                row.Cells[2].Value = "";// resets the amount to add to cart
                row.Cells[4].Value = 0;// resets the total
                row.Cells[5].Value = 0;// resets the quantity in cart
            }
        }
    }
}
