using System;
using System.Collections.Generic;

namespace SharedLibraries.Payloads;

public partial class Product
{
    public string ProductID { get; set; } = string.Empty;
}

public partial class ProductWithDetails : Product
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; } = decimal.Zero;
}
public partial class ProductAndQuantity : ProductWithDetails
{
    public int Quantity { get; set; }
}


public partial class OrderDetails
{
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string CreditCardNumber { get; set; } = string.Empty;
    public string ExpirationMonth { get; set; } = string.Empty;
    public string ExpirationYear { get; set; } = string.Empty;
    public string CVV { get; set; } = string.Empty;
    public DateTime Timestamp {get; set;} = DateTime.Now;
}

public partial class Order
{
    public string OrderID { get; set; } = string.Empty;
    public List<ProductAndQuantity> Products { get; set; } = new List<ProductAndQuantity>();
    public OrderDetails Details { get; set; } = new OrderDetails();

    public decimal GetTotal()
    {
        decimal Total = 0;
        for(int i = 0; i < Products.Count; i++)
        {
            Total += Products[i].Quantity*Products[i].Price;
        }
        return Total;
    }
}

public sealed partial class SendProductsPayload
{
    public List<ProductWithDetails> Products { get; set; } = new List<ProductWithDetails>();
}

public sealed partial class PlaceOrderPayload
{
    public List<ProductAndQuantity> Products { get; set; } = new List<ProductAndQuantity>();
    public string Username { get; set; } = string.Empty;
    public OrderDetails Details { get; set; } = new OrderDetails();
}

public sealed partial class OrderResultPayload
{
    public bool Success { get; set; }
}
