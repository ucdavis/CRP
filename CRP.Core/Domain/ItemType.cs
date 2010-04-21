using System.Collections.Generic;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace CRP.Core.Domain
{
    public class ItemType : DomainObject
    {
        public ItemType()
        {
            SetDefaults();
        }

        public ItemType(string name)
        {
            Name = name;

            SetDefaults();
        }

        private void SetDefaults()
        {
            IsActive = true;

            ExtendedProperties = new List<ExtendedProperty>();
        }

        [Required]
        [Length(50)]
        public virtual string Name { get; set; }
        public virtual bool IsActive { get; set; }

        public virtual ICollection<ExtendedProperty> ExtendedProperties { get; set; }

        public virtual void AddExtendedProperty(ExtendedProperty extendedProperty)
        {
            extendedProperty.ItemType = this;
            ExtendedProperties.Add(extendedProperty);
        }
    }
}
