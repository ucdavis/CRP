using NHibernate.Validator.Constraints;
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
            Required = false;
        }

        [NotNull]
        public virtual Item Item { get; set; }
        [NotNull]
        [Valid]
        public virtual QuestionSet QuestionSet { get; set; } //TODO: Review if this should use [valid]
        public virtual bool TransactionLevel { get; set; }
        public virtual bool QuantityLevel { get; set; }
        public virtual int Order { get; set; }
        public virtual bool Required { get; set; }
        
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
            TransactionLevelAndQuantityLevel = true;
            if (TransactionLevel == QuantityLevel)
            {
                TransactionLevelAndQuantityLevel = false;
            }
        }

        #region Fields ONLY used for complex validation, not in database
        [AssertTrue(Message = "TransactionLevel or QuantityLevel must be set but not both.")]
        private bool TransactionLevelAndQuantityLevel { get; set; }
        #endregion Fields ONLY used for complex validation, not in database
        
    }
}
