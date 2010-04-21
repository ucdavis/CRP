﻿using System;
using System.Collections.Generic;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

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
        }
            
        [Required]
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
        public virtual ICollection<Question> Questions { get; set; }
        public virtual ICollection<ItemQuestionSet> Items { get; set; }
        public virtual ICollection<ItemTypeQuestionSet> ItemTypes { get; set; }

        public virtual void AddQuestion(Question question)
        {
            question.QuestionSet = this;
            question.Order = Questions.Count + 1;

            Questions.Add(question);
        }

        /// <summary>
        /// Populates the complex logic fields.
        /// It needs to be done from both the 
        /// IsValid and ValidationResults to work
        /// correctly from both the Controller and Repository.
        /// </summary>
        private void PopulateComplexLogicFields()
        {
            CollegeReusableSchool = true;
            Reusability = true;
            if (CollegeReusable)
            {
                if (School == null)
                {
                    CollegeReusableSchool = false;
                }
            }
            // should really only be reusable at one level
            if (SystemReusable && CollegeReusable || SystemReusable && UserReusable || CollegeReusable && UserReusable)
            {
                Reusability = false;
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

        [AssertTrue(Message = "Must have school if college reusable")]
        public virtual bool CollegeReusableSchool { get; set; }
        [AssertTrue(Message = "Only one reusable flag may be set to true")]
        public virtual bool Reusability { get; set; }

    }
}
