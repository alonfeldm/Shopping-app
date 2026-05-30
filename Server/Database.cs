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
    private static readonly string DbPath = Path.Combine(Application.StartupPath, "Database.db");
    private static readonly string ConnectionString = $"Data Source={DbPath};Pooling=True;";


    public static void InitializeDB()
    {
        if (!File.Exists(DbPath))
        {
            SQLiteConnection.CreateFile(DbPath);
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
            "foreign key (ORDERID) references Orders(ORDERID));";
        using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
        using (SQLiteCommand command = new SQLiteCommand(CreateTablesQuery, connection))
        {
            connection.Open();
            command.ExecuteNonQuery();
        }

    }
    public static User? SelectUser(string username)
    {
        using (SQLiteConnection Connection = new SQLiteConnection(ConnectionString))
        using (SQLiteCommand Command = new SQLiteCommand("select * from Users where USERNAME=@username", Connection))
        {
            Command.Parameters.AddWithValue("@username", username);
            Connection.Open();

            using (SQLiteDataReader Reader = Command.ExecuteReader())
            {
                if (Reader.Read())
                {
                    User user = new User();
                    user.Username = Reader["USERNAME"].ToString() ?? "";
                    user.PasswordHash = Reader["PASSWORDHASH"].ToString() ?? "";
                    user.Salt = Reader["SALT"].ToString() ?? "";
                    return user;
                }
            }
        }

        return null;
    }
    public static void SaveUser(User user)
    {
        string Query = "insert into Users (USERNAME, PASSWORDHASH, SALT)" + "values (@username, @passwordHash, @salt)";
        using (SQLiteConnection Connection = new SQLiteConnection(ConnectionString))
        using (SQLiteCommand Command = new SQLiteCommand(Query, Connection))
        {
            Command.Parameters.AddWithValue("@username", user.Username);
            Command.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
            Command.Parameters.AddWithValue("@salt", user.Salt);
            Connection.Open();
            Command.ExecuteNonQuery();
        }
    }
    public static void SaveOrderItem(string OrderId, ProductAndQuantity product)
    {
        string Query = "insert into OrderItems (ORDERID, PRODUCTID, QUANTITY, PRICE)" + "values (@orderId, @productId, @quantity, @price)";
        using (SQLiteConnection Connection = new SQLiteConnection(ConnectionString))
        using (SQLiteCommand Command = new SQLiteCommand(Query, Connection))
        {
            Command.Parameters.AddWithValue("@orderId", OrderId);
            Command.Parameters.AddWithValue("@productId", product.ProductID);
            Command.Parameters.AddWithValue("@quantity", product.Quantity);
            Command.Parameters.AddWithValue("@price", product.Price);

            Connection.Open();
            Command.ExecuteNonQuery();
        }
    }
    public static void SaveOrderDetails(Order order)
    {
        string Query =
            "insert into Orders " +
            "(ORDERID, USERNAME, FIRSTNAME, LASTNAME, ADDRESS, CREDITCARDNUMBER, EXPIRATIONMONTH, EXPIRATIONYEAR, CVV, TIMESTAMP) " +
            "values (@orderId, @username, @firstName, @lastName, @address, @creditCardNumber, @expirationMonth, @expirationYear, @cvv, @timestamp)";

        using (SQLiteConnection Connection = new SQLiteConnection(ConnectionString))
        using (SQLiteCommand Command = new SQLiteCommand(Query, Connection))
        {
            Command.Parameters.AddWithValue("@orderId", order.OrderID);
            Command.Parameters.AddWithValue("@username", order.Details.Username);
            Command.Parameters.AddWithValue("@firstName", order.Details.FirstName);
            Command.Parameters.AddWithValue("@lastName", order.Details.LastName);
            Command.Parameters.AddWithValue("@address", order.Details.Address);
            Command.Parameters.AddWithValue("@creditCardNumber", order.Details.CreditCardNumber);
            Command.Parameters.AddWithValue("@expirationMonth", order.Details.ExpirationMonth);
            Command.Parameters.AddWithValue("@expirationYear", order.Details.ExpirationYear);
            Command.Parameters.AddWithValue("@cvv", order.Details.CVV);
            Command.Parameters.AddWithValue("@timestamp", order.Details.Timestamp);

            Connection.Open();
            Command.ExecuteNonQuery();
        }
    }
    public static void SaveFullOrder(Order order)
    {
        SaveOrderDetails(order);

        for (int i = 0; i < order.Products.Count; i++)
        {
            SaveOrderItem(order.OrderID, order.Products[i]);
        }
    }

    public static void SaveMessage(SharedLibraries.Payloads.Message message)
    {
        string Query = "insert into Messages (MESSAGEID, USERNAME, CONTENT, TIMESTAMP)" + "values (@messageId, @username, @content, @timestamp)";
        using (SQLiteConnection Connection = new SQLiteConnection(ConnectionString))
        using (SQLiteCommand Command = new SQLiteCommand(Query, Connection))
        {
            Command.Parameters.AddWithValue("@messageId", message.MessageId);
            Command.Parameters.AddWithValue("@username", message.SentBy);
            Command.Parameters.AddWithValue("@content", message.Text);
            Command.Parameters.AddWithValue("@timestamp", message.Timestamp);

            Connection.Open();
            Command.ExecuteNonQuery();
        }
    }
    public static List<SharedLibraries.Payloads.Message> GetAllMessages()
    {
        List<SharedLibraries.Payloads.Message> messages = new List<SharedLibraries.Payloads.Message>();
        using (SQLiteConnection Connection = new SQLiteConnection(ConnectionString))
        using (SQLiteCommand Command = new SQLiteCommand("select * from Messages", Connection))
        {
            Connection.Open();
            using (SQLiteDataReader Reader = Command.ExecuteReader())
            {
                while (Reader.Read())
                {
                    SharedLibraries.Payloads.Message message = new SharedLibraries.Payloads.Message();
                    message.MessageId = Reader["MESSAGEID"].ToString() ?? "";
                    message.SentBy = Reader["USERNAME"].ToString() ?? "";
                    message.Text = Reader["CONTENT"].ToString() ?? "";
                    DateTime.TryParse(Reader["TIMESTAMP"].ToString(), out DateTime time);
                    message.Timestamp = time;
                    messages.Add(message);
                }
            }
        }
        return messages;
    }
    public static List<ProductWithDetails> GetAllProducts()
    {
        List<ProductWithDetails> ProductList = new List<ProductWithDetails>();
        ProductList.Add(new ProductWithDetails
        {
            ProductID = "01",
            Name = "Campus Laptop Sleeve",
            Description = "Protective 14-inch sleeve for school and office use.",
            Price = 79.90m
        });

        ProductList.Add(new ProductWithDetails
        {
            ProductID = "02",
            Name = "Mechanical Keyboard",
            Description = "Compact keyboard with a comfortable typing feel.",
            Price = 229.00m
        });

        ProductList.Add(new ProductWithDetails
        {
            ProductID = "03",
            Name = "USB-C Dock",
            Description = "Seven-port dock with HDMI, ethernet, and USB expansion.",
            Price = 189.50m
        });
        ProductList.Add(new ProductWithDetails
        {
            ProductID = "04",
            Name = "Wireless Mouse",
            Description = "Ergonomic mouse with adjustable DPI settings.",
            Price = 49.99m
        });
        ProductList.Add(new ProductWithDetails
        {
            ProductID = "05",
            Name = "Noise-Cancelling Headphones",
            Description = "Over-ear headphones with active noise cancellation.",
            Price = 199.99m
        });
        ProductList.Add(new ProductWithDetails
        {
            ProductID = "06",
            Name = "4K 27in Monitor",
            Description = "27-inch monitor with stunning 4K resolution.",
            Price = 349.99m
        });
        ProductList.Add(new ProductWithDetails
        {
            ProductID = "07",
            Name = "External SSD 1TB",
            Description = "Portable 1TB SSD with fast data transfer speeds.",
            Price = 149.99m
        });
        ProductList.Add(new ProductWithDetails
        {
            ProductID = "08",
            Name = "Webcam with Microphone",
            Description = "1080p webcam with built-in microphone for clear video calls.",
            Price = 89.99m
        });
        ProductList.Add(new ProductWithDetails
        {
            ProductID = "09",
            Name = "Laptop Stand",
            Description = "Adjustable stand to improve laptop ergonomics.",
            Price = 39.99m
        });
        ProductList.Add(new ProductWithDetails
        {
            ProductID = "10",
            Name = "Bluetooth Speaker",
            Description = "Portable speaker with rich sound and long battery life.",
            Price = 59.99m
        });

        // Return the full list.
        return ProductList;
    }
    public void ClearMessages()
    {
        string Query = "DELETE FROM Messages";
        using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
        {
            using (SQLiteCommand Command = new SQLiteCommand(Query, connection))
            {
                connection.Open();
                Command.ExecuteNonQuery();
            }
        }
    }
    public void ClearUsers()
    {
        string Query = "DELETE FROM Users";
        using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
        {
            using (SQLiteCommand Command = new SQLiteCommand(Query, connection))
            {
                connection.Open();
                Command.ExecuteNonQuery();
            }
        }

    }
    public void ClearOrders()
    {
        string Query = "DELETE FROM Orders";
        using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
        {
            using (SQLiteCommand Command = new SQLiteCommand(Query, connection))
            {
                connection.Open();
                Command.ExecuteNonQuery();
            }
        }
    }
    public static int GetItemPrice(int ProductId)
    {
        for(int i = 0; i < GetAllProducts().Count; i++)
        {
            if(GetAllProducts()[i].ProductID == ProductId.ToString())
            {
                return int.Parse(GetAllProducts()[i].Price.ToString());
            }
        }
        return -1;
    }
    public static bool ProductExists(string ProductId)
    {
        foreach (ProductWithDetails product in GetAllProducts())
        {
            if (product.ProductID == ProductId)
            {
                return true;
            }
        }
        return false;
    }
}