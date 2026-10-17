using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BookStore.Web.Models;

public class IsbnAttribute : ValidationAttribute
{
    public bool RequireChecksum { get; set; } = false;

    public IsbnAttribute()
    {
        ErrorMessage = "Nieprawidłowy format ISBN.";
    }

    public override bool IsValid(object? value)
    {
        if (value is not string isbnRaw || string.IsNullOrWhiteSpace(isbnRaw))
        {
            return true;
        }
        string cleanIsbn = isbnRaw.Replace("-", "").Replace(" ", "");
        
        if ((cleanIsbn.Length != 10 && cleanIsbn.Length != 13) || !cleanIsbn.All(char.IsDigit))
        {
            return false;
        }

        if (RequireChecksum && cleanIsbn.Length == 13)
        {
            int sum = 0;
            for (int i = 0; i < 13; i++)
            {
                int digit = cleanIsbn[i] - '0'; 
                sum += digit * (i % 2 == 0 ? 1 : 3);
            }
            
            if (sum % 10 != 0)
            {
                return false;
            }
        }

        return true;
    }
}