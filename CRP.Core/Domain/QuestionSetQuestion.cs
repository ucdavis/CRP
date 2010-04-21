using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;

namespace CRP.Core.Domain
{
    public class QuestionSetQuestion : DomainObject
    {
        public QuestionSetQuestion()
        {
        }

        public QuestionSetQuestion(QuestionSet questionSet, Question question, int order)
        {
            QuestionSet = questionSet;
            Question = question;
            Order = order;
        }

        [NotNull]
        public virtual QuestionSet QuestionSet { get; set; }
        [NotNull]
        public virtual Question Question { get; set; }
        public virtual int Order { get; set; }
    }
}
