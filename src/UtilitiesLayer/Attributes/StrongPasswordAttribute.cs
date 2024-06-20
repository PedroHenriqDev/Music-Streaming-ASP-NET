using System.ComponentModel.DataAnnotations;

namespace UtilitiesLayer.Attributes;

public class StrongPasswordAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value == null || string.IsNullOrEmpty((string)value)) return false;

        string password = (string)value;

        bool hasLetter = password.Any(char.IsLetter);
        bool hasNumber = password.Any(char.IsNumber);
        bool hasSpecialChar = password.Any(c => !char.IsLetterOrDigit(c));

        return hasLetter && hasNumber && hasSpecialChar;
    }
}
