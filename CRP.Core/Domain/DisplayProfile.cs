using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace CRP.Core.Domain
{
    public class DisplayProfile : DomainObject
    {
        public DisplayProfile()
        {
            
        }

        public DisplayProfile(string name)
        {
            Name = name;
        }

        [Required]
        public virtual string Name { get; set; }
        [NotNull]
        public virtual Unit Unit { get; set; }
        public virtual byte[] Logo { get; set; }
    }
}
