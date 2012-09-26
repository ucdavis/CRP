using System;
using System.Collections.Generic;
using System.Linq;
using CRP.Core.Abstractions;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace CRP.Core.Domain
{
    public class Item : DomainObject
    {
        public Item()
        {
            SetDefaults();
        }

        public Item(string name, int quantity)
        {
            Name = name;
            Quantity = quantity;

            SetDefaults();
        }

        private void SetDefaults()
        {
            Available = false;
            Private = false;

            Tags = new List<Tag>();
            ExtendedPropertyAnswers = new List<ExtendedPropertyAnswer>();
            Coupons = new List<Coupon>();
            Editors = new List<Editor>();
            QuestionSets = new List<ItemQuestionSet>();
            Templates = new List<Template>();
            Transactions = new List<Transaction>();
            Reports = new List<ItemReport>();
            MapPins = new List<MapPin>();

            DateCreated = SystemTime.Now();
            ItemCoupons = false;
            TransactionQuestionSet = false;
            QuantityQuestionSet = false;

            AllowCheckPayment = true;
            AllowCreditPayment = true;
        }

        [Required]
        [Length(100)]
        public virtual string Name { get; set; }

        [Required]
        [Length(750)]
        public virtual string Summary { get; set; }

        public virtual string Description { get; set; }
        [RangeDouble(Min = 0.00, Max = 922337203685477.00, Message = "must be zero or more")]
        public virtual decimal CostPerItem { get; set; }
        /// <summary>
        /// # items available for sale
        /// </summary>
        [Min(0)]
        public virtual int Quantity { get; set; }
        [Length(50)]
        public virtual string QuantityName { get; set; }
        /// <summary>
        /// This is now called Last Date to Register Online, so it needs to be available of that date
        /// </summary>
        public virtual DateTime? Expiration { get; set; }
        public virtual byte[] Image { get; set; }
        public virtual string Link { get; set; }

        public virtual bool AllowCheckPayment { get; set; }
        public virtual bool AllowCreditPayment { get; set; }

        public virtual bool HideDonation { get { return true; } } //We now no longer allow donations. Always hide it.

        [Length(50)]
        public virtual string DonationLinkLegend { get; set; }
        [Length(500)]
        public virtual string DonationLinkInformation { get; set; }
        [Length(50)]
        public virtual string DonationLinkText { get; set; }
        [Length(200)]
        public virtual string DonationLinkLink { get; set; }


        [NotNull]
        public virtual ItemType ItemType { get; set; }

        [NotNull]
        public virtual Unit Unit { get; set; }

        public virtual DateTime DateCreated { get; set; }
        /// <summary>
        /// Whether or not the item is available for the public to view
        /// </summary>
        public virtual bool Available { get; set; }
        /// <summary>
        /// Whether or not this item should be displayed in the searchable list
        /// </summary>
        public virtual bool Private { get; set; }
        /// <summary>
        /// Whether or not this item is a restricted item.  Not Null or Empty means restricted.
        /// </summary>
        [Length(10)]
        public virtual string RestrictedKey { get; set; }

        /// <summary>
        /// Link used for embedding a map
        /// </summary>
        public virtual string MapLink { get; set; }
        /// <summary>
        /// Link used in the below text for linking to google maps
        /// </summary>
        public virtual string LinkLink { get; set; }

        /// <summary>
        /// Gets or sets the check payment instructions.
        /// </summary>
        /// <value>The check payment instructions.</value>
        [Required]
        public virtual string CheckPaymentInstructions { get; set; }


        public virtual string TouchnetFID { get; set; }
        

        [NotNull]
        public virtual ICollection<Tag> Tags { get; set; }
        [NotNull]
        public virtual ICollection<ExtendedPropertyAnswer> ExtendedPropertyAnswers { get; set; }
        [NotNull]
        public virtual ICollection<Coupon> Coupons { get; set; }
        [NotNull]
        public virtual ICollection<Editor> Editors { get; set; }
        [NotNull]
        public virtual ICollection<ItemQuestionSet> QuestionSets { get; set; }
        [NotNull]
        public virtual ICollection<Transaction> Transactions { get; set; }
        [NotNull]
        [Size(Max=1)]
        public virtual ICollection<Template> Templates { get; set; }
        [NotNull]
        public virtual ICollection<ItemReport> Reports { get; set; }

        [NotNull]
        public virtual ICollection<MapPin> MapPins { get; set; }

        public virtual Template Template
        {
            get
            {
                if (Templates != null && Templates.Count > 0)
                {
                    return Templates.FirstOrDefault();
                }

                return null;
            }
            set
            {
                if (Templates != null && Templates.Count > 0)
                {
                    var temp = Templates.FirstOrDefault();
                    temp.Text = value.Text;
                }
                else
                {
                    if (Templates == null)
                    {
                        Templates = new List<Template>();
                    }

                    value.Item = this;
                    Templates.Add(value);
                }
            }
        }

        /// <summary>
        /// # of active, sold
        /// </summary>
        public virtual int Sold { 
            get
            {
                return Transactions.Where(a => a.IsActive).Sum(a => a.Quantity);
            }
        }

        /// <summary>
        /// Gets the sold and paid quantity.
        /// </summary>
        /// <value>The sold and paid quantity.</value>
        public virtual int SoldAndPaidQuantity
        {
            get
            {
                return Transactions.Where(a => a.IsActive && a.Paid).Sum(a => a.Quantity);
            }
        }

        /// <summary>
        /// The only thing that is really new here is that there is a check to see if there is any more quantity
        /// </summary>
        public virtual bool IsAvailableForReg { 
            get
            {
                var lastDayToRegisterOnLine = Expiration == null ? SystemTime.Now().AddDays(-1).Date : (DateTime)Expiration;
                if (Sold >= Quantity || SystemTime.Now().Date > lastDayToRegisterOnLine.Date || !Available)
                {
                    return false;
                }

                return true;
            }
        }

        public virtual void AddTag(Tag tag)
        {
            Tags.Add(tag);
        }

        public virtual void RemoveTag(Tag tag)
        {
            Tags.Remove(tag);
        }

        public virtual void AddExtendedPropertyAnswer(ExtendedPropertyAnswer extendedPropertyAnswer)
        {
            extendedPropertyAnswer.Item = this;
            ExtendedPropertyAnswers.Add(extendedPropertyAnswer);
        }

        public virtual void RemoveExtendedPropertyAnswer(ExtendedPropertyAnswer extendedPropertyAnswer)
        {
            ExtendedPropertyAnswers.Remove(extendedPropertyAnswer);
        }

        public virtual void AddCoupon(Coupon coupon)
        {
            coupon.Item = this;
            Coupons.Add(coupon);
        }

        public virtual void RemoveCoupon(Coupon coupon)
        {
            Coupons.Remove(coupon);
        }

        public virtual void AddEditor(Editor editor)
        {
            editor.Item = this;
            Editors.Add(editor);
        }

        public virtual void RemoveEditor(Editor editor)
        {
            Editors.Remove(editor);
        }

        public virtual void AddMapPin(MapPin mapPin)
        {
            mapPin.Item = this;
            MapPins.Add(mapPin);
        }

        public virtual void RemoveMapPin(MapPin mapPin)
        {
            MapPins.Remove(mapPin);
        }

        public virtual void AddTransactionQuestionSet(QuestionSet questionSet)
        {
            //if(QuestionSets.Where(a => a.QuestionSet == questionSet).Where(a => a.TransactionLevel).Any())
            //{
            //    //Already has it
            //    return;
            //}
            var itemQuestionSet = new ItemQuestionSet(this, questionSet, QuestionSets.Count);
            itemQuestionSet.TransactionLevel = true;
            QuestionSets.Add(itemQuestionSet);
        }

        public virtual void AddQuantityQuestionSet(QuestionSet questionSet)
        {
            //if (QuestionSets.Where(a => a.QuestionSet == questionSet).Where(a => a.QuantityLevel).Any())
            //{
            //    //Already has it
            //    return;
            //}
            var itemQuestionSet = new ItemQuestionSet(this, questionSet, QuestionSets.Count);
            itemQuestionSet.QuantityLevel = true;
            QuestionSets.Add(itemQuestionSet);            
        }

        public virtual void RemoveQuestionSet(ItemQuestionSet itemQuestionSet)
        {
            QuestionSets.Remove(itemQuestionSet);
        }

        /// <summary>
        /// Determines whether this instance is valid.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsValid()
        {
            PopulateComplexLogicFields();
            return base.IsValid();
        }

        public override ICollection<UCDArch.Core.CommonValidator.IValidationResult> ValidationResults()
        {
            PopulateComplexLogicFields();
            return base.ValidationResults();
        }

        /// <summary>
        /// Populates the complex logic fields.
        /// Tag and Item both have a Name field so to avoid confusion, this method is used.
        /// </summary>
        private void PopulateComplexLogicFields()
        {
            ItemTags = true;
            if (Tags != null && Tags.Count > 0)
            {
                foreach (var tag in Tags)
                {
                    if (!tag.IsValid())
                    {
                        ItemTags = false;
                        break;
                    }
                }
            }
            ItemCoupons = true;
            if(Coupons != null && Coupons.Count > 0)
            {
                foreach (Coupon coupon in Coupons)
                {
                    if(coupon.IsActive && coupon.DiscountAmount > CostPerItem)
                    {
                        ItemCoupons = false;
                        break;
                    }
                }
            }

            TransactionQuestionSet = true;
            QuantityQuestionSet = true;
            if (QuestionSets != null)
            {
                foreach (var questionSet in QuestionSets)
                {
                    ItemQuestionSet set = questionSet;
                    var transactionCount =
                        QuestionSets.Where(a => a.QuestionSet == set.QuestionSet).Where(a => a.TransactionLevel).Count();
                    var quantityCount =
                        QuestionSets.Where(a => a.QuestionSet == set.QuestionSet).Where(a => a.QuantityLevel).Count();
                    if (transactionCount > 1)
                    {
                        TransactionQuestionSet = false;
                    }
                    if (quantityCount > 1)
                    {
                        QuantityQuestionSet = false;
                    }
                    if (TransactionQuestionSet == false && QuantityQuestionSet == false)
                    {
                        break;
                    }
                }
            }
        }

        #region Fields ONLY used for complex validation, not in database
        [AssertTrue(Message = "One or more tags is not valid")]
        private bool ItemTags { get; set; }

        [AssertTrue(Message = "One or more active coupons has a discount amount greater than the cost per item")]
        private bool ItemCoupons { get; set; }

        [AssertTrue(Message = "Transaction Question is already added")]
        private bool TransactionQuestionSet { get; set; }
        [AssertTrue(Message = "Quantity Question is already added")]
        private bool QuantityQuestionSet { get; set; }

        [AssertTrue(Message = "Must check at least one payment method")]
        private bool AllowedPaymentMethods
        {
            get
            {
                if(AllowCheckPayment == false && AllowCreditPayment == false)
                {
                    return false;
                }
                return true;
            }
        }

        [AssertTrue(Message = "Must select an Account Number when available to public is checked and credit payment is allowed")]
        private bool FID
        {
            get
            {
                if(string.IsNullOrEmpty(TouchnetFID) && Available && AllowCreditPayment)
                {
                    return false;
                }
                return true;
            }
        }

        [AssertTrue(Message = "FID must be 3 characters long when selected")]
        private bool FID_Length
        {
            get
            {
                if (!string.IsNullOrEmpty(TouchnetFID) && TouchnetFID.Trim().Length != 3)
                {
                    return false;
                }
                return true;
            }
        }

        [AssertTrue(Message = "Only 1 MapPin can be Primary")]
        private bool MapPinPrimary
        {
            get { 
                var count = 0;
                if (MapPins != null) //Null check done elsewhere
                {
                    foreach (MapPin mapPin in MapPins)
                    {
                        if (mapPin.IsPrimary)
                        {
                            count++;
                        }
                    }
                }
                if(count > 1)
                {
                    return false;
                }
                return true;
            }
        }
        #endregion Fields ONLY used for complex validation, not in database
    }
}
