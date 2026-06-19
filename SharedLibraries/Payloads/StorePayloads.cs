using System;
using System.Collections.Generic;

namespace SharedLibraries.Payloads;

public partial class Product
{
    public string ProductID { get; set; } = string.Empty;// the basic need
}

public partial class ProductWithDetails : Product
{
    public string Name { get; set; } = string.Empty;// a name for the store
    public string Description { get; set; } = string.Empty;// might get used by the store
    public decimal Price { get; set; } = decimal.Zero;// a price for the store
}
public partial class ProductAndQuantity : ProductWithDetails
{
    public int Quantity { get; set; }// adds a quantity field for the shopping cart, not needed by the store
}


public partial class OrderDetails// personal and payment info
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
    public string OrderID { get; set; } = string.Empty;// differentiating id
    public List<ProductAndQuantity> Products { get; set; } = new List<ProductAndQuantity>();// list of products bought
    public OrderDetails Details { get; set; } = new OrderDetails();// personal and payment info

    public decimal GetTotal()// used to get the total price
    {
        decimal Total = 0;
        for(int i = 0; i < Products.Count; i++)
        {
            Total += Products[i].Quantity*Products[i].Price;
        }
        return Total;
    }
}

public sealed partial class SendProductsPayload// contains the current products, obsolete
{
    public List<ProductWithDetails> Products { get; set; } = new List<ProductWithDetails>();
}

public sealed partial class PlaceOrderPayload// the payload sent by the user when buying, contains the cart and payment and personal info needed
{
    public List<ProductAndQuantity> Products { get; set; } = new List<ProductAndQuantity>();
    public OrderDetails Details { get; set; } = new OrderDetails();
}

public sealed partial class OrderResultPayload// the result of the order to be displayed to the user.
{
    public bool Success { get; set; }
}
