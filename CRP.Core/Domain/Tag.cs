using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UCDArch.Core.DomainModel;


namespace CRP.Core.Domain
{
    public class Tag : DomainObject
    {
        public Tag()
        {
            SetDefaults();
        }

        public Tag(string name)
        {
            Name = name;

            SetDefaults();
        }

        private void SetDefaults()
        {
            Items = new List<Item>();
        }

        [Required]
        [Length(50)]
        public virtual string Name { get; set; }
        [NotNull]
        public virtual IEnumerable<Item> Items { get; set; }
    }
}
