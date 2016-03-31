using System.ComponentModel.DataAnnotations;

namespace CRP.Core.Validation.Extensions
{
    public class AssertFalseAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is bool && ((bool)value == false))
            {
                return true;
            }
            return false;

        }
    }
}
