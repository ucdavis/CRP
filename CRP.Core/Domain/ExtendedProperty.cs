using UCDArch.Core.DomainModel;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace CRP.Core.Domain
{
    public class ExtendedProperty : DomainObject
    {
        public ExtendedProperty()
        {
            SetDefaults();
        }

        private void SetDefaults()
        {
            Order = 0;
        }

        [Required]
        public virtual ItemType ItemType { get; set; }
        [Required]
        [StringLength(100)]
        [JsonProperty]
        public virtual string Name { get; set; }
        [Required]
        [JsonProperty]
        public virtual QuestionType QuestionType { get; set; }
        public virtual int Order { get; set; }
    }
}
