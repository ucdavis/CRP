using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using UCDArch.Core.DomainModel;

namespace CRP.Core.Domain
{
    public class FinancialAccount : DomainObject
    {
        public FinancialAccount()
        {
        }

        [StringLength(128)]
        [Required]
        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        /// <summary>
        /// Chart Code.
        /// </summary>
        [StringLength(1)]
        [Required]
        public virtual string Chart { get; set; }

        /// <summary>
        /// Account used in the general ledger.
        /// Accounts are specific to a Chart Code.
        /// </summary>
        [StringLength(7)]
        [RegularExpression("[A-Z0-9]*")]
        [Required]
        public virtual string Account { get; set; }

        /// <summary>
        /// Sub-Account is an optional accounting unit attribute.
        /// Chart Code and Account are part of Sub-Account key.
        /// </summary>
        [StringLength(5)]
        [DisplayFormat(NullDisplayText = "-----")]
        public virtual string SubAccount { get; set; }

        [StringLength(9)]
        [DisplayFormat(NullDisplayText = "---------")]
        public virtual string Project { get; set; }

        //May not use this. Don't think there is a place in the db...
        public virtual Unit Unit { get; set; }

        public virtual bool IsActive { get; set; }
        public virtual bool IsUserAdded { get; set; }

        public virtual string GetAccountString()
        {
            if (string.IsNullOrWhiteSpace(SubAccount))
            {
                return $"{Chart}-{Account}";
            }

            return $"{Chart}-{Account}-{SubAccount}";
        }
    }
}
