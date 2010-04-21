using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;
using Newtonsoft.Json;

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

        [NotNull]
        public virtual ItemType ItemType { get; set; }
        [Required]
        [Length(100)]
        [JsonProperty]
        public virtual string Name { get; set; }
        [NotNull]
        [JsonProperty]
        public virtual QuestionType QuestionType { get; set; }
        public virtual int Order { get; set; }
    }
}
