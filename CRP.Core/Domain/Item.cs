using System;
using System.Collections.Generic;
using System.Linq;
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

            DateCreated = DateTime.Now;
        }

        [Required]
        [Length(100)]
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual decimal CostPerItem { get; set; }
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

        public virtual ICollection<Tag> Tags { get; set; }
        public virtual ICollection<ExtendedPropertyAnswer> ExtendedPropertyAnswers { get; set; }
        public virtual ICollection<Coupon> Coupons { get; set; }
        public virtual ICollection<Editor> Editors { get; set; }
        public virtual ICollection<ItemQuestionSet> QuestionSets { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual ICollection<Template> Templates { get; set; }

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

        public virtual int Sold { 
            get
            {
                return Quantity - Transactions.Sum(a => a.Quantity);
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
    }
}
