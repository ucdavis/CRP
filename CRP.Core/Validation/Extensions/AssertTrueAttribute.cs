using System.ComponentModel.DataAnnotations;

namespace CRP.Core.Validation.Extensions
{
    public class AssertTrueAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is bool && ((bool)value))
            {
                return true;
            }
            return false;

        }
    }
}
