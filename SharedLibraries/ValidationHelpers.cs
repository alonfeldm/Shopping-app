using SharedLibraries.Payloads;// product and quantity object
using System.Collections.Generic;
using System.ComponentModel;// for lists
namespace SharedLibraries.ValidationHelpers;

public class ValidationHelpers
{
    public static bool ValidateCreditCardNumber(string CreditCardNumber)
    // helper to check if the credit card number is valid
    //it only checks if its a 16 digit positive number, i dont use a security digit because id need to prepare credit card number for checks
    {
        if (CreditCardNumber.Length != 16 || string.IsNullOrEmpty(CreditCardNumber))// checks length and if its empty
        {
            return false;
        }
        try
        {
            long CreditCardNumberLong = long.Parse(CreditCardNumber);
            if (CreditCardNumberLong < 0)// if the number is negative its not valid
            {
                return false;
            }
            return true;
        }
        catch// if the try parse failed that means its not a number
        {
            return false;
        }
    }
    public static bool ValidateExpiration(string month, string year)// helper to validate the expiration date of the credit card
    {
        if (month.Length != 2 || year.Length != 2 || string.IsNullOrEmpty(month) || string.IsNullOrEmpty(year))// checks if the month and year are empty or not 2 letters
        {
            return false;
        }
        try
        {
            int monthInt = int.Parse(month);
            int yearInt = int.Parse(year);
            if (monthInt < 1 || monthInt > 12 || yearInt < 26 || yearInt > 99)// checks if the credit card year and month are valid, month in the range of 1 to 12
            //and year has to be between the current year and 2099
            {
                return false;
            }
            return true;
        }
        catch// if the try failed that means the month or the year are not integers and then they are invalid
        {
            return false;
        }
    }
    public static bool ValidateCvv(string cvv)// helper to validate the cvv
    {
        if (cvv.Length != 3 || string.IsNullOrEmpty(cvv))// checks if the length isnt three letters or if its empty or null
        {
            return false;
        }
        try
        {
            int cvvInt = int.Parse(cvv);
            if (cvvInt < 0)// if the cvv is negative its not valid, its definitely 999 or less beacuse of the length of three
            {
                return false;
            }
            return true;
        }
        catch// if the try parse failed the cvv isnt an int
        {
            return false;
        }
    }
    public static bool ValidateNames(string text)// helper that checks if a name just isnt null or empty
    {
        if (string.IsNullOrEmpty(text))
        {
            return false;
        }
        return true;
    }
    public static bool ValidateProducts(List<ProductAndQuantity> products)
    // helper that checks if the product has a product id and is in a valid quantity for each product in the list
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
    public static bool ValidateUsername(string username)// helper to check if the username is within normal lengthes and isnt null
    {
        if(string.IsNullOrEmpty(username) || username.Length < 3 || username.Length > 20)
        {
            return false;
        }
        return true;
    }
    public static bool ValidatePassword(string password)// helper to check if the password is within normal lengthes and isnt null
    {
        if (string.IsNullOrEmpty(password) || password.Length < 4 || password.Length > 25)
        {
            return false;
        }
        return true;
    }
}