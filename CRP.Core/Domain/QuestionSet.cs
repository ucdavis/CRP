using System.Collections.Generic;
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

        //public virtual ICollection<QuestionSetQuestion> Questions { get; set; }
        public virtual ICollection<Question> Questions { get; set; }


        public virtual void AddQuestion(Question question)
        {
            question.QuestionSet = this;
            question.Order = Questions.Count + 1;

            Questions.Add(question);
        }

        public override bool IsValid()
        {
            var flag = true;

            if (CollegeReusable)
            {
                if (School == null)
                {
                    flag = false;
                }
            }

            return base.IsValid() && flag;
        }
    }
}
