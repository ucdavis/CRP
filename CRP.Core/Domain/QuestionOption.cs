using System.ComponentModel.DataAnnotations;
using UCDArch.Core.DomainModel;


namespace CRP.Core.Domain
{
    public class QuestionOption : DomainObject
    {
        public QuestionOption()
        {
        }

        public QuestionOption(string name)
        {
            Name = name;
        }

        [Required]
        [Length(200)]
        public virtual string Name { get; set; }
        [NotNull]
        public virtual Question Question { get; set; }
    }
}
