using System.Collections.Generic;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace CRP.Core.Domain
{
    public class ItemType : DomainObject 
    {
        public ItemType()
        {
            SetDefaults();
        }

        public ItemType(string name)
        {
            Name = name;

            SetDefaults();
        }

        private void SetDefaults()
        {
            IsActive = true;

            ExtendedProperties = new List<ExtendedProperty>();
            QuestionSets = new List<ItemTypeQuestionSet>();
            Items = new List<Item>();
        }

        [Required]
        [Length(50)]
        public virtual string Name { get; set; }
        public virtual bool IsActive { get; set; }

        [NotNull]
        public virtual ICollection<ExtendedProperty> ExtendedProperties { get; set; }
        [NotNull]
        public virtual ICollection<ItemTypeQuestionSet> QuestionSets { get; set; }
        [NotNull]
        public virtual ICollection<Item> Items { get; set; }

        public virtual void AddExtendedProperty(ExtendedProperty extendedProperty)
        {
            extendedProperty.ItemType = this;
            extendedProperty.Order = ExtendedProperties.Count + 1;
            ExtendedProperties.Add(extendedProperty);
        }

        public virtual void AddTransactionQuestionSet(QuestionSet questionSet)
        {
            QuestionSets.Add(new ItemTypeQuestionSet(this, questionSet){TransactionLevel = true});
        }

        public virtual void AddQuantityQuestionSet(QuestionSet questionSet)
        {
            QuestionSets.Add(new ItemTypeQuestionSet(this, questionSet) { QuantityLevel = true });
        }
    }
}
