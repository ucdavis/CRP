using CRP.Core.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CRP.Mvc.Controllers.ViewModels.ItemManagement
{
    public class EditItemViewModel
    {
        public int Id { get; set; }

        [Required]
        public int ItemTypeId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(750)]
        public string Summary { get; set; }

        public string Description { get; set; }

        [Range(0.00, 922337203685477.00, ErrorMessage = "must be zero or more")]
        public decimal? CostPerItem { get; set; }
        /// <summary>
        /// 
        /// # items available for sale
        /// </summary>
        [Range(0, Int32.MaxValue)]
        public int? Quantity { get; set; }

        [StringLength(50)]
        public string QuantityName { get; set; }

        /// <summary>
        /// This is now called Last Date to Register Online, so it needs to be available of that date
        /// </summary>
        public DateTime? Expiration { get; set; }

        public byte[] Image { get; set; }

        public string Link { get; set; }

        public bool AllowCheckPayment { get; set; }

        public bool AllowCreditPayment { get; set; }

        [StringLength(50)]
        public string DonationLinkLegend { get; set; }

        [StringLength(500)]
        public string DonationLinkInformation { get; set; }

        [StringLength(50)]
        public string DonationLinkText { get; set; }

        [StringLength(200)]
        public string DonationLinkLink { get; set; }

        /// <summary>
        /// Whether or not the item is available for the public to view
        /// </summary>
        public bool Available { get; set; }

        /// <summary>
        /// Whether or not this item should be displayed in the searchable list
        /// </summary>
        public bool Private { get; set; }

        /// <summary>
        /// Whether or not this item is a restricted item.  Not Null or Empty means restricted.
        /// </summary>
        [StringLength(10)]
        public string RestrictedKey { get; set; }


        public bool NotifyEditors { get; set; }

        /// <summary>
        /// Gets or sets the check payment instructions.
        /// </summary>
        /// <value>The check payment instructions.</value>
        [Required]
        public string CheckPaymentInstructions { get; set; }

        [Required]
        public int UnitId { get; set; }

        [Required]
        public int FinancialAccountId { get; set; }

        public string[] Tags { get; set; }

        public virtual IReadOnlyList<ExtendedPropertyAnswer> ExtendedPropertyAnswers { get; set; }

        public virtual IReadOnlyList<Editor> Editors { get; set; }

        public virtual IReadOnlyList<ItemQuestionSet> QuestionSets { get; set; }

        public virtual IReadOnlyList<Coupon> Coupons { get; set; }
    }
}