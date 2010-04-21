using NHibernate.Validator.Constraints;
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

        public virtual string Answer { get; set; }
        [NotNull]
        public virtual Item Item { get; set; }
        [NotNull]
        public virtual ExtendedProperty ExtendedProperty { get; set; }
    }
}
