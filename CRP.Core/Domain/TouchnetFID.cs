using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;
using CRP.Core.Validation.Extensions;
using UCDArch.Core.DomainModel;

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
        [StringLength(3, MinimumLength = 3)]
        //[StringLengthRange(Minimum = 10, ErrorMessage = "Custom Error")]
        public virtual string FID { get; set; }

        [Required]
        [StringLength(100)]
        public virtual string Description { get; set; }

    }
}
