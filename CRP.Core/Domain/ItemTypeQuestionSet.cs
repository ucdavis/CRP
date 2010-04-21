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
            var flag = true;
            
            if (TransactionLevel == QuantityLevel)
            {
                flag = false;
            }

            return base.IsValid() && flag;
        }

        [NotNull]
        public virtual ItemType ItemType { get; set; }
        [NotNull]
        public virtual QuestionSet QuestionSet { get; set; }
        public virtual bool TransactionLevel { get; set; }
        public virtual bool QuantityLevel { get; set; }
    }
}
