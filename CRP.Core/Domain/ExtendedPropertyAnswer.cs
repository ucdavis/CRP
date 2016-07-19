using System.ComponentModel.DataAnnotations;
using UCDArch.Core.DomainModel;

namespace CRP.Core.Domain
{
    public class ExtendedPropertyAnswer : DomainObject
    {
        public ExtendedPropertyAnswer()
        {
            
        }

        public ExtendedPropertyAnswer(string answer, Item item, ExtendedProperty extendedProperty)
        {
            Answer = answer;
            Item = item;
            ExtendedProperty = extendedProperty;
        }

        [Required]
        public virtual string Answer { get; set; }
        [Required]
        public virtual Item Item { get; set; }
        [Required]
        public virtual ExtendedProperty ExtendedProperty { get; set; }
    }
}
