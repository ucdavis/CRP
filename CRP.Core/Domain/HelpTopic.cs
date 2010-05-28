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

        public virtual string ShortDescription
        {
            get
            {
                if(Description == null)
                {
                    return string.Empty;
                }
                if(Description.Length < 80)
                {
                    return Description.Substring(0, Description.Length);
                }

                return Description.Substring(0, 80);

            }
        }
    }
}
