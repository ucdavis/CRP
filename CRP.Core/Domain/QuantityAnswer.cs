using System;
using System.Collections.Generic;
using CRP.Core.Abstractions;
using System.ComponentModel.DataAnnotations;
using CRP.Core.Validation.Extensions;
using UCDArch.Core.DomainModel;


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
            QuantityIdNotEmpty = false;
        }

        [Required]
        public virtual Transaction Transaction { get; set; }
        [Required]
        public virtual QuestionSet QuestionSet { get; set; }
        [Required]
        public virtual Question Question { get; set; }
        public virtual Guid QuantityId { get; set; }
        [Required]
        public virtual string Answer { get; set; }

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
        /// </summary>
        private void PopulateComplexLogicFields()
        {
            QuantityIdNotEmpty = false;
            if (QuantityId == Guid.Empty)
            {
                QuantityIdNotEmpty = true;
            }
        }

        #region Fields ONLY used for complex validation, not in database
        [AssertFalse(ErrorMessage = "QuantityId may not be empty.")]
        public virtual bool QuantityIdNotEmpty { get; set; }
        #endregion Fields ONLY used for complex validation, not in database
    }
}
