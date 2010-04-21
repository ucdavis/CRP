using UCDArch.Core.DomainModel;

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

        public virtual string Text { get; set; }
        public virtual Item Item { get; set; }
        public virtual bool Default { get; set; }
    }
}
