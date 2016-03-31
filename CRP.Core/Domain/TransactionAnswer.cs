using CRP.Core.Abstractions;
using System.ComponentModel.DataAnnotations;
using UCDArch.Core.DomainModel;


namespace CRP.Core.Domain
{
    public class TransactionAnswer : DomainObject, IQuestionAnswer
    {
        public TransactionAnswer()
        {
            
        }

        public TransactionAnswer(Transaction transaction, QuestionSet questionSet, Question question, string answer)
        {
            Transaction = transaction;
            QuestionSet = questionSet;
            Question = question;
            Answer = answer;
        }

        [NotNull]
        public virtual Transaction Transaction { get; set; }
        [NotNull]
        public virtual QuestionSet QuestionSet { get; set; }
        [NotNull]
        public virtual Question Question { get; set; }
        [Required]
        public virtual string Answer { get; set; }
    }
}
