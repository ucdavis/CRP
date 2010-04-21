using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CRP.Tests.Core.Helpers
{
    public static class AttributeAndFieldValidation
    {
        /// <summary>
        /// Validates the fields and attributes.
        /// </summary>
        /// <param name="expectedFields">The expected fields. (Fields must be in ascending order, and any attributes must also be in ascending order.)</param>
        /// <param name="entityType">Type of the entity.</param>
        public static void ValidateFieldsAndAttributes(List<NameAndType> expectedFields, Type entityType)
        {
            #region Act
            // get all public static properties of MyClass type


            var propertyInfos = entityType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            // sort properties by name
            Array.Sort(propertyInfos, (propertyInfo1, propertyInfo2) => propertyInfo1.Name.CompareTo(propertyInfo2.Name));
            #endregion Act

            #region Assert
            Assert.AreEqual(expectedFields.Count, propertyInfos.Count());
            for (int i = 0; i < propertyInfos.Count(); i++)
            {
                Assert.AreEqual(expectedFields[i].Name, propertyInfos[i].Name);
                Assert.AreEqual(expectedFields[i].Property, propertyInfos[i].PropertyType.ToString(), "For Field: " + propertyInfos[i].Name);
                var foundAttributes = CustomAttributeData.GetCustomAttributes(propertyInfos[i])
                    .AsQueryable().OrderBy(a => a.ToString()).ToList();
                Assert.AreEqual(expectedFields[i].Attributes.Count, foundAttributes.Count(), "For Field: " + propertyInfos[i].Name);
                if (foundAttributes.Count() > 0)
                {                    
                    for (int j = 0; j < foundAttributes.Count(); j++)
                    {
                        Assert.AreEqual(expectedFields[i].Attributes[j], foundAttributes[j].ToString(), "For Field: " + propertyInfos[i].Name);
                    }
                }
            }
            #endregion Assert
        }
    }
}
