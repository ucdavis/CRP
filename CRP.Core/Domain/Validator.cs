using System.ComponentModel.DataAnnotations;
using UCDArch.Core.DomainModel;


namespace CRP.Core.Domain
{
    public class Validator : DomainObject
    {
        [Required]
        [Length(50)]
        public virtual string Name { get; set; }
        [Length(50)]
        public virtual string Class { get; set; }
        public virtual string RegEx { get; set; }
        public virtual string ErrorMessage { get; set; }

    }
}
