using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;

namespace CRP.Core.Domain
{
    public class OpenIdUser: DomainObjectWithTypedId<string>
    {
        [Length(255)]
        public virtual string Email { get; set; }
        [Length(255)]
        public virtual string FirstName { get; set; }
        [Length(255)]
        public virtual string LastName { get; set; }
        [Length(255)]
        public virtual string StreetAddress { get; set; }
        [Length(255)]
        public virtual string Address2 { get; set; }
        [Length(255)]
        public virtual string City { get; set; }
        [Length(50)]
        public virtual string State { get; set; }
        [Length(10)]
        public virtual string Zip { get; set; }
        [Length(20)]
        public virtual string PhoneNumber { get; set; }

        public virtual string UserId { 
            set
            {
                Id = value;       
            }
        }
    }
}
