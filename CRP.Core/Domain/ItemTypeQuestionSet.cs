using NHibernate.Validator.Constraints;
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

        public override bool IsValid()
        {
            TransactionLevelQuantityLevel = TransactionLevel != QuantityLevel;
            return base.IsValid();
        }

        [NotNull]
        public virtual ItemType ItemType { get; set; }
        [NotNull]
        public virtual QuestionSet QuestionSet { get; set; }
        public virtual bool TransactionLevel { get; set; }
        public virtual bool QuantityLevel { get; set; }

        [AssertTrue(Message = "TransactionLevel must be different from QuantityLevel")]
        public virtual bool TransactionLevelQuantityLevel { get; set; }
        
    }
}
