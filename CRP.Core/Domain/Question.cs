using System.Collections.Generic;
using System.Linq;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

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
        }

        [Required]
        [Length(200)]
        public virtual string Name { get; set; }
        [NotNull]
        public virtual QuestionType QuestionType { get; set; }
        [NotNull]
        public virtual QuestionSet QuestionSet { get; set; }
        public virtual int Order { get; set; }

        public virtual ICollection<QuestionOption> Options { get; set; }
        public virtual ICollection<Validator> Validators { get; set; }

        public virtual void AddOption(QuestionOption questionOption)
        {
            questionOption.Question = this;
            Options.Add(questionOption);
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

        public override bool IsValid()
        {
            var valid = true;

            // validate that the options are valid
            if (QuestionType != null && QuestionType.HasOptions)
            {
                foreach(var o in Options)
                {
                    if (string.IsNullOrEmpty(o.Name))
                    {
                        valid = false;
                        break;
                    }
                }
            }

            return base.IsValid() && valid;
        }

    }
}
