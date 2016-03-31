using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using UCDArch.Core.DomainModel;



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
        [JsonProperty]
        public virtual string Name { get; set; }
        public virtual bool HasOptions { get; set; }
        public virtual bool ExtendedProperty { get; set; }
    }
}
