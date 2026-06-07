using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using SharedLibraries;
using SharedLibraries.Payloads;

namespace Server;

internal sealed class Database
{
    private static readonly string DbPath = Path.Combine(Application.StartupPath, "Database.db");// the path to where the database exists
    private static readonly string ConnectionString = $"Data Source={DbPath};Pooling=True;";


    public static void InitializeDB()// the builder
    {
        if (!File.Exists(DbPath))
        {
            SQLiteConnection.CreateFile(DbPath);// if theres no db file in the path then a new file is created
        }
        string CreateTablesQuery =
            "create table if not exists Users (" +
            "USERNAME text primary key," +
            "PASSWORDHASH text," +
            "SALT text);" +

            "create table if not exists Messages (" +
            "MESSAGEID text primary key," +
            "USERNAME text," +
            "CONTENT text," +
            "TIMESTAMP text);" +

            "create table if not exists Orders (" +
            "ORDERID text primary key," +
            "USERNAME text," +
            "FIRSTNAME text," +
            "LASTNAME text," +
            "ADDRESS text," +
            "CREDITCARDNUMBER text," +
            "EXPIRATIONMONTH text," +
            "EXPIRATIONYEAR text," +
            "CVV text," +
            "TIMESTAMP text);" +

            "create table if not exists OrderItems (" +
            "ORDERITEMID integer primary key autoincrement," +
            "ORDERID text," +
            "PRODUCTID text," +
            "QUANTITY integer," +
            "PRICE real," +
            "foreign key (ORDERID) references Orders(ORDERID));";// the query to create the tables, if a file exists it wont destroy the stored data
        using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
        using (SQLiteCommand command = new SQLiteCommand(CreateTablesQuery, connection))
        {
            connection.Open();// opens the connection
            command.ExecuteNonQuery();// executes the query and automatically closes the connection
        }

    }
    public static User? SelectUser(string username)// used to find if a user is valid and for its details
    {
        using (SQLiteConnection Connection = new SQLiteConnection(ConnectionString))
        using (SQLiteCommand Command = new SQLiteCommand("select * from Users where USERNAME=@username", Connection))
        {
            Command.Parameters.AddWithValue("@username", username);
            Connection.Open();

            using (SQLiteDataReader Reader = Command.ExecuteReader())// executes the command and reads the data 
            {
                if (Reader.Read())
                {
                    User user = new User();// creates a new user to fill with the new data
                    user.Username = Reader["USERNAME"].ToString() ?? "";
                    user.PasswordHash = Reader["PASSWORDHASH"].ToString() ?? "";
                    user.Salt = Reader["SALT"].ToString() ?? "";
                    return user;// returns the new user
                }
            }
        }

        return null;//if the reader couldnt find data that matches the query then the user doesnt exist and returns null
    }
    public static void SaveUser(User user)
    {
        string Query = "insert into Users (USERNAME, PASSWORDHASH, SALT)" + "values (@username, @passwordHash, @salt)";
        using (SQLiteConnection Connection = new SQLiteConnection(ConnectionString))
        using (SQLiteCommand Command = new SQLiteCommand(Query, Connection))
        {
            Command.Parameters.AddWithValue("@username", user.Username);// fills the query with the users fields 
            Command.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
            Command.Parameters.AddWithValue("@salt", user.Salt);
            Connection.Open();// opens the connection
            Command.ExecuteNonQuery();// executes the query and automatically closes the connection
        }
    }
    public static void SaveOrderItem(string OrderId, ProductAndQuantity product)// sued to save to the order table
    {
        string Query = "insert into OrderItems (ORDERID, PRODUCTID, QUANTITY, PRICE)" + "values (@orderId, @productId, @quantity, @price)";
        using (SQLiteConnection Connection = new SQLiteConnection(ConnectionString))
        using (SQLiteCommand Command = new SQLiteCommand(Query, Connection))
        {
            Command.Parameters.AddWithValue("@orderId", OrderId);// fills the query with the orders fields 
            Command.Parameters.AddWithValue("@productId", product.ProductID);
            Command.Parameters.AddWithValue("@quantity", product.Quantity);
            Command.Parameters.AddWithValue("@price", product.Price);
            Connection.Open();// opens the connection
            Command.ExecuteNonQuery();// executes the query and automatically closes the connection
        }
    }
    public static void SaveOrderDetails(Order order)// used to save to the orderDetails table
    {
        string Query =
            "insert into Orders " +
            "(ORDERID, USERNAME, FIRSTNAME, LASTNAME, ADDRESS, CREDITCARDNUMBER, EXPIRATIONMONTH, EXPIRATIONYEAR, CVV, TIMESTAMP) " +
            "values (@orderId, @username, @firstName, @lastName, @address, @creditCardNumber, @expirationMonth, @expirationYear, @cvv, @timestamp)";

        using (SQLiteConnection Connection = new SQLiteConnection(ConnectionString))
        using (SQLiteCommand Command = new SQLiteCommand(Query, Connection))
        {
            Command.Parameters.AddWithValue("@orderId", order.OrderID);// fills the query with the orders fields 
            Command.Parameters.AddWithValue("@username", order.Details.Username);
            Command.Parameters.AddWithValue("@firstName", order.Details.FirstName);
            Command.Parameters.AddWithValue("@lastName", order.Details.LastName);
            Command.Parameters.AddWithValue("@address", order.Details.Address);
            Command.Parameters.AddWithValue("@creditCardNumber", order.Details.CreditCardNumber);
            Command.Parameters.AddWithValue("@expirationMonth", order.Details.ExpirationMonth);
            Command.Parameters.AddWithValue("@expirationYear", order.Details.ExpirationYear);
            Command.Parameters.AddWithValue("@cvv", order.Details.CVV);
            Command.Parameters.AddWithValue("@timestamp", order.Details.Timestamp);
            Connection.Open();// opens the connection
            Command.ExecuteNonQuery();// executes the query and automatically closes the connection
        }
    }
    public static void SaveFullOrder(Order order)// loops the saveorderitem function on the order
    {
        SaveOrderDetails(order);

        for (int i = 0; i < order.Products.Count; i++)
        {
            SaveOrderItem(order.OrderID, order.Products[i]);
        }
    }

    public static void SaveMessage(SharedLibraries.Payloads.Message message)//used to save to the messages table
    {
        string Query = "insert into Messages (MESSAGEID, USERNAME, CONTENT, TIMESTAMP)" + "values (@messageId, @username, @content, @timestamp)";
        using (SQLiteConnection Connection = new SQLiteConnection(ConnectionString))
        using (SQLiteCommand Command = new SQLiteCommand(Query, Connection))
        {
            Command.Parameters.AddWithValue("@messageId", message.MessageId);// fills the query with the orders fields 
            Command.Parameters.AddWithValue("@username", message.SentBy);
            Command.Parameters.AddWithValue("@content", message.Text);
            Command.Parameters.AddWithValue("@timestamp", message.Timestamp);
            Connection.Open();// opens the connection
            Command.ExecuteNonQuery();// executes the query and automatically closes the connection
        }
    }
    public static List<SharedLibraries.Payloads.Message> GetAllMessages()// used to get all of the messages stored in the messages table
    {
        List<SharedLibraries.Payloads.Message> messages = new List<SharedLibraries.Payloads.Message>();// create a list to hold all of the messages
        using (SQLiteConnection Connection = new SQLiteConnection(ConnectionString))
        using (SQLiteCommand Command = new SQLiteCommand("select * from Messages", Connection))
        {
            Connection.Open();
            using (SQLiteDataReader Reader = Command.ExecuteReader())
            {
                while (Reader.Read())// if the reader could read anything
                {
                    SharedLibraries.Payloads.Message message = new SharedLibraries.Payloads.Message();
                    message.MessageId = Reader["MESSAGEID"].ToString() ?? "";// input all of the read data
                    message.SentBy = Reader["USERNAME"].ToString() ?? "";
                    message.Text = Reader["CONTENT"].ToString() ?? "";
                    DateTime.TryParse(Reader["TIMESTAMP"].ToString(), out DateTime time);
                    message.Timestamp = time;
                    messages.Add(message);// add the message to the list
                }
            }
        }
        return messages;// return the messages found, if none were found then the list is already empty
    }
    public static List<ProductWithDetails> GetAllProducts()// function that creates and returns the product list
    {
        List<ProductWithDetails> ProductList = new List<ProductWithDetails>();// hard coded product list, arbitrary pricing
        ProductList.Add(new ProductWithDetails
        {
            ProductID = "01",
            Name = "Campus Laptop Sleeve",
            Description = "Protective 14-inch sleeve for school and office use.",
            Price = 80m
        });

        ProductList.Add(new ProductWithDetails
        {
            ProductID = "02",
            Name = "Mechanical Keyboard",
            Description = "Compact keyboard with a comfortable typing feel.",
            Price = 230m
        });

        ProductList.Add(new ProductWithDetails
        {
            ProductID = "03",
            Name = "USB-C Dock",
            Description = "Seven-port dock with HDMI, ethernet, and USB expansion.",
            Price = 185m
        });
        ProductList.Add(new ProductWithDetails
        {
            ProductID = "04",
            Name = "Wireless Mouse",
            Description = "Ergonomic mouse with adjustable DPI settings.",
            Price = 50m
        });
        ProductList.Add(new ProductWithDetails
        {
            ProductID = "05",
            Name = "Noise-Cancelling Headphones",
            Description = "Over-ear headphones with active noise cancellation.",
            Price = 200m
        });
        ProductList.Add(new ProductWithDetails
        {
            ProductID = "06",
            Name = "4K 27in Monitor",
            Description = "27-inch monitor with stunning 4K resolution.",
            Price = 350m
        });
        ProductList.Add(new ProductWithDetails
        {
            ProductID = "07",
            Name = "External SSD 1TB",
            Description = "Portable 1TB SSD with fast data transfer speeds.",
            Price = 150m
        });
        ProductList.Add(new ProductWithDetails
        {
            ProductID = "08",
            Name = "Webcam with Microphone",
            Description = "1080p webcam with built-in microphone for clear video calls.",
            Price = 90m
        });
        ProductList.Add(new ProductWithDetails
        {
            ProductID = "09",
            Name = "Laptop Stand",
            Description = "Adjustable stand to improve laptop ergonomics.",
            Price = 40m
        });
        ProductList.Add(new ProductWithDetails
        {
            ProductID = "10",
            Name = "Bluetooth Speaker",
            Description = "Portable speaker with rich sound and long battery life.",
            Price = 60m
        });

        return ProductList;// returns the product list
    }
    public static void ClearMessages()//to allow the server host to wipe sent messages
    {
        string Query = "DELETE FROM Messages";// the query to delete values in the messages table
        using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
        {
            using (SQLiteCommand Command = new SQLiteCommand(Query, connection))
            {
                connection.Open(); // opens a connection to the database
                Command.ExecuteNonQuery();// executes the query to delete, automatically closes the connection after
            }
        }
    }
    public static void ClearUsers()// to allow the server host to wipe the users
    {
        string Query = "DELETE FROM Users";// the query to delete values in the users table
        using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
        {
            using (SQLiteCommand Command = new SQLiteCommand(Query, connection))
            {
                connection.Open();// opens a connection to the database
                Command.ExecuteNonQuery();// executes the query to delete, automatically closes the connection after
            }
        }

    }
    public static void ClearOrders()// to allow the server host to wipe the orders(the order table and order items tables)
    {
        string OrderQuery = "DELETE FROM Orders";// the query to delete values in the orders table
        string OrderItemsQuery = "DELETE FROM OrderItems"; // the query to delete values in the orderitems table
        using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
        {
            using (SQLiteCommand Command = new SQLiteCommand(OrderItemsQuery, connection))
            {
                connection.Open();// opens a connection to the database
                Command.ExecuteNonQuery();// executes the command to wipe orderitems
                connection.Close();// because there is a second query with the same connection i need to close the connection and reopen for the next query
            }
            using (SQLiteCommand Command = new SQLiteCommand(OrderQuery, connection))
            {
                connection.Open();// opens a connection to the database
                Command.ExecuteNonQuery();// executes the command to wipe orders, automatically closes the connection
            }
        }
    }
    public static int GetItemPrice(string ProductId)
    {
        List<ProductWithDetails>? Allproducts = GetAllProducts();
        for (int i = 0; i < Allproducts.Count; i++)// loops on the products
        {
            if (Allproducts[i].ProductID == ProductId.ToString())// checks if a product with a matching productID exists
            {
                return int.Parse(Allproducts[i].Price.ToString());// returns the price of the item with a matching productID
            }
        }
        return -1;// if the item doesnt exist returns a price that is obviousley invalid as the store wont sell in negative prices
    }
    public static bool ProductExists(string ProductId)// to check if the productID send by the user is valid
    {
        List<ProductWithDetails>? Allproducts = GetAllProducts();
        foreach (ProductWithDetails product in Allproducts)// checks if a product exists with the same productID as the one sent by the user
        {
            if (product.ProductID == ProductId)
            {
                return true;
            }
        }
        return false;// if a matching productID wasnt found in the loop, that means the productID sent by the user was invalid
    }
}