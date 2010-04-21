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

            Questions = new List<QuestionSetQuestion>();
        }
            
        [Required]
        public virtual string Name { get; set; }
        public virtual bool CollegeReusable { get; set; }
        public virtual bool SystemReusable { get; set; }
        public virtual bool UserReusable { get; set; }
        //TODO: Map the user to the user object
        //[NotNull]
        //public virtual int UserId { get; set; }

        public virtual ICollection<QuestionSetQuestion> Questions { get; set; }
    }
}
