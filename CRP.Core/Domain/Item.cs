using System;
using System.Collections.Generic;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace CRP.Core.Domain
{
    public class Item : DomainObject
    {
        public Item()
        {
        }

        public Item(string name, int quantity)
        {
            Name = name;
            Quantity = quantity;

            Tags = new List<Tag>();
            ExtendedPropertyAnswers = new List<ExtendedPropertyAnswer>();
            Coupons = new List<Coupon>();
            Editors = new List<Editor>();
        }

        [Required]
        [Length(100)]
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual decimal CostPerItem { get; set; }
        public virtual int Quantity { get; set; }
        public virtual DateTime? Expiration { get; set; }
        public virtual byte[] Image { get; set; }
        public virtual string Link { get; set; }
        [NotNull]
        public virtual ItemType ItemType { get; set; }
        [NotNull]
        public virtual Unit Unit { get; set; }

        public virtual ICollection<Tag> Tags { get; set; }
        public virtual ICollection<ExtendedPropertyAnswer> ExtendedPropertyAnswers { get; set; }
        public virtual ICollection<Coupon> Coupons { get; set; }
        public virtual ICollection<Editor> Editors { get; set; }
        public virtual ICollection<ItemQuestionSet> QuestionSets { get; set; }

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

        public virtual void AddQuestionSet(QuestionSet questionSet)
        {
            var itemQuestionSet = new ItemQuestionSet(this, questionSet, QuestionSets.Count);
            QuestionSets.Add(itemQuestionSet);
        }

        public virtual void RemoveQuestionSet(ItemQuestionSet itemQuestionSet)
        {
            //TODO: Review this
            QuestionSets.Remove(itemQuestionSet);
        }
    }
}
