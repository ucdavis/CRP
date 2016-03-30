using System.ComponentModel.DataAnnotations;
using UCDArch.Core.DomainModel;

namespace CRP.Core.Domain
{
    public class Editor : DomainObject
    {
        public Editor()
        {
            SetDefaults();
        }

        public Editor(Item item, User user)
        {
            Item = item;
            User = user;
        }

        private void SetDefaults()
        {
            Owner = false;
        }
        [Required]
        public virtual Item Item { get; set; }
        [Required]
        public virtual User User { get; set; }
        public virtual bool Owner { get; set; }
    }
}
