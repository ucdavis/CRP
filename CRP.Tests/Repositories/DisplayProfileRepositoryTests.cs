using System;
using System.Collections.Generic;
using System.Linq;
using CRP.Core.Domain;
using CRP.Tests.Core;
using CRP.Tests.Core.Extensions;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing.Extensions;

namespace CRP.Tests.Repositories
{
    [TestClass]
    public class DisplayProfileRepositoryTests : AbstractRepositoryTests<DisplayProfile, int>
    {
        private IRepositoryWithTypedId<School, string> SchoolRepository { get; set; }
        protected IRepository<DisplayProfile> DisplayProfileRepository { get; set; }


        #region Init and Overrides

        public DisplayProfileRepositoryTests()
        {
            SchoolRepository = new RepositoryWithTypedId<School, string>();
            DisplayProfileRepository = new Repository<DisplayProfile>();
        }
        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override DisplayProfile GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.DisplayProfile(counter);
            rtValue.Unit = Repository.OfType<Unit>().GetById(1);
            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<DisplayProfile> GetQuery(int numberAtEnd)
        {
            return Repository.OfType<DisplayProfile>().Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(DisplayProfile entity, int counter)
        {
            Assert.AreEqual("Name" + counter, entity.Name);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(DisplayProfile entity, ARTAction action)
        {
            const string updateValue = "Updated";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.Name);
                    break;
                case ARTAction.Restore:
                    entity.Name = RestoreValue;
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.Name;
                    entity.Name = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// DisplayProfile Requires Units
        /// </summary>
        protected override void LoadData()
        {
            Repository.OfType<DisplayProfile>().DbContext.BeginTransaction();
            LoadUnits(1);
            LoadRecords(5);
            Repository.OfType<DisplayProfile>().DbContext.CommitTransaction();
        }

        #endregion Init and Overrides

        #region Name Tests

        #region Invalid Tests

        /// <summary>
        /// Tests the name with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithNullValueDoesNotSave()
        {
            DisplayProfile displayProfile = null;
            try
            {
                #region Arrange
                displayProfile = GetValid(9);
                displayProfile.Name = null;
                #endregion Arrange

                #region Act
                DisplayProfileRepository.DbContext.BeginTransaction();
                DisplayProfileRepository.EnsurePersistent(displayProfile);
                DisplayProfileRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(displayProfile);
                var results = displayProfile.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(displayProfile.IsTransient());
                Assert.IsFalse(displayProfile.IsValid());
                #endregion Assert

                throw;
            }		
        }

        /// <summary>
        /// Tests the name with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithEmptyStringDoesNotSave()
        {
            DisplayProfile displayProfile = null;
            try
            {
                #region Arrange
                displayProfile = GetValid(9);
                displayProfile.Name = string.Empty;
                #endregion Arrange

                #region Act
                DisplayProfileRepository.DbContext.BeginTransaction();
                DisplayProfileRepository.EnsurePersistent(displayProfile);
                DisplayProfileRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(displayProfile);
                var results = displayProfile.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(displayProfile.IsTransient());
                Assert.IsFalse(displayProfile.IsValid());
                #endregion Assert

                throw;
            }
        }

        /// <summary>
        /// Tests the name with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithSpacesOnlyDoesNotSave()
        {
            DisplayProfile displayProfile = null;
            try
            {
                #region Arrange
                displayProfile = GetValid(9);
                displayProfile.Name = " ";
                #endregion Arrange

                #region Act
                DisplayProfileRepository.DbContext.BeginTransaction();
                DisplayProfileRepository.EnsurePersistent(displayProfile);
                DisplayProfileRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(displayProfile);
                var results = displayProfile.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(displayProfile.IsTransient());
                Assert.IsFalse(displayProfile.IsValid());
                #endregion Assert

                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Test

        /// <summary>
        /// Tests the name with one character saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithOneCharacterSaves()
        {
            #region Arrange
            var displayProfile = GetValid(9);
            displayProfile.Name = "x";
            #endregion Arrange

            #region Act
            DisplayProfileRepository.DbContext.BeginTransaction();
            DisplayProfileRepository.EnsurePersistent(displayProfile);
            DisplayProfileRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(1, displayProfile.Name.Length);
            Assert.IsFalse(displayProfile.IsTransient());
            Assert.IsTrue(displayProfile.IsValid());
            #endregion Assert				
        }

        /// <summary>
        /// Tests the name with many characters saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithManyCharactersSaves()
        {
            #region Arrange
            var displayProfile = GetValid(9);
            displayProfile.Name = "x".RepeatTimes(200);
            #endregion Arrange

            #region Act
            DisplayProfileRepository.DbContext.BeginTransaction();
            DisplayProfileRepository.EnsurePersistent(displayProfile);
            DisplayProfileRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(200, displayProfile.Name.Length);
            Assert.IsFalse(displayProfile.IsTransient());
            Assert.IsTrue(displayProfile.IsValid());
            #endregion Assert
        }
        #endregion Valid Test
        
        #endregion Name Tests

        #region Unit Tests

        #region Invalid Tests

        /// <summary>
        /// Tests the unit with new value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestUnitWithNewValueDoesNotSave()
        {
            DisplayProfile displayProfile = null;
            try
            {
                #region Arrange
                displayProfile = GetValid(9);
                displayProfile.Unit = new Unit();
                #endregion Arrange

                #region Act
                DisplayProfileRepository.DbContext.BeginTransaction();
                DisplayProfileRepository.EnsurePersistent(displayProfile);
                DisplayProfileRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                #region Assert
                Assert.IsNotNull(displayProfile);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: CRP.Core.Domain.Unit, Entity: CRP.Core.Domain.Unit", ex.Message);
                #endregion Assert

                throw;
            }		
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the unit with valid data saves.
        /// </summary>
        [TestMethod]
        public void TestUnitWithValidDataSaves()
        {
            LoadUnits(3);
            #region Arrange
            var displayProfile = GetValid(9);
            displayProfile.Unit = Repository.OfType<Unit>().GetNullableById(3);
            displayProfile.School = null;
            #endregion Arrange

            #region Act
            DisplayProfileRepository.DbContext.BeginTransaction();
            DisplayProfileRepository.EnsurePersistent(displayProfile);
            DisplayProfileRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(displayProfile.IsTransient());
            Assert.IsTrue(displayProfile.IsValid());
            #endregion Assert		
        }

        #endregion Valid Tests
        #endregion Unit Tests

        #region School Tests

        #region Invalid Tests

        /// <summary>
        /// Tests the School with new value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestSchoolWithNewValueDoesNotSave()
        {
            DisplayProfile displayProfile = null;
            try
            {
                #region Arrange
                displayProfile = GetValid(9);
                displayProfile.School = new School();
                displayProfile.Unit = null;
                #endregion Arrange

                #region Act
                DisplayProfileRepository.DbContext.BeginTransaction();
                DisplayProfileRepository.EnsurePersistent(displayProfile);
                DisplayProfileRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                #region Assert
                Assert.IsNotNull(displayProfile);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: CRP.Core.Domain.School, Entity: CRP.Core.Domain.School", ex.Message);
                #endregion Assert

                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the School with valid data saves.
        /// </summary>
        [TestMethod]
        public void TestSchoolWithValidDataSaves()
        {
            LoadSchools(3);
            #region Arrange
            var displayProfile = GetValid(9);
            displayProfile.School = SchoolRepository.GetNullableById("3");
            displayProfile.Unit = null;
            #endregion Arrange

            #region Act
            DisplayProfileRepository.DbContext.BeginTransaction();
            DisplayProfileRepository.EnsurePersistent(displayProfile);
            DisplayProfileRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(displayProfile.IsTransient());
            Assert.IsTrue(displayProfile.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Unit Tests

        #region Logo Tests

        /// <summary>
        /// Tests the logo with null value saves.
        /// </summary>
        [TestMethod]
        public void TestLogoWithNullValueSaves()
        {
            #region Arrange
            var displayProfile = GetValid(9);
            displayProfile.Logo = null;
            #endregion Arrange

            #region Act
            DisplayProfileRepository.DbContext.BeginTransaction();
            DisplayProfileRepository.EnsurePersistent(displayProfile);
            DisplayProfileRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(displayProfile.IsTransient());
            Assert.IsTrue(displayProfile.IsValid());
            #endregion Assert		
        }

        /// <summary>
        /// Tests the logo with content saves.
        /// </summary>
        [TestMethod]
        public void TestLogoWithContentSaves()
        {
            #region Arrange
            var displayProfile = GetValid(9);
            displayProfile.Logo = new byte[] {1, 2, 3, 4, 5, 6, 7};
            #endregion Arrange

            #region Act
            DisplayProfileRepository.DbContext.BeginTransaction();
            DisplayProfileRepository.EnsurePersistent(displayProfile);
            DisplayProfileRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(displayProfile.IsTransient());
            Assert.IsTrue(displayProfile.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the logo with empty content saves.
        /// </summary>
        [TestMethod]
        public void TestLogoWithEmptyContentSaves()
        {
            #region Arrange
            var displayProfile = GetValid(9);
            displayProfile.Logo = new byte[0];
            #endregion Arrange

            #region Act
            DisplayProfileRepository.DbContext.BeginTransaction();
            DisplayProfileRepository.EnsurePersistent(displayProfile);
            DisplayProfileRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(displayProfile.IsTransient());
            Assert.IsTrue(displayProfile.IsValid());
            #endregion Assert
        }
        #endregion Logo Tests

        #region SchoolMaster Tests

        /// <summary>
        /// Tests the school master when true saves.
        /// </summary>
        [TestMethod]
        public void TestSchoolMasterWhenTrueSaves()
        {
            #region Arrange
            LoadSchools(1);
            var displayProfile = GetValid(9);
            displayProfile.School = SchoolRepository.GetNullableById("1");
            displayProfile.Unit = null;
            displayProfile.SchoolMaster = true;
            #endregion Arrange

            #region Act
            DisplayProfileRepository.DbContext.BeginTransaction();
            DisplayProfileRepository.EnsurePersistent(displayProfile);
            DisplayProfileRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(displayProfile.IsTransient());
            Assert.IsTrue(displayProfile.IsValid());
            #endregion Assert		
        }


        /// <summary>
        /// Tests the school master when false saves.
        /// </summary>
        [TestMethod]
        public void TestSchoolMasterWhenFalseSaves()
        {
            #region Arrange
            LoadSchools(1);
            var displayProfile = GetValid(9);
            displayProfile.School = SchoolRepository.GetNullableById("1");
            displayProfile.Unit = null;
            displayProfile.SchoolMaster = false;
            #endregion Arrange

            #region Act
            DisplayProfileRepository.DbContext.BeginTransaction();
            DisplayProfileRepository.EnsurePersistent(displayProfile);
            DisplayProfileRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(displayProfile.IsTransient());
            Assert.IsTrue(displayProfile.IsValid());
            #endregion Assert
        }
        #endregion SchoolMaster Tests

        #region HeaderColor Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the HeaderColor with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestHeaderColorWithTooLongValueDoesNotSave()
        {
            DisplayProfile displayProfile = null;
            try
            {
                #region Arrange
                displayProfile = GetValid(9);
                displayProfile.HeaderColor = "x".RepeatTimes((50 + 1));
                #endregion Arrange

                #region Act
                DisplayProfileRepository.DbContext.BeginTransaction();
                DisplayProfileRepository.EnsurePersistent(displayProfile);
                DisplayProfileRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(displayProfile);
                Assert.AreEqual(50 + 1, displayProfile.HeaderColor.Length);
                var results = displayProfile.ValidationResults().AsMessageList();
                results.AssertErrorsAre("HeaderColor: length must be between 0 and 50");
                Assert.IsTrue(displayProfile.IsTransient());
                Assert.IsFalse(displayProfile.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the HeaderColor with null value saves.
        /// </summary>
        [TestMethod]
        public void TestHeaderColorWithNullValueSaves()
        {
            #region Arrange
            var displayProfile = GetValid(9);
            displayProfile.HeaderColor = null;
            #endregion Arrange

            #region Act
            DisplayProfileRepository.DbContext.BeginTransaction();
            DisplayProfileRepository.EnsurePersistent(displayProfile);
            DisplayProfileRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(displayProfile.IsTransient());
            Assert.IsTrue(displayProfile.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the HeaderColor with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestHeaderColorWithEmptyStringSaves()
        {
            #region Arrange
            var displayProfile = GetValid(9);
            displayProfile.HeaderColor = string.Empty;
            #endregion Arrange

            #region Act
            DisplayProfileRepository.DbContext.BeginTransaction();
            DisplayProfileRepository.EnsurePersistent(displayProfile);
            DisplayProfileRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(displayProfile.IsTransient());
            Assert.IsTrue(displayProfile.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the HeaderColor with one space saves.
        /// </summary>
        [TestMethod]
        public void TestHeaderColorWithOneSpaceSaves()
        {
            #region Arrange
            var displayProfile = GetValid(9);
            displayProfile.HeaderColor = " ";
            #endregion Arrange

            #region Act
            DisplayProfileRepository.DbContext.BeginTransaction();
            DisplayProfileRepository.EnsurePersistent(displayProfile);
            DisplayProfileRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(displayProfile.IsTransient());
            Assert.IsTrue(displayProfile.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the HeaderColor with one character saves.
        /// </summary>
        [TestMethod]
        public void TestHeaderColorWithOneCharacterSaves()
        {
            #region Arrange
            var displayProfile = GetValid(9);
            displayProfile.HeaderColor = "x";
            #endregion Arrange

            #region Act
            DisplayProfileRepository.DbContext.BeginTransaction();
            DisplayProfileRepository.EnsurePersistent(displayProfile);
            DisplayProfileRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(displayProfile.IsTransient());
            Assert.IsTrue(displayProfile.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the HeaderColor with long value saves.
        /// </summary>
        [TestMethod]
        public void TestHeaderColorWithLongValueSaves()
        {
            #region Arrange
            var displayProfile = GetValid(9);
            displayProfile.HeaderColor = "x".RepeatTimes(50);
            #endregion Arrange

            #region Act
            DisplayProfileRepository.DbContext.BeginTransaction();
            DisplayProfileRepository.EnsurePersistent(displayProfile);
            DisplayProfileRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(50, displayProfile.HeaderColor.Length);
            Assert.IsFalse(displayProfile.IsTransient());
            Assert.IsTrue(displayProfile.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion HeaderColor Tests




        //TODO: Other tests
         
        #region Validation Moved from the controller Tests (SchoolAndUnit and SchoolOrUnit)

        /// <summary>
        /// Tests the both school and unit populated will not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestBothSchoolAndUnitPopulatedWillNotSave()
        {
            DisplayProfile displayProfileRecord = null;
            try
            {
                SchoolRepository.DbContext.BeginTransaction();
                LoadSchools(1);
                SchoolRepository.DbContext.CommitTransaction();

                Repository.OfType<DisplayProfile>().DbContext.BeginTransaction();
                displayProfileRecord = CreateValidEntities.DisplayProfile(1);
                displayProfileRecord.Unit = Repository.OfType<Unit>().GetById(1);
                displayProfileRecord.School = SchoolRepository.GetById("1");
                Repository.OfType<DisplayProfile>().EnsurePersistent(displayProfileRecord);
                Repository.OfType<DisplayProfile>().DbContext.CommitTransaction();
            }
            catch (Exception)
            {
                Assert.IsNotNull(displayProfileRecord);
                var results = displayProfileRecord.ValidationResults().AsMessageList();
                results.AssertErrorsAre("DepartmentAndSchool: Department and School cannot be selected together.");
                Assert.IsTrue(displayProfileRecord.IsTransient());
                Assert.IsFalse(displayProfileRecord.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests that neither school and unit populated will not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNeitherSchoolAndUnitPopulatedWillNotSave()
        {
            DisplayProfile displayProfileRecord = null;
            try
            {
                Repository.OfType<DisplayProfile>().DbContext.BeginTransaction();
                displayProfileRecord = CreateValidEntities.DisplayProfile(1);
                displayProfileRecord.Unit = null;
                displayProfileRecord.School = null;
                Repository.OfType<DisplayProfile>().EnsurePersistent(displayProfileRecord);
                Repository.OfType<DisplayProfile>().DbContext.CommitTransaction();
            }
            catch (Exception)
            {
                Assert.IsNotNull(displayProfileRecord);
                var results = displayProfileRecord.ValidationResults().AsMessageList();
                results.AssertErrorsAre("DepartmentOrSchool: A Department or School must be specified.");
                Assert.IsTrue(displayProfileRecord.IsTransient());
                Assert.IsFalse(displayProfileRecord.IsValid());
                throw;
            }
        }



        #endregion Validation Moved from the controller Tests

        #region SchoolMasterAndSchool Tests

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestSchoolMasterAndSchoolWhenInvalidWillNotSave()
        {
            DisplayProfile displayProfile = null;
            try
            {
                #region Arrange
                displayProfile = GetValid(9);
                displayProfile.School = null;
                displayProfile.Unit = Repository.OfType<Unit>().GetNullableById(1);
                displayProfile.SchoolMaster = true;
                #endregion Arrange

                #region Act
                DisplayProfileRepository.DbContext.BeginTransaction();
                DisplayProfileRepository.EnsurePersistent(displayProfile);
                DisplayProfileRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(displayProfile);
                var results = displayProfile.ValidationResults().AsMessageList();
                results.AssertErrorsAre("SchoolMasterAndSchool: SchoolMaster may only be true when School is selected.");
                Assert.IsTrue(displayProfile.IsTransient());
                Assert.IsFalse(displayProfile.IsValid());
                #endregion Assert

                throw;
            }	
        }
        
        #endregion SchoolMasterAndSchool Tests

        #region Reflection of Database.

        /// <summary>
        /// Tests all fields in the database have been tested.
        /// If this fails and no other tests, it means that a field has been added which has not been tested above.
        /// </summary>
        [TestMethod]
        public void TestAllFieldsInTheDatabaseHaveBeenTested()
        {
            #region Arrange

            var expectedFields = new List<NameAndType>();            
            expectedFields.Add(new NameAndType("DepartmentAndSchool", "System.Boolean", new List<string>
            {
                 "[NHibernate.Validator.Constraints.AssertTrueAttribute(Message = \"Department and School cannot be selected together.\")]"
            }));
            expectedFields.Add(new NameAndType("DepartmentOrSchool", "System.Boolean", new List<string>
            {
                 "[NHibernate.Validator.Constraints.AssertTrueAttribute(Message = \"A Department or School must be specified.\")]"
            }));
            expectedFields.Add(new NameAndType("HeaderColor", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)50)]"
            }));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                 "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                 "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Logo", "System.Byte[]", new List<string>()));
            expectedFields.Add(new NameAndType("Name", "System.String", new List<string>
            {
                 "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("School", "CRP.Core.Domain.School", new List<string>()));
            expectedFields.Add(new NameAndType("SchoolMaster", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("SchoolMasterAndSchool", "System.Boolean", new List<string>
            {
                 "[NHibernate.Validator.Constraints.AssertTrueAttribute(Message = \"SchoolMaster may only be true when School is selected.\")]"
            }));
            expectedFields.Add(new NameAndType("Unit", "CRP.Core.Domain.Unit", new List<string>()));
    
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(DisplayProfile));

        }



        #endregion Reflection of Database.


        #region HelperMethods

        //private void LoadSchools(int entriesToAdd)
        //{
        //    for (int i = 0; i < entriesToAdd; i++)
        //    {
        //        var validEntity = CreateValidEntities.School(i + 1);
                
        //        SchoolRepository.EnsurePersistent(validEntity);
        //    }
        //}

        #endregion HelperMethods
    }
}
