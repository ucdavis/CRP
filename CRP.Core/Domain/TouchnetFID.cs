using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace CRP.Core.Domain
{
    public class TouchnetFID : DomainObject
    {
        public TouchnetFID()
        {
            
        }
        public TouchnetFID(string fid, string description)
        {
            FID = fid;
            Description = description;
        }

        [Required]
        [Length(3,3)]
        public virtual string FID { get; set; }

        [Required]
        [Length(100)]
        public virtual string Description { get; set; }
    }
}
