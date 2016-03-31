using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using UCDArch.Core.DomainModel;


namespace CRP.Core.Domain
{
    public class Question : DomainObject
    {
        public Question()
        {
            SetDefaults();
        }

        public Question(string name, QuestionType questionType)
        {
            Name = name;
            QuestionType = questionType;

            SetDefaults();
        }

        private void SetDefaults()
        {
            Options = new List<QuestionOption>();
            Validators = new List<Validator>();
            OptionsNames = false;
            OptionsRequired = false;
            OptionsNotAllowed = false;
        }

        [Required]
        [Length(200)]
        public virtual string Name { get; set; }
        [NotNull]
        public virtual QuestionType QuestionType { get; set; }
        [NotNull]
        public virtual QuestionSet QuestionSet { get; set; }
        public virtual int Order { get; set; }
        [NotNull]
        public virtual ICollection<QuestionOption> Options { get; set; }
        [NotNull]
        public virtual ICollection<Validator> Validators { get; set; }

        public virtual void AddOption(QuestionOption questionOption)
        {
            if (QuestionType != null && QuestionType.HasOptions)
            {
                questionOption.Question = this;
                Options.Add(questionOption);
            }
        }

        public virtual void RemoveOptions(QuestionOption questionOption)
        {
            Options.Remove(questionOption);
        }

        public virtual string ValidationClasses { 
            get
            {
                return string.Join(" ", Validators.Select(a => a.Class).ToArray());
            }  
        }

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
            OptionsNames = true;
            OptionsRequired = true;
            OptionsNotAllowed = true;

            if (QuestionType != null && QuestionType.HasOptions)
            {
                if(Options !=null && Options.Count > 0)
                {
                    foreach (var o in Options)
                    {
                        if (o.Name == null || string.IsNullOrEmpty(o.Name.Trim()))
                        {
                            OptionsNames = false;
                            break;
                        }
                    }
                }
                else
                {
                    OptionsRequired = false; //Fail it, they are required
                }
            }
            else
            {
                if (Options != null && Options.Count > 0)
                {
                    OptionsNotAllowed = false; //Fail it, they are not allowed
                }
            }

        }

        #region Fields ONLY used for complex validation, not in database
        [AssertTrue(Message = "One or more options is invalid")]
        private bool OptionsNames { get; set; }

        [AssertTrue(Message = "The question type requires at least one option.")]
        private bool OptionsRequired { get; set; }

        [AssertTrue(Message = "Options not allowed")]
        private bool OptionsNotAllowed { get; set; }
        #endregion Fields ONLY used for complex validation, not in database

    }
}
