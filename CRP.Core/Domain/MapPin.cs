using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace CRP.Core.Domain
{
    public class MapPin : DomainObject
    {
        public MapPin()
        {
            SetDefaults();
        }

        public MapPin(Item item, bool isPrimary)
        {
            SetDefaults();
            Item = item;
            IsPrimary = isPrimary;
        }

        private void SetDefaults()
        {
            IsPrimary = false;
        }


        [NotNull]
        public virtual Item Item { get; set; }

        public virtual bool IsPrimary { get; set; }
        [Required]
        public virtual string Latitude { get; set; }
        [Required]
        public virtual string Longitude { get; set; }
        [Length(50)]
        public virtual string Title { get; set; }
        [Length(250)]
        public virtual string Description { get; set; }
    }
}
