using System;
using System.Collections.Generic;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace CRP.Core.Domain
{
    public class Item : DomainObject
    {
        public Item()
        {
        }

        public Item(string name, int quantity)
        {
            Name = name;
            Quantity = quantity;

            Tags = new List<Tag>();
            ExtendedPropertyAnswers = new List<ExtendedPropertyAnswer>();
        }

        [Required]
        [Length(100)]
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual int Quantity { get; set; }
        public virtual DateTime? Expiration { get; set; }
        public virtual byte[] Image { get; set; }
        public virtual string Link { get; set; }
        public virtual ItemType ItemType { get; set; }
        //TOOD: Map the deparment class
        //public virtual Department Deptartment { get; set; }

        public virtual ICollection<Tag> Tags { get; set; }
        public virtual ICollection<ExtendedPropertyAnswer> ExtendedPropertyAnswers { get; set; }

        public virtual void AddTag(Tag tag)
        {
            tag.Item = this;
            Tags.Add(tag);
        }

        public virtual void AddExtendedPropertyAnswer(ExtendedPropertyAnswer extendedPropertyAnswer)
        {
            extendedPropertyAnswer.Item = this;
            ExtendedPropertyAnswers.Add(extendedPropertyAnswer);
        }
    }
}
