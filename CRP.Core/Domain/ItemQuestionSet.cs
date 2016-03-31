using System.ComponentModel.DataAnnotations;
using CRP.Core.Validation.Extensions;
using UCDArch.Core.DomainModel;

namespace CRP.Core.Domain
{
    public class ItemQuestionSet : DomainObject
    {
        public ItemQuestionSet()
        {
            SetDefaults();
        }

        public ItemQuestionSet(Item item, QuestionSet questionSet, int order)
        {
            Item = item;
            QuestionSet = questionSet;
            Order = order;

            SetDefaults();
        }

        private void SetDefaults()
        {
            TransactionLevel = false;
            QuantityLevel = false;
        }

        [Required]
        public virtual Item Item { get; set; }
        [Required]
        //[Valid] //I think what this does, is actually check each entity in the question set to make sure it is valid... Not sure how to do with DataAnnotations
        public virtual QuestionSet QuestionSet { get; set; } //TODO: Review if this should use [valid]
        public virtual bool TransactionLevel { get; set; }
        public virtual bool QuantityLevel { get; set; }
        public virtual int Order { get; set; }
        
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

        private void PopulateComplexLogicFields()
        {
            TransactionLevelAndQuantityLevel = false;
            if (TransactionLevel == QuantityLevel)
            {
                TransactionLevelAndQuantityLevel = true;
            }
        }

        #region Fields ONLY used for complex validation, not in database
        [AssertFalse(ErrorMessage = "TransactionLevel or QuantityLevel must be set but not both.")]
        public virtual bool TransactionLevelAndQuantityLevel { get; set; }
        #endregion Fields ONLY used for complex validation, not in database
        
    }
}
