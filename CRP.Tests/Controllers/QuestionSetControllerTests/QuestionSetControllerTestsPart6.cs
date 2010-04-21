using System.Linq;
using CRP.Controllers;
using CRP.Controllers.Helpers;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using CRP.Tests.Core.Extensions;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;

namespace CRP.Tests.Controllers.QuestionSetControllerTests
{
    public partial class QuestionSetControllerTests
    {
        #region Create Get Tests

        /// <summary>
        /// Tests the create get returns view.
        /// </summary>
        [TestMethod]
        public void TestCreateGetReturnsView()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });
            SetupDataForCreateTests();
            #endregion Arrange

            #region Act/Assert
            Controller.Create(null, null, null, null)
                .AssertViewRendered()
                .WithViewData<QuestionSetViewModel>();
            #endregion Act/Assert
        }

        /// <summary>
        /// Tests the create get returns view model with only role admin if admin and any others.
        /// </summary>
        [TestMethod]
        public void TestCreateGetReturnsViewModelWithOnlyRoleAdminIfAdminAndAnyOthers()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User, RoleNames.SchoolAdmin, RoleNames.Admin, RoleNames.ManageAll });
            SetupDataForCreateTests();
            #endregion Arrange

            #region Act
            var result = Controller.Create(null, null, null, null)
                .AssertViewRendered()
                .WithViewData<QuestionSetViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsAdmin);
            Assert.IsFalse(result.IsSchoolAdmin);
            Assert.IsFalse(result.IsUser);
            #endregion Assert
        }

        /// <summary>
        /// Tests the create get returns view model with only role school admin if school admin and not admin and any others.
        /// </summary>
        [TestMethod]
        public void TestCreateGetReturnsViewModelWithOnlyRoleSchoolAdminIfSchoolAdminAndNotAdminAndAnyOthers()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User, RoleNames.SchoolAdmin, RoleNames.ManageAll });
            SetupDataForCreateTests();
            #endregion Arrange

            #region Act
            var result = Controller.Create(null, null, null, null)
                .AssertViewRendered()
                .WithViewData<QuestionSetViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsAdmin);
            Assert.IsTrue(result.IsSchoolAdmin);
            Assert.IsFalse(result.IsUser);
            #endregion Assert
        }

        /// <summary>
        /// Tests the create get returns view model with only role user if not school admin and not admin.
        /// </summary>
        [TestMethod]
        public void TestCreateGetReturnsViewModelWithOnlyRoleUserIfNotSchoolAdminAndNotAdmin()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.ManageAll });
            SetupDataForCreateTests();
            #endregion Arrange

            #region Act
            var result = Controller.Create(null, null, null, null)
                .AssertViewRendered()
                .WithViewData<QuestionSetViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsAdmin);
            Assert.IsFalse(result.IsSchoolAdmin);
            Assert.IsTrue(result.IsUser);
            #endregion Assert
        }

        /// <summary>
        /// Tests the create get only sets certain values when parameters are passed.
        /// </summary>
        [TestMethod]
        public void TestCreateGetOnlySetsCertainValuesWhenParametersArePassed()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetupDataForCreateTests();
            #endregion Arrange

            #region Act
            var result = Controller.Create(null, null, null, null)
                .AssertViewRendered()
                .WithViewData<QuestionSetViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Transaction);
            Assert.IsFalse(result.Quantity);
            Assert.IsNull(result.Item);
            Assert.IsNull((result.ItemType));
            #endregion Assert
        }

        /// <summary>
        /// Tests the create get redirects to list if passed item id is not null and is not found.
        /// </summary>
        [TestMethod]
        public void TestCreateGetRedirectsToListIfPassedItemIdIsNotNullAndIsNotFound()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetupDataForCreateTests();
            ItemRepository.Expect(a => a.GetNullableByID(1)).Return(null).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Create(1, null, null, null)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.AreEqual("Item not found.", Controller.Message);
            #endregion Assert
        }

        /// <summary>
        /// Tests the create get redirects to list if passed item type id is not null and is not found.
        /// </summary>
        [TestMethod]
        public void TestCreateGetRedirectsToListIfPassedItemTypeIdIsNotNullAndIsNotFound()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetupDataForCreateTests();
            ItemTypeRepository.Expect(a => a.GetNullableByID(1)).Return(null).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Create(null, 1, null, null)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.AreEqual("ItemType not found.", Controller.Message);
            #endregion Assert
        }

        /// <summary>
        /// Tests the create get sets item if item id is passed and found.
        /// </summary>
        [TestMethod]
        public void TestCreateGetSetsItemIfItemIdIsPassedAndFound()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetupDataForCreateTests();
            #endregion Arrange

            #region Act
            var result = Controller.Create(2, null, null, null)
                .AssertViewRendered()
                .WithViewData<QuestionSetViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreSame(Items[1], result.Item);
            Assert.IsNull(result.ItemType);
            Assert.IsFalse(result.Transaction);
            Assert.IsFalse(result.Quantity);
            #endregion Assert
        }

        /// <summary>
        /// Tests the create get sets item type if item type id is passed and found.
        /// </summary>
        [TestMethod]
        public void TestCreateGetSetsItemTypeIfItemTypeIdIsPassedAndFound()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetupDataForCreateTests();
            #endregion Arrange

            #region Act
            var result = Controller.Create(null, 2, null, null)
                .AssertViewRendered()
                .WithViewData<QuestionSetViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreSame(ItemTypes[1], result.ItemType);
            Assert.IsNull(result.Item);
            Assert.IsFalse(result.Transaction);
            Assert.IsFalse(result.Quantity);
            #endregion Assert
        }

        /// <summary>
        /// Tests the create get sets transaction value if passed.
        /// </summary>
        [TestMethod]
        public void TestCreateGetSetsTransactionValueIfPassed1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetupDataForCreateTests();
            #endregion Arrange

            #region Act
            var result = Controller.Create(null, null, true, null)
                .AssertViewRendered()
                .WithViewData<QuestionSetViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.ItemType);
            Assert.IsNull(result.Item);
            Assert.IsTrue(result.Transaction);
            Assert.IsFalse(result.Quantity);
            #endregion Assert
        }
        /// <summary>
        /// Tests the create get sets transaction value if passed.
        /// </summary>
        [TestMethod]
        public void TestCreateGetSetsTransactionValueIfPassed2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetupDataForCreateTests();
            #endregion Arrange

            #region Act
            var result = Controller.Create(null, null, false, null)
                .AssertViewRendered()
                .WithViewData<QuestionSetViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.ItemType);
            Assert.IsNull(result.Item);
            Assert.IsFalse(result.Transaction);
            Assert.IsFalse(result.Quantity);
            #endregion Assert
        }

        /// <summary>
        /// Tests the create get sets quantity value if passed.
        /// </summary>
        [TestMethod]
        public void TestCreateGetSetsQuantityValueIfPassed1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetupDataForCreateTests();
            #endregion Arrange

            #region Act
            var result = Controller.Create(null, null, null, true)
                .AssertViewRendered()
                .WithViewData<QuestionSetViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.ItemType);
            Assert.IsNull(result.Item);
            Assert.IsFalse(result.Transaction);
            Assert.IsTrue(result.Quantity);
            #endregion Assert
        }
        /// <summary>
        /// Tests the create get sets quantity value if passed.
        /// </summary>
        [TestMethod]
        public void TestCreateGetSetsQuantityValueIfPassed2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetupDataForCreateTests();
            #endregion Arrange

            #region Act
            var result = Controller.Create(null, null, null, false)
                .AssertViewRendered()
                .WithViewData<QuestionSetViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.ItemType);
            Assert.IsNull(result.Item);
            Assert.IsFalse(result.Transaction);
            Assert.IsFalse(result.Quantity);
            #endregion Assert
        }
        #endregion Create Get Tests

        #region Create Post Tests

        /// <summary>
        /// Tests the create post assigns the current user to the question set.
        /// </summary>
        [TestMethod]
        public void TestCreatePostAssignsTheCurrentUserToTheQuestionSet()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetupDataForCreateTests();
            var questionSetToCreate = CreateValidEntities.QuestionSet(null);
            questionSetToCreate.User = Users[0];
            questionSetToCreate.SystemReusable = true;
            Assert.AreNotSame(Users[1], questionSetToCreate.User);
            #endregion Arrange

            #region Act
            Controller.Create(null, null, questionSetToCreate, "", null, null);
            #endregion Act

            #region Assert
            Assert.AreSame(Users[1], questionSetToCreate.User);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the create post returns view if passed school is not found when it is college reusable.
        /// </summary>
        [TestMethod]
        public void TestCreatePostReturnsViewIfPassedSchoolIsNotFoundWhenItIsCollegeReusable()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetupDataForCreateTests();
            var questionSetToCreate = CreateValidEntities.QuestionSet(null);
            questionSetToCreate.CollegeReusable = true;
            SchoolRepository.Expect(a => a.GetNullableByID("NotFound")).Return(null).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Create(null, null, questionSetToCreate, "NotFound", null, null)
                .AssertViewRendered()
                .WithViewData<QuestionSetViewModel>();
            #endregion Act

            #region Assert
            QuestionSetRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<QuestionSet>.Is.Anything));
            ItemRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Item>.Is.Anything));
            ItemTypeRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<ItemType>.Is.Anything));
            Controller.ModelState.AssertErrorsAre("CollegeReusableSchool: Must have school if college reusable");
            #endregion Assert		
        }

        /// <summary>
        /// Tests the create post redirects to edit and saves when school is found and is valid.
        /// </summary>
        [TestMethod]
        public void TestCreatePostRedirectsToEditAndSavesWhenSchoolIsFoundAndIsValid()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetupDataForCreateTests();
            var questionSetToCreate = CreateValidEntities.QuestionSet(null);
            questionSetToCreate.CollegeReusable = true;
            SchoolRepository.Expect(a => a.GetNullableByID(Schools[1].Id)).Return(Schools[1]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Create(null, null, questionSetToCreate, Schools[1].Id, null, null)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.Edit(questionSetToCreate.Id));
            #endregion Act

            #region Assert
            QuestionSetRepository.AssertWasCalled(a => a.EnsurePersistent(questionSetToCreate));
            ItemRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Item>.Is.Anything));
            ItemTypeRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<ItemType>.Is.Anything));
            Assert.AreEqual("Question Set has been created successfully.", Controller.Message);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the create post returns view if invalid.
        /// </summary>
        [TestMethod]
        public void TestCreatePostReturnsViewIfInvalid()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetupDataForCreateTests();
            var questionSetToCreate = CreateValidEntities.QuestionSet(null);
            questionSetToCreate.SystemReusable = true;
            questionSetToCreate.Name = " "; //Invalid
            #endregion Arrange

            #region Act
            Controller.Create(null, null, questionSetToCreate, "NotFound", null, null)
                .AssertViewRendered()
                .WithViewData<QuestionSetViewModel>();
            #endregion Act

            #region Assert
            QuestionSetRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<QuestionSet>.Is.Anything));
            ItemRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Item>.Is.Anything));
            ItemTypeRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<ItemType>.Is.Anything));
            Controller.ModelState.AssertErrorsAre("Name: may not be null or empty");
            #endregion Assert
        }
        
        /// <summary>
        /// Tests the create post returns view if item id passed but not transaction or quantity.
        /// </summary>
        [TestMethod]
        public void TestCreatePostReturnsViewIfItemIdPassedButNotTransactionOrQuantity()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetupDataForCreateTests();
            var questionSetToCreate = CreateValidEntities.QuestionSet(null);
            #endregion Arrange

            #region Act
            Controller.Create(2, null, questionSetToCreate, "", null, null)
                .AssertViewRendered()
                .WithViewData<QuestionSetViewModel>();
            #endregion Act

            #region Assert
            QuestionSetRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<QuestionSet>.Is.Anything));
            ItemRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Item>.Is.Anything));
            ItemTypeRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<ItemType>.Is.Anything));
            Controller.ModelState.AssertErrorsAre("Transaction or Quantity must be specified.");
            #endregion Assert		
        }

        /// <summary>
        /// Tests the create post returns view if item id passed and transaction and quantity are both false.
        /// </summary>
        [TestMethod]
        public void TestCreatePostReturnsViewIfItemIdPassedAndTransactionAndQuantityAreBothFalse()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetupDataForCreateTests();
            var questionSetToCreate = CreateValidEntities.QuestionSet(null);
            #endregion Arrange

            #region Act
            Controller.Create(2, null, questionSetToCreate, "", false, false)
                .AssertViewRendered()
                .WithViewData<QuestionSetViewModel>();
            #endregion Act

            #region Assert
            QuestionSetRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<QuestionSet>.Is.Anything));
            ItemRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Item>.Is.Anything));
            ItemTypeRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<ItemType>.Is.Anything));
            Controller.ModelState.AssertErrorsAre("Transaction and Quantity cannot be the same.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the create post returns view if item id passed and transaction and quantity are both true.
        /// </summary>
        [TestMethod]
        public void TestCreatePostReturnsViewIfItemIdPassedAndTransactionAndQuantityAreBothTrue()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetupDataForCreateTests();
            var questionSetToCreate = CreateValidEntities.QuestionSet(null);
            #endregion Arrange

            #region Act
            Controller.Create(2, null, questionSetToCreate, "", true, true)
                .AssertViewRendered()
                .WithViewData<QuestionSetViewModel>();
            #endregion Act

            #region Assert
            QuestionSetRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<QuestionSet>.Is.Anything));
            ItemRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Item>.Is.Anything));
            ItemTypeRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<ItemType>.Is.Anything));
            Controller.ModelState.AssertErrorsAre("Transaction and Quantity cannot be the same.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the create post saves if item id passed and only transaction is true.
        /// </summary>
        [TestMethod]
        public void TestCreatePostSavesIfItemIdPassedAndOnlyTransactionIsTrue()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetupDataForCreateTests();
            var questionSetToCreate = CreateValidEntities.QuestionSet(null);
            #endregion Arrange

            #region Act
            Controller.Create(2, null, questionSetToCreate, "", true, false)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.Edit(questionSetToCreate.Id));
            #endregion Act

            #region Assert
            QuestionSetRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<QuestionSet>.Is.Anything));
            ItemRepository.AssertWasCalled(a => a.EnsurePersistent(Items[1]));
            ItemTypeRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<ItemType>.Is.Anything));
            Assert.IsTrue(Controller.ModelState.IsValid);
            Assert.AreEqual(1, Items[1].QuestionSets.Where(a => a.TransactionLevel).Count());
            Assert.AreEqual(0, Items[1].QuestionSets.Where(a => a.QuantityLevel).Count());
            #endregion Assert		
        }

        /// <summary>
        /// Tests the create post saves if item id passed and only quantity is true.
        /// </summary>
        [TestMethod]
        public void TestCreatePostSavesIfItemIdPassedAndOnlyQuantityIsTrue()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetupDataForCreateTests();
            var questionSetToCreate = CreateValidEntities.QuestionSet(null);
            #endregion Arrange

            #region Act
            Controller.Create(2, null, questionSetToCreate, "", false, true)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.Edit(questionSetToCreate.Id));
            #endregion Act

            #region Assert
            QuestionSetRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<QuestionSet>.Is.Anything));
            ItemRepository.AssertWasCalled(a => a.EnsurePersistent(Items[1]));
            ItemTypeRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<ItemType>.Is.Anything));
            Assert.IsTrue(Controller.ModelState.IsValid);
            Assert.AreEqual(0, Items[1].QuestionSets.Where(a => a.TransactionLevel).Count());
            Assert.AreEqual(1, Items[1].QuestionSets.Where(a => a.QuantityLevel).Count());
            #endregion Assert
        }

        /// <summary>
        /// Tests the create post does not save if name is contact information.
        /// </summary>
        [TestMethod]
        public void TestCreatePostDoesNotSaveIfNameIsContactInformation()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetupDataForCreateTests();
            var questionSetToCreate = CreateValidEntities.QuestionSet(null);
            questionSetToCreate.SystemReusable = true;
            questionSetToCreate.Name = "ContACt INforMAtion"; //Invalid
            #endregion Arrange

            #region Act
            Controller.Create(null, null, questionSetToCreate, "NotFound", null, null)
                .AssertViewRendered()
                .WithViewData<QuestionSetViewModel>();
            #endregion Act

            #region Assert
            QuestionSetRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<QuestionSet>.Is.Anything));
            ItemRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Item>.Is.Anything));
            ItemTypeRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<ItemType>.Is.Anything));
            Controller.ModelState.AssertErrorsAre("Contact Information is reserved for internal system use only.");
            #endregion Assert	
        }

        #endregion Create Post Tests
    }
}
