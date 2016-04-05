using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CRP.Core.Validation.Extensions;
using UCDArch.Core.DomainModel;


namespace CRP.Core.Domain
{
    public class QuestionSet : DomainObject
    {
        public QuestionSet()
        {
            SetDefaults();
        }

        public QuestionSet(string name)
        {
            Name = name;

            SetDefaults();
        }

        private void SetDefaults()
        {
            CollegeReusable = false;
            SystemReusable = false;
            UserReusable = false;

            Questions = new List<Question>();
            Items = new List<ItemQuestionSet>();
            ItemTypes = new List<ItemTypeQuestionSet>();
        }
            
        [Required]
        [StringLength(50)]
        public virtual string Name { get; set; }
        public virtual bool CollegeReusable { get; set; }
        public virtual bool SystemReusable { get; set; }
        public virtual bool UserReusable { get; set; }
        public virtual School School { get; set; }
        public virtual User User { get; set; }
        /// <summary>
        /// Used to determine whether or not a reusable question set should be displayed as an option.
        /// </summary>
        public virtual bool IsActive { get; set; }

        //public virtual ICollection<QuestionSetQuestion> Questions { get; set; }
        [Required]
        public virtual ICollection<Question> Questions { get; set; }
        [Required]
        public virtual ICollection<ItemQuestionSet> Items { get; set; }
        [Required]
        public virtual ICollection<ItemTypeQuestionSet> ItemTypes { get; set; }

        public virtual void AddQuestion(Question question)
        {
            question.QuestionSet = this;
            question.Order = Questions.Count + 1;

            Questions.Add(question);
        }

        public virtual void RemoveQuestion(Question question)
        {
            if (Questions.Contains(question))
            {
                Questions.Remove(question);
            }
        }

        //TODO: Review.
        public virtual void AddItems(ItemQuestionSet itemQuestionSet)
        {
            itemQuestionSet.QuestionSet = this;
            Items.Add(itemQuestionSet);
        }
        public virtual void RemoveItems(ItemQuestionSet itemQuestionSet)
        {
            if(Items.Contains(itemQuestionSet))
            {
                Items.Remove(itemQuestionSet);
            }
        }
        public virtual void AddItemTypes(ItemTypeQuestionSet itemTypeQuestionSet)
        {
            itemTypeQuestionSet.QuestionSet = this;
            ItemTypes.Add(itemTypeQuestionSet);
        }
        public virtual void RemoveItemTypes(ItemTypeQuestionSet itemTypeQuestionSet)
        {
            if(ItemTypes.Contains(itemTypeQuestionSet))
            {
                ItemTypes.Remove(itemTypeQuestionSet);
            }
        }

        /// <summary>
        /// Populates the complex logic fields.
        /// It needs to be done from both the 
        /// IsValid and ValidationResults to work
        /// correctly from both the Controller and Repository.
        /// </summary>
        private void PopulateComplexLogicFields()
        {
            CollegeReusableSchool = false;
            Reusability = false;
            if (CollegeReusable)
            {
                if (School == null)
                {
                    CollegeReusableSchool = true;
                }
            }
            // should really only be reusable at one level
            if (SystemReusable && CollegeReusable || SystemReusable && UserReusable || CollegeReusable && UserReusable)
            {
                Reusability = true;
            } 
        }

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

        [AssertFalse(ErrorMessage = "Must have school if college reusable")]
        public virtual bool CollegeReusableSchool { get; set; }
        [AssertFalse(ErrorMessage = "Only one reusable flag may be set to true")]
        public virtual bool Reusability { get; set; }

    }
}
