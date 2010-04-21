using NHibernate.Validator.Constraints;
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
        [NotNull]
        public virtual Item Item { get; set; }
        [NotNull]
        public virtual User User { get; set; }
        public virtual bool Owner { get; set; }
    }
}
