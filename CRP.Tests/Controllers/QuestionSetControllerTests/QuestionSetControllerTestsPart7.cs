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
            SchoolRepository.Expect(a => a.GetNullableById("NotFound")).Return(null).Repeat.Any();
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
            SchoolRepository.Expect(a => a.GetNullableById(Schools[1].Id)).Return(Schools[1]).Repeat.Any();
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

        #region ItemId Tests
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
            Assert.AreEqual("Question Set has been created successfully.", Controller.Message);
            #endregion Assert
        }
        #endregion ItemId Tests

        #region ItemTypeId Tests
        /// <summary>
        /// Tests the create post returns view if item type id passed but not transaction or quantity.
        /// </summary>
        [TestMethod]
        public void TestCreatePostReturnsViewIfItemTypeIdPassedButNotTransactionOrQuantity()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetupDataForCreateTests();
            var questionSetToCreate = CreateValidEntities.QuestionSet(null);
            #endregion Arrange

            #region Act
            Controller.Create(null, 2, questionSetToCreate, "", null, null)
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
        /// Tests the create post returns view if item type id passed and transaction and quantity are both false.
        /// </summary>
        [TestMethod]
        public void TestCreatePostReturnsViewIfItemTypeIdPassedAndTransactionAndQuantityAreBothFalse()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetupDataForCreateTests();
            var questionSetToCreate = CreateValidEntities.QuestionSet(null);
            #endregion Arrange

            #region Act
            Controller.Create(null, 2, questionSetToCreate, "", false, false)
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
        /// Tests the create post returns view if item type id passed and transaction and quantity are both true.
        /// </summary>
        [TestMethod]
        public void TestCreatePostReturnsViewIfItemTypeIdPassedAndTransactionAndQuantityAreBothTrue()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetupDataForCreateTests();
            var questionSetToCreate = CreateValidEntities.QuestionSet(null);
            #endregion Arrange

            #region Act
            Controller.Create(null, 2, questionSetToCreate, "", true, true)
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
        /// Tests the create post saves if item type id passed and only transaction is true.
        /// </summary>
        [TestMethod]
        public void TestCreatePostSavesIfItemTypeIdPassedAndOnlyTransactionIsTrue()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetupDataForCreateTests();
            var questionSetToCreate = CreateValidEntities.QuestionSet(null);
            #endregion Arrange

            #region Act
            Controller.Create(null, 2, questionSetToCreate, "", true, false)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.Edit(questionSetToCreate.Id));
            #endregion Act

            #region Assert
            QuestionSetRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<QuestionSet>.Is.Anything));
            ItemRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Item>.Is.Anything));
            ItemTypeRepository.AssertWasCalled(a => a.EnsurePersistent(ItemTypes[1]));
            Assert.IsTrue(Controller.ModelState.IsValid);
            Assert.AreEqual(1, ItemTypes[1].QuestionSets.Where(a => a.TransactionLevel).Count());
            Assert.AreEqual(0, ItemTypes[1].QuestionSets.Where(a => a.QuantityLevel).Count());
            #endregion Assert
        }

        /// <summary>
        /// Tests the create post saves if item type id passed and only quantity is true.
        /// </summary>
        [TestMethod]
        public void TestCreatePostSavesIfItemTypeIdPassedAndOnlyQuantityIsTrue()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetupDataForCreateTests();
            var questionSetToCreate = CreateValidEntities.QuestionSet(null);
            #endregion Arrange

            #region Act
            Controller.Create(null, 2, questionSetToCreate, "", false, true)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.Edit(questionSetToCreate.Id));
            #endregion Act

            #region Assert
            QuestionSetRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<QuestionSet>.Is.Anything));
            ItemRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Item>.Is.Anything));
            ItemTypeRepository.AssertWasCalled(a => a.EnsurePersistent(ItemTypes[1]));
            Assert.IsTrue(Controller.ModelState.IsValid);
            Assert.AreEqual(0, ItemTypes[1].QuestionSets.Where(a => a.TransactionLevel).Count());
            Assert.AreEqual(1, ItemTypes[1].QuestionSets.Where(a => a.QuantityLevel).Count());
            Assert.AreEqual("Question Set has been created successfully.", Controller.Message);
            #endregion Assert
        }
        #endregion ItemTypeId Tests


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

        /// <summary>
        /// Tests the create post redirects to list if item id is passed but not found.
        /// </summary>
        [TestMethod]
        public void TestCreatePostRedirectsToListIfItemIdIsPassedButNotFound()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetupDataForCreateTests();
            var questionSetToCreate = CreateValidEntities.QuestionSet(null);
            ItemRepository.Expect(a => a.GetNullableById(1)).Return(null).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Create(1, null, questionSetToCreate, "", false, true)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            QuestionSetRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<QuestionSet>.Is.Anything));
            ItemRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Item>.Is.Anything));
            ItemTypeRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<ItemType>.Is.Anything));
            Assert.AreEqual("Item not found.", Controller.Message);
            #endregion Assert
        }

        /// <summary>
        /// Tests the create post redirects to list if item type id is passed but not found.
        /// </summary>
        [TestMethod]
        public void TestCreatePostRedirectsToListIfItemTypeIdIsPassedButNotFound()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetupDataForCreateTests();
            var questionSetToCreate = CreateValidEntities.QuestionSet(null);
            ItemTypeRepository.Expect(a => a.GetNullableById(1)).Return(null).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Create(null, 1, questionSetToCreate, "", false, true)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            QuestionSetRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<QuestionSet>.Is.Anything));
            ItemRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Item>.Is.Anything));
            ItemTypeRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<ItemType>.Is.Anything));
            Assert.AreEqual("ItemType not found.", Controller.Message);
            #endregion Assert
        }


        /// <summary>
        /// Tests the create post validates the item.
        /// </summary>
        [TestMethod]
        public void TestCreatePostValidatesTheItem()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetupDataForCreateTests();
            Items[1].Name = " ";//Invalid
            var questionSetToCreate = CreateValidEntities.QuestionSet(null);
            #endregion Arrange

            #region Act
            Controller.Create(2, null, questionSetToCreate, "", true, false)
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
        /// Tests the type of the create post validates the item.
        /// </summary>
        [TestMethod]
        public void TestCreatePostValidatesTheItemType()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetupDataForCreateTests();
            ItemTypes[1].Name = " ";//Invalid
            var questionSetToCreate = CreateValidEntities.QuestionSet(null);
            #endregion Arrange

            #region Act
            Controller.Create(null, 2, questionSetToCreate, "", true, false)
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
        /// Tests the type of the create post requires reusable when not attached to an item or item.
        /// </summary>
        [TestMethod]
        public void TestCreatePostRequiresReusableWhenNotAttachedToAnItemOrItemType()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetupDataForCreateTests();
            var questionSetToCreate = CreateValidEntities.QuestionSet(null);
            #endregion Arrange

            #region Act
            Controller.Create(null, null, questionSetToCreate, "", null, null)
                .AssertViewRendered()
                .WithViewData<QuestionSetViewModel>();
            #endregion Act

            #region Assert
            QuestionSetRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<QuestionSet>.Is.Anything));
            ItemRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Item>.Is.Anything));
            ItemTypeRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<ItemType>.Is.Anything));
            Controller.ModelState.AssertErrorsAre("Question set must be reusable of some sort.");
            #endregion Assert	
        }

        /// <summary>
        /// Tests the create post set user reusable when not admin school admin but user.
        /// </summary>
        [TestMethod]
        public void TestCreatePostSetUserReusableWhenNotAdminSchoolAdminButUser()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });
            SetupDataForCreateTests();
            var questionSetToCreate = CreateValidEntities.QuestionSet(null);
            #endregion Arrange

            #region Act
            Controller.Create(null, null, questionSetToCreate, "", null, null)
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
        /// Tests the create post saves when reusable flag set.
        /// </summary>
        [TestMethod]
        public void TestCreatePostSavesWhenReusableFlagSet()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetupDataForCreateTests();
            var questionSetToCreate = CreateValidEntities.QuestionSet(null);
            questionSetToCreate.SystemReusable = true;
            #endregion Arrange

            #region Act
            Controller.Create(null, null, questionSetToCreate, "", null, null)
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
        /// Tests the create post populates view correctly if create fails.
        /// </summary>
        [TestMethod]
        public void TestCreatePostPopulatesViewCorrectlyIfCreateFails()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetupDataForCreateTests();
            var questionSetToCreate = CreateValidEntities.QuestionSet(null);
            #endregion Arrange

            #region Act
            var result = Controller.Create(2, 2, questionSetToCreate, "", true, true)
                .AssertViewRendered()
                .WithViewData<QuestionSetViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreSame(questionSetToCreate, result.QuestionSet);
            Assert.AreSame(Items[1], result.Item);
            Assert.AreSame(ItemTypes[1], result.ItemType);
            Assert.IsTrue(result.Transaction);
            Assert.IsTrue(result.Quantity);
            Assert.IsTrue(result.IsAdmin);
            #endregion Assert	
        }
        #endregion Create Post Tests
    }
}
