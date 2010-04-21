﻿using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;

namespace CRP.Core.Domain
{
    public class ItemTypeQuestionSet : DomainObject
    {
        public ItemTypeQuestionSet()
        {
            SetDefaults();
        }

        public ItemTypeQuestionSet(ItemType itemType, QuestionSet questionSet)
        {
            ItemType = itemType;
            QuestionSet = questionSet;

            SetDefaults();
        }

        private void SetDefaults()
        {
            TransactionLevel = false;
            QuantityLevel = false;
        }

        /// <summary>
        /// Populates the complex logic fields.
        /// It needs to be done from both the 
        /// IsValid and ValidationResults to work
        /// correctly from both the Controller and Repository.
        /// </summary>
        private void PopulateComplexLogicFields()
        {
            TransactionLevelQuantityLevel = TransactionLevel != QuantityLevel;
        }

        public override bool IsValid()
        {
            PopulateComplexLogicFields();
            return base.IsValid();
        }

        public override System.Collections.Generic.ICollection<UCDArch.Core.CommonValidator.IValidationResult> ValidationResults()
        {
            PopulateComplexLogicFields();
            return base.ValidationResults();
        }

        [NotNull]
        public virtual ItemType ItemType { get; set; }
        [NotNull]
        [Valid]
        public virtual QuestionSet QuestionSet { get; set; }
        public virtual bool TransactionLevel { get; set; }
        public virtual bool QuantityLevel { get; set; }

        [AssertTrue(Message = "TransactionLevel must be different from QuantityLevel")]
        public virtual bool TransactionLevelQuantityLevel { get; set; }
        
    }
}
