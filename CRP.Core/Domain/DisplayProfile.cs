using UCDArch.Core.NHibernateValidator.Extensions;

namespace CRP.Core.Domain
{
    public class DisplayProfile
    {
        public DisplayProfile()
        {
            
        }

        public DisplayProfile(string name)
        {
            Name = name;
        }

        [Required]
        public virtual string Name { get; set; }
        [Required]
        public virtual Unit Unit { get; set; }
        public virtual byte[] Logo { get; set; }
    }
}
