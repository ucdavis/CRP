﻿using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using CRP.Core.Validation.Extensions;
using UCDArch.Core.DomainModel;



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
        [StringLength(50)]
        public virtual string Name { get; set; }
        public virtual bool IsActive { get; set; }

        [Required] //TODO: Check that required works... May need my custom validation for collections.
        public virtual ICollection<ExtendedProperty> ExtendedProperties { get; set; }
        [Required]
        public virtual ICollection<ItemTypeQuestionSet> QuestionSets { get; set; }
        [Required]
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

        public virtual void RemoveQuestionSet(ItemTypeQuestionSet itemTypeQuestionSet)
        {
            QuestionSets.Remove(itemTypeQuestionSet);
        }
        public virtual void RemoveQuestionSet(QuestionSet questionSet)
        {
            var itemTypeQuestionSet = QuestionSets.Where(a => a.QuestionSet == questionSet).FirstOrDefault();
            if (itemTypeQuestionSet != null)
            {
                QuestionSets.Remove(itemTypeQuestionSet);
            }
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
            ItemTypeExtendedProperties = false;
            ItemTypeQuestionSets = false;
            if (ExtendedProperties != null && ExtendedProperties.Count > 0)
            {
                foreach (var extendedProperty in ExtendedProperties)
                {
                    if (!extendedProperty.IsValid())
                    {
                        ItemTypeExtendedProperties = true;
                        break;
                    }
                }
            }
            if (QuestionSets != null && QuestionSets.Count > 0)
            {
                foreach (var questionSet in QuestionSets)
                {
                    if (!questionSet.IsValid())
                    {
                        ItemTypeQuestionSets = true;
                        break;
                    }
                }
            }
        }

        #region Fields ONLY used for complex validation, not in database
        [AssertFalse(ErrorMessage = "One or more Extended Properties is not valid")]
        public virtual bool ItemTypeExtendedProperties { get; set; }
        [AssertFalse(ErrorMessage = "One or more Question Sets is not valid")]
        public virtual bool ItemTypeQuestionSets { get; set; }
        #endregion Fields ONLY used for complex validation, not in database
    }
}
