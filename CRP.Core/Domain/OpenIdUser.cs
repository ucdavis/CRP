using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UCDArch.Core.DomainModel;


namespace CRP.Core.Domain
{
    public class OpenIdUser: DomainObjectWithTypedId<string>
    {
        public OpenIdUser()
        {
            Transactions = new List<Transaction>();
        }

        [StringLength(255)]
        public virtual string Email { get; set; }
        [StringLength(255)]
        public virtual string FirstName { get; set; }
        [StringLength(255)]
        public virtual string LastName { get; set; }
        [StringLength(255)]
        public virtual string StreetAddress { get; set; }
        [StringLength(255)]
        public virtual string Address2 { get; set; }
        [StringLength(255)]
        public virtual string City { get; set; }
        [StringLength(50)]
        public virtual string State { get; set; }
        [StringLength(10)]
        public virtual string Zip { get; set; }
        [StringLength(20)]
        public virtual string PhoneNumber { get; set; }
        [Required]
        public virtual ICollection<Transaction> Transactions { get; set; }
        [Required]
        public virtual string UserId { 
            set
            {
                Id = value;       
            }
            get { return Id; }
        }
    }
}
