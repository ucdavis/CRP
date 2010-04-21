using System.Collections.Generic;
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
            CollegeReusable = false;
            SystemReusable = false;
            UserReusable = false;

            Options = new List<QuestionOption>();
        }

        [Required]
        [Length(200)]
        public virtual string Name { get; set; }
        [NotNull]
        public virtual QuestionType QuestionType { get; set; }
        public virtual bool CollegeReusable { get; set; }
        public virtual bool SystemReusable { get; set; }
        public virtual bool UserReusable { get; set; }

        public virtual School School { get; set; }
        public virtual User User { get; set; }

        public virtual ICollection<QuestionOption> Options { get; set; }

        public virtual void AddOption(QuestionOption questionOption)
        {
            questionOption.Question = this;
            Options.Add(questionOption);
        }

        public virtual void RemoveOptions(QuestionOption questionOption)
        {
            Options.Remove(questionOption);
        }
    }
}
