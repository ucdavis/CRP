using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace CRP.Core.Domain
{
    public class DisplayProfile : DomainObject
    {
        public DisplayProfile()
        {
            SetDefaults();
        }

        public DisplayProfile(string name)
        {
            Name = name;

            SetDefaults();
        }

        private void SetDefaults()
        {
            SiteMaster = false;
            SchoolMaster = false;
        }

        [Required]
        public virtual string Name { get; set; }
        public virtual Unit Unit { get; set; }
        public virtual School School { get; set; }
        public virtual byte[] Logo { get; set; }

        public virtual bool SiteMaster { get; set; }
        public virtual bool SchoolMaster { get; set; }
    }
}
