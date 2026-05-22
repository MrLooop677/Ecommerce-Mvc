using System.ComponentModel.DataAnnotations;

namespace EcommerceMvc.Validations
{
    [AttributeUsage(AttributeTargets.Property,AllowMultiple =false)]
    public class CustomeLengthAttribute:ValidationAttribute
    {
        private readonly int _minLength;
        private readonly int _maxLength;
        public CustomeLengthAttribute(int minLength, int maxLength)
        {
            _minLength = minLength;
            _maxLength = maxLength;
        }
        public override bool IsValid(object? value)
        {
            if (value is string result) { 
                if(result.Length>= _minLength && result.Length<= _maxLength)
                {
                    return true;
                }
            }
            return false;
        }
        public override string FormatErrorMessage(string name)
        {
            return $"The field {name} must be between {_minLength} and {_maxLength} characters long.";
        }
    }
}
