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
        public virtual QuestionSet QuestionSet { get; set; }
        public virtual bool TransactionLevel { get; set; }
        public virtual bool QuantityLevel { get; set; }
        public virtual int Order { get; set; }
        public virtual bool Required { get; set; }
    }
}
