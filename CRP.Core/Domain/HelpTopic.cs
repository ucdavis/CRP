using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace CRP.Core.Domain
{
    public class HelpTopic : DomainObject
    {
        [Required]
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
    }
}
