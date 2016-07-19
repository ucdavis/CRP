using System.ComponentModel.DataAnnotations;
using CRP.Core.Validation.Extensions;
using UCDArch.Core.DomainModel;


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


        [Required]
        public virtual Item Item { get; set; }

        public virtual bool IsPrimary { get; set; }
        [StringLength(50)]
        public virtual string Latitude { get; set; }
        [StringLength(50)]
        public virtual string Longitude { get; set; }
        [Required]
        [StringLength(50)]
        public virtual string Title { get; set; }
        [StringLength(100)]
        public virtual string Description { get; set; }

        [AssertFalse(ErrorMessage = "Select map to position the pointer.")]
        public virtual bool MapPosition
        {
            get
            {
                if (string.IsNullOrEmpty(Latitude) || string.IsNullOrEmpty(Latitude.Trim()) || string.IsNullOrEmpty(Longitude) || string.IsNullOrEmpty(Longitude.Trim()))
                {
                    return true;
                }
                return false;
            }
        }
    }
}
