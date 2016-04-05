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
        [StringLength(200)]
        public virtual string Name { get; set; }
        [Required]
        public virtual Question Question { get; set; }
    }
}
