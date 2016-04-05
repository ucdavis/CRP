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
        [StringLength(50)]
        public virtual string Name { get; set; }
        [Required]
        public virtual IEnumerable<Item> Items { get; set; }
    }
}
