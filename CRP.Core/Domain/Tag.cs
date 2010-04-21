using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace CRP.Core.Domain
{
    public class Tag : DomainObject
    {
        [NotNull]
        public virtual Item Item { get; set; }
        [Required]
        [Length(50)]
        public virtual string Name { get; set; }
    }
}
