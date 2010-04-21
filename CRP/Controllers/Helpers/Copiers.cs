using System;
using CRP.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using Check = UCDArch.Core.Utils.Check;
using System.Linq;

namespace CRP.Controllers.Helpers
{
    public class Copiers
    {
        public static Item CopyItem(IRepository repository, Item src, Item dest, ExtendedPropertyParameter[] extendedProperties, string[] tags)
        {
            Check.Require(repository != null, "Repository is required.");
            Check.Require(src != null, "Source item is required.");
            Check.Require(dest != null, "Destination item is required.");

            // copy the fields from the source
            dest.Name = src.Name;
            dest.Description = src.Description;
            dest.CostPerItem = src.CostPerItem;
            dest.Quantity = src.Quantity;
            dest.Expiration = src.Expiration;
            dest.Link = src.Link;
            dest.Available = src.Available;
            dest.Private = src.Private;

            PopulateItem(repository, dest, extendedProperties, tags);

            return dest;
        }

        public static Item PopulateItem(IRepository repository, Item item, ExtendedPropertyParameter[] extendedProperties, string[] tags)
        {
            if (extendedProperties != null)
            {
                // go through and deal with the extended properties
                foreach (var exp in extendedProperties)
                {
                    // check to see if the extended property answer has already been created
                    var extendedProperty = repository.OfType<ExtendedProperty>().GetNullableByID(exp.propertyId);
                    var epa =
                        item.ExtendedPropertyAnswers.Where(a => a.ExtendedProperty == extendedProperty).FirstOrDefault();

                    if (epa == null)
                    {
                        epa = new ExtendedPropertyAnswer(exp.value, item, extendedProperty);
                        item.AddExtendedPropertyAnswer(epa);
                    }
                    else
                    {
                        epa.Answer = exp.value;
                    }
                }
            }

            // go through and deal with the tags
            var existingTags = repository.OfType<Tag>().GetAll();

            if (tags != null)
            {
                foreach (var s in tags)
                {
                    // check to see if it's already associated with the item
                    var tag = item.Tags.FirstOrDefault(a => a.Name == s);
                    var existingTag = existingTags.FirstOrDefault(a => a.Name == s);

                    if (existingTag == null) // no tag with that text exists, create a new one and insert
                    {
                        item.AddTag(new Tag(s));
                    }
                    else if (existingTag != null && tag == null) // tag exists but isn't associated with the item
                    {
                        item.AddTag(existingTag);
                    }
                }

                // remove the tags that are no longer part of it
                var removeTags = item.Tags.Where(a => !tags.Contains(a.Name));

                for (int i = 0; i < removeTags.Count(); i++)
                {
                    item.Tags.Remove(removeTags.ToArray()[i]);
                }
            }

            return item;
        }

        public static DisplayProfile CopyDisplayProfile(DisplayProfile src, DisplayProfile dest)
        {
            dest.Name = src.Name;

            return dest;
        }
    }
}
