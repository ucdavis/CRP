using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace CRP.Core.Domain
{
    public class QuestionOption : DomainObject
    {
        [Required]
        [Length(200)]
        public virtual string Name { get; set; }
        [NotNull]
        public virtual Question Question { get; set; }
    }
}
