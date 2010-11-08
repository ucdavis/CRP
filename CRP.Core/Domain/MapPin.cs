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

        public MapPin(Item item, bool isPrimary, string latitude, string longitude)
        {
            SetDefaults();
            Item = item;
            IsPrimary = isPrimary;
            Latitude = latitude;
            Longitude = longitude;
        }

        private void SetDefaults()
        {
            IsPrimary = false;
        }


        [NotNull]
        public virtual Item Item { get; set; }

        public virtual bool IsPrimary { get; set; }
        [Length(50)]
        public virtual string Latitude { get; set; }
        [Length(50)]
        public virtual string Longitude { get; set; }
        [Required]
        [Length(50)]
        public virtual string Title { get; set; }
        [Length(250)]
        public virtual string Description { get; set; }

        [AssertTrue(Message = "Select map to position the pointer.")]
        public virtual bool MapPosition
        {
            get
            {
                if (string.IsNullOrEmpty(Latitude) || string.IsNullOrEmpty(Latitude.Trim()) || string.IsNullOrEmpty(Longitude) || string.IsNullOrEmpty(Longitude.Trim()))
                {
                    return false;
                }
                return true;
            }
        }
    }
}
