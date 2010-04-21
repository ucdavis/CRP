using System.Linq;
using System.Web.Mvc;
using CRP.Controllers.Filter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.Attributes;
using UCDArch.Web.Attributes;

namespace CRP.Tests.Controllers.QuestionSetControllerTests
{
    public partial class QuestionSetControllerTests
    {
        #region Reflection Tests

        #region Controller Class Tests
        /// <summary>
        /// Tests the controller inherits from super controller.
        /// </summary>
        [TestMethod]
        public void TestControllerInheritsFromSuperController()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.BaseType.Name;
            #endregion Act

            #region Assert
            Assert.AreEqual("SuperController", result);
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller has only three attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerHasOnlyThreeAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(3, result.Count());
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller has transaction attribute.
        /// </summary>
        [TestMethod]
        public void TestControllerHasTransactionAttribute()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<UseTransactionsByDefaultAttribute>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Count() > 0, "UseTransactionsByDefaultAttribute not found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller has anti forgery token attribute.
        /// </summary>
        [TestMethod]
        public void TestControllerHasAntiForgeryTokenAttribute()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<UseAntiForgeryTokenOnPostByDefault>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Count() > 0, "UseAntiForgeryTokenOnPostByDefault not found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller has handle transactions manually attribute.
        /// </summary>
        [TestMethod]
        public void TestControllerHasAuthorizeAttribute()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<AuthorizeAttribute>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Count() > 0, "AuthorizeAttribute not found.");
            #endregion Assert
        }
        #endregion Controller Class Tests

        #region Controller Method Tests

        /// <summary>
        /// Tests the controller contains expected number of public methods.
        /// </summary>
        [TestMethod]
        public void TestControllerContainsExpectedNumberOfPublicMethods()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetMethods().Where(a => a.DeclaringType == controllerClass);
            #endregion Act

            #region Assert
            Assert.AreEqual(12, result.Count(), "It looks like a method was added or removed from the controller.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method index contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodIndexContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("Index");
            #endregion Arrange

            #region Act
            //var expectedAttribute = controllerMethod.GetCustomAttributes(true).OfType<AcceptPostAttribute>();
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            //Assert.AreEqual(1, expectedAttribute.Count(), "AcceptPostAttribute not found");
            Assert.AreEqual(0, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method list contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodListContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("List");
            #endregion Arrange

            #region Act
            //var expectedAttribute = controllerMethod.GetCustomAttributes(true).OfType<AcceptPostAttribute>();
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            //Assert.AreEqual(1, expectedAttribute.Count(), "AcceptPostAttribute not found");
            Assert.AreEqual(0, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        [TestMethod]
        public void TestControllerMethodDetailsContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("Details");
            #endregion Arrange

            #region Act
            //var expectedAttribute = controllerMethod.GetCustomAttributes(true).OfType<AcceptPostAttribute>();
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            //Assert.AreEqual(1, expectedAttribute.Count(), "AcceptPostAttribute not found");
            Assert.AreEqual(0, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method edit contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodEditContainsExpectedAttributes1()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Edit");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            Assert.AreEqual(2, controllerMethod.Count(), "Over loads for edit changed");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method edit contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodEditContainsExpectedAttributes2()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Edit");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<AcceptPostAttribute>();
            var allAttributes = controllerMethod.ElementAt(1).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "AcceptPostAttribute not found");
            Assert.AreEqual(1, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method create contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodCreateContainsExpectedAttributes1()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Create");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            Assert.AreEqual(2, controllerMethod.Count(), "Over loads for create changed");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method create contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodCreateContainsExpectedAttributes2()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Create");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<AcceptPostAttribute>();
            var allAttributes = controllerMethod.ElementAt(1).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "AcceptPostAttribute not found");
            Assert.AreEqual(2, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method create contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodCreateContainsExpectedAttributes3()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Create");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<HandleTransactionsManuallyAttribute>();
            var allAttributes = controllerMethod.ElementAt(1).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "HandleTransactionsManuallyAttribute not found");
            Assert.AreEqual(2, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method link to item type contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodLinkToItemTypeContainsExpectedAttributes1()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "LinkToItemType");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(0).GetCustomAttributes(true).OfType<AuthorizeAttribute>();
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "AuthorizeAttribute not found");
            Assert.AreEqual("Admin", expectedAttribute.ElementAt(0).Roles);
            Assert.AreEqual(1, allAttributes.Count());
            Assert.AreEqual(2, controllerMethod.Count(), "Over loads for create changed");
            #endregion Assert
        }
        /// <summary>
        /// Tests the controller method link to item type contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodLinkToItemTypeContainsExpectedAttributes2()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "LinkToItemType");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<AcceptPostAttribute>();
            var allAttributes = controllerMethod.ElementAt(1).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "AcceptPostAttribute not found");
            Assert.AreEqual(2, allAttributes.Count());
            #endregion Assert
        }
        /// <summary>
        /// Tests the controller method link to item type contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodLinkToItemTypeContainsExpectedAttributes3()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "LinkToItemType");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<AdminOnlyAttribute>();
            var allAttributes = controllerMethod.ElementAt(1).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "AdminOnlyAttribute not found");
            Assert.AreEqual(2, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method link to item contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodLinkToItemContainsExpectedAttributes1()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "LinkToItem");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(0).GetCustomAttributes(true).OfType<UserOnlyAttribute>();
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "UserOnlyAttribute not found");
            Assert.AreEqual(1, allAttributes.Count());
            Assert.AreEqual(2, controllerMethod.Count(), "Over loads for create changed");
            #endregion Assert
        }
        /// <summary>
        /// Tests the controller method link to item contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodLinkToItemContainsExpectedAttributes2()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "LinkToItem");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<AcceptPostAttribute>();
            var allAttributes = controllerMethod.ElementAt(1).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "AcceptPostAttribute not found");
            Assert.AreEqual(2, allAttributes.Count());
            #endregion Assert
        }
        /// <summary>
        /// Tests the controller method link to item contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodLinkToItemContainsExpectedAttributes3()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "LinkToItem");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<UserOnlyAttribute>();
            var allAttributes = controllerMethod.ElementAt(1).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "UserOnlyAttribute not found");
            Assert.AreEqual(2, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method unlink from item contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodUnlinkFromItemContainsExpectedAttributes1()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("UnlinkFromItem");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.GetCustomAttributes(true).OfType<AcceptPostAttribute>();
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "AcceptPostAttribute not found");
            Assert.AreEqual(2, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method unlink from item contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodUnlinkFromItemContainsExpectedAttributes2()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("UnlinkFromItem");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.GetCustomAttributes(true).OfType<UserOnlyAttribute>();
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "UserOnlyAttribute not found");
            Assert.AreEqual(2, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }
        #endregion Controller Method Tests

        #endregion Reflection Tests
    }
}
