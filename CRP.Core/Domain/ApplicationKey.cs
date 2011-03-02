using System;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

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
        [Length(100)]
        public virtual string Application { get; set; }
        [Required]
        [Length(50)]
        public virtual string Key { get; set; }
        public virtual bool IsActive { get; set; }

        public virtual void SetKey()
        {
            Key = Guid.NewGuid().ToString().Substring(0, 25);
        }
    }
}
