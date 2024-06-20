using System.ComponentModel.DataAnnotations;

namespace UtilitiesLayer.Attributes;

public class FutureDateAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if(value == null || !(value is DateTime)) return false;

        return (DateTime)value <= DateTime.Now;
    }
}
