using System.ComponentModel.DataAnnotations;
using UCDArch.Core.DomainModel;


namespace CRP.Core.Domain
{
    public class Validator : DomainObject
    {
        [Required]
        [StringLength(50)]
        public virtual string Name { get; set; }
        [StringLength(50)]
        public virtual string Class { get; set; }
        public virtual string RegEx { get; set; }
        public virtual string ErrorMessage { get; set; }

    }
}
