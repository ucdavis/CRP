using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace CRP.Core.Domain
{
    public class Template : DomainObject
    {
        public Template()
        {
            SetDefaults();
        }

        public Template(string text)
        {
            Text = text;

            SetDefaults();
        }

        private void SetDefaults()
        {
            Default = false;
        }

        [Required]
        public virtual string Text { get; set; }
        [NotNull]
        public virtual Item Item { get; set; }
        public virtual bool Default { get; set; }
    }
}
