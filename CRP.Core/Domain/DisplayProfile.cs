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
        //TODO: Map to the department class
        //[Required]
        //public virtual int DepartmentId { get; set; }
        public virtual byte[] Logo { get; set; }
    }
}
