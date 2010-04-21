using System;
using CRP.Core.Abstractions;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace CRP.Core.Domain
{
    public class QuantityAnswer : DomainObject, IQuestionAnswer
    {
        public QuantityAnswer()
        {
            
        }

        /// <summary>
        /// Create a new quantity answer.
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="questionSet"></param>
        /// <param name="question"></param>
        /// <param name="answer"></param>
        /// <param name="quantityId">Leave null if creating a new quantity sequence</param>
        public QuantityAnswer(Transaction transaction, QuestionSet questionSet, Question question, string answer, Guid? quantityId)
        {
            Transaction = transaction;
            QuestionSet = questionSet;
            Question = question;
            Answer = answer;

            QuantityId = quantityId ?? Guid.NewGuid();
        }

        [NotNull]
        public virtual Transaction Transaction { get; set; }
        [NotNull]
        public virtual QuestionSet QuestionSet { get; set; }
        [NotNull]
        public virtual Question Question { get; set; }
        public virtual Guid QuantityId { get; set; }
        [Required]
        public virtual string Answer { get; set; }
    }
}
