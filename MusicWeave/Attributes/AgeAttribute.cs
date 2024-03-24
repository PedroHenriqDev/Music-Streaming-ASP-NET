using System.ComponentModel.DataAnnotations;

namespace MusicWeave.Attributes
{
    public class AgeAttribute : ValidationAttribute
    {

        public override bool IsValid(object? value)
        {
            if(value == null || !(value is DateTime)) return false;

            DateTime birthDate = (DateTime)value;
            TimeSpan duration = DateTime.Now.Subtract(birthDate);

            return duration.TotalDays > 3650;
        }
    }
}
