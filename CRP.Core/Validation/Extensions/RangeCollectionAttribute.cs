using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace CRP.Core.Validation.Extensions
{
    class RangeCollectionAttribute : ValidationAttribute
    {
        private readonly int _minElements;
        private readonly int _maxElements;
        private readonly bool _required;

        public RangeCollectionAttribute(bool required, int minElements, int maxElements)
        {
            _minElements = minElements;
            _maxElements = maxElements;
            _required = required;
        }

        public override bool IsValid(object value)
        {
            var list = value as IList;
            if (list == null)
            {
                if (_required)
                {
                    return false;
                }
                return true;
            }
            var count = list.Count;
            if (count < _minElements || count > _maxElements)
            {
                return false;
            }
            return true;

        }
    }

}
