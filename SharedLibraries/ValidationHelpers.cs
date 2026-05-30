using SharedLibraries.Payloads;
using System.Collections.Generic;
namespace SharedLibraries.ValidationHelpers;

public class ValidationHelpers
{
    public static bool ValidateCreditCardNumber(string CreditCardNumber)
    {
        if (CreditCardNumber.Length != 16 || string.IsNullOrEmpty(CreditCardNumber))
        {
            return false;
        }
        try
        {
            long CreditCardNumberLong = long.Parse(CreditCardNumber);
            if (CreditCardNumberLong < 0)
            {
                return false;
            }
            return true;
        }
        catch
        {
            return false;
        }
    }
    public static bool ValidateExpiration(string month, string year)
    {
        if (month.Length != 2 || year.Length != 2 || string.IsNullOrEmpty(month) || string.IsNullOrEmpty(year))
        {
            return false;
        }
        try
        {
            int monthInt = int.Parse(month);
            int yearInt = int.Parse(year);
            if (monthInt < 1 || monthInt > 12 || yearInt < 26 || yearInt > 99)
            {
                return false;
            }
            return true;
        }
        catch
        {
            return false;
        }
    }
    public static bool ValidateCvv(string cvv)
    {
        if (cvv.Length != 3 || string.IsNullOrEmpty(cvv))
        {
            return false;
        }
        try
        {
            int cvvInt = int.Parse(cvv);
            if (cvvInt < 0)
            {
                return false;
            }
            return true;
        }
        catch
        {
            return false;
        }
    }
    public static bool ValidateNames(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return false;
        }
        return true;
    }
    public static bool ValidateProducts(List<ProductAndQuantity> products)
    {
        foreach (ProductAndQuantity product in products)
        {
            if (string.IsNullOrEmpty(product.ProductID))
            {
                return false;
            }
            if(product.Quantity < 1)
            {
                return false;
            }
        }
        return true;
    }
    public static bool ValidateUsername(string username)
    {
        if(string.IsNullOrEmpty(username) || username.Length < 3 || username.Length > 20)
        {
            return false;
        }
        return true;
    }
    public static bool ValidatePassword(string password)
    {
        if (string.IsNullOrEmpty(password) || password.Length < 4 || password.Length > 25)
        {
            return false;
        }
        return true;
    }
}