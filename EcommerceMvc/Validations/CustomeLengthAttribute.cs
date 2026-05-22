using System.ComponentModel.DataAnnotations;

namespace EcommerceMvc.Validations
{
    [AttributeUsage(AttributeTargets.Property,AllowMultiple =false)]
    public class CustomeLengthAttribute:ValidationAttribute
    {
        
        public override bool IsValid(object? value)
        {
            return base.IsValid(value);
        }
    }
}
