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

            DateCreated = SystemTime.Now();
            ItemCoupons = false;
        }

        [Required]
        [Length(100)]
        public virtual string Name { get; set; }
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
        public virtual DateTime? Expiration { get; set; }
        public virtual byte[] Image { get; set; }
        public virtual string Link { get; set; }

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
        /// # sold
        /// </summary>
        public virtual int Sold { 
            get
            {
                return Transactions.Sum(a => a.Quantity);
            }
        }

        /// <summary>
        /// The only thing that is really new here is that there is a check to see if there is any more quantity
        /// </summary>
        public virtual bool IsAvailableForReg { 
            get
            {
                if (Sold >= Quantity || SystemTime.Now() > Expiration || !Available)
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

        public virtual void AddTransactionQuestionSet(QuestionSet questionSet)
        {
            var itemQuestionSet = new ItemQuestionSet(this, questionSet, QuestionSets.Count);
            itemQuestionSet.TransactionLevel = true;
            QuestionSets.Add(itemQuestionSet);
        }

        public virtual void AddQuantityQuestionSet(QuestionSet questionSet)
        {
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
        }

        #region Fields ONLY used for complex validation, not in database
        [AssertTrue(Message = "One or more tags is not valid")]
        private bool ItemTags { get; set; }

        [AssertTrue(Message = "One or more active coupons has a discount amount greater than the cost per item")]
        private bool ItemCoupons { get; set; }
        #endregion Fields ONLY used for complex validation, not in database
    }
}
