using System;
using System.ComponentModel.DataAnnotations;
using UCDArch.Core.DomainModel;


namespace CRP.Core.Domain
{
    public class ApplicationKey : DomainObject
    {
        public ApplicationKey()
        {
            IsActive = true;
            SetKey();
        }

        [Required]
        [StringLength(100)]
        public virtual string Application { get; set; }
        [Required]
        [StringLength(50)]
        public virtual string Key { get; set; }
        public virtual bool IsActive { get; set; }

        public virtual void SetKey()
        {
            Key = Guid.NewGuid().ToString().Substring(0, 25);
        }
    }
}
