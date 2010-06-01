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
            IsActive = true;
            NumberOfReads = 0;
        }
        [Required]
        public virtual string Question { get; set; }
        public virtual string Answer { get; set; }
        public virtual bool AvailableToPublic { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual int NumberOfReads { get; set; }
    }
}
