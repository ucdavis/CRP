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
        /// <param name="expectedFields">The expected fields.</param>
        /// <param name="entityType">Type of the entity.</param>
        public static void ValidateFieldsAndAttributes(List<NameAndType> expectedFields, Type entityType)
        {
            #region Act
            // get all public static properties of MyClass type
            var propertyInfos = entityType.GetProperties();

            // sort properties by name
            Array.Sort(propertyInfos, (propertyInfo1, propertyInfo2) => propertyInfo1.Name.CompareTo(propertyInfo2.Name));
            #endregion Act

            #region Assert
            Assert.AreEqual(propertyInfos.Count(), expectedFields.Count);
            for (int i = 0; i < propertyInfos.Count(); i++)
            {
                Assert.AreEqual(propertyInfos[i].Name, expectedFields[i].Name);
                Assert.AreEqual(propertyInfos[i].PropertyType.ToString(), expectedFields[i].Property);
                var foundAttributes = CustomAttributeData.GetCustomAttributes(propertyInfos[i]);
                Assert.AreEqual(expectedFields[i].Attributes.Count, foundAttributes.Count);
                if (foundAttributes.Count > 0)
                {
                    for (int j = 0; j < foundAttributes.Count; j++)
                    {
                        Assert.AreEqual(expectedFields[i].Attributes[j], foundAttributes[j].ToString(), "For Field: " + propertyInfos[i].Name);
                    }
                }
            }
            #endregion Assert
        }
    }
}
