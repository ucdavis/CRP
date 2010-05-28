using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace CRP.Core.Domain
{
    public class HelpTopic : DomainObject
    {
        public HelpTopic()
        {
            SetDefaults();
        }

        public virtual void SetDefaults()
        {
            AvailableToPublic = false;
        }
        [Required]
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        [NotNull]
        public virtual bool AvailableToPublic { get; set; }
    }
}
