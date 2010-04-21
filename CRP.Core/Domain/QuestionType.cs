using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace CRP.Core.Domain
{
    public class QuestionType: DomainObject
    {
        public QuestionType()
        {
            SetDefaults();
        }

        public QuestionType(string name)
        {
            Name = name;

            SetDefaults();
        }

        private void SetDefaults()
        {
            HasOptions = false;
        }

        [Required]
        [Length(50)]
        public virtual string Name { get; set; }
        public virtual bool HasOptions { get; set; }
    }
}
