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
    }
}
