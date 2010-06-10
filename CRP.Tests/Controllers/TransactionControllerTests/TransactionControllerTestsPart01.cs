using System.Linq;
using CRP.Controllers.Filter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.Attributes;
using UCDArch.Web.Attributes;

namespace CRP.Tests.Controllers.TransactionControllerTests
{
    /// <summary>
    /// Transaction Controller Tests 
    /// </summary>
    public partial class TransactionControllerTests
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
            var controllerClass = ControllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.BaseType.Name;
            #endregion Act

            #region Assert
            Assert.AreEqual("SuperController", result);
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller has only two attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerHasOnlyTwoAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(2, result.Count());
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller has transaction attribute.
        /// </summary>
        [TestMethod]
        public void TestControllerHasTransactionAttribute()
        {
            #region Arrange
            var controllerClass = ControllerClass;
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
            var controllerClass = ControllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<UseAntiForgeryTokenOnPostByDefault>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Count() > 0, "UseAntiForgeryTokenOnPostByDefault not found.");
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
            var controllerClass = ControllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetMethods().Where(a => a.DeclaringType == controllerClass);
            #endregion Act

            #region Assert
            Assert.AreEqual(16, result.Count(), "It looks like a method was added or removed from the controller.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method checkout get contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodCheckoutGetContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Checkout");
            var index = 99;
            if (controllerMethod.ElementAt(0).GetCustomAttributes(true).Count() == 0)
            {
                index = 0;
            }
            else if (controllerMethod.ElementAt(1).GetCustomAttributes(true).Count() == 0)
            {
                index = 1;
            }
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.ElementAt(index).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }


        /// <summary>
        /// Tests the controller method checkout post contains expected attributes1.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodCheckoutPostContainsExpectedAttributes1()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Checkout");
            var index = 99;
            if (controllerMethod.ElementAt(0).GetCustomAttributes(true).Count() == 2)
            {
                index = 0;
            }
            else if (controllerMethod.ElementAt(1).GetCustomAttributes(true).Count() == 2)
            {
                index = 1;
            }

            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(index).GetCustomAttributes(true).OfType<AcceptPostAttribute>();
            var allAttributes = controllerMethod.ElementAt(index).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "AcceptPostAttribute not found");
            Assert.AreEqual(2, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method checkout post contains expected attributes2.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodCheckoutPostContainsExpectedAttributes2()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Checkout");
            var index = 99;
            if (controllerMethod.ElementAt(0).GetCustomAttributes(true).Count() == 2)
            {
                index = 0;
            }
            else if (controllerMethod.ElementAt(1).GetCustomAttributes(true).Count() == 2)
            {
                index = 1;
            }
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(index).GetCustomAttributes(true).OfType<CaptchaValidatorAttribute>();
            var allAttributes = controllerMethod.ElementAt(index).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "CaptchaValidatorAttribute not found");
            Assert.AreEqual(2, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }


        /// <summary>
        /// Tests the controller method confirmation contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodConfirmationContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethod("Confirmation");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method payment success contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodPaymentSuccessContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethod("PaymentSuccess");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method payment cancel contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodPaymentCancelContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethod("PaymentCancel");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method payment error contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodPaymentErrorContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethod("PaymentError");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }


        /// <summary>
        /// Tests the controller method calculate validation string contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodCalculateValidationStringContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethod("CalculateValidationString");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method edit get contains expected attributes1.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodEditGetContainsExpectedAttributes1()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Edit");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(0).GetCustomAttributes(true).OfType<AnyoneWithRoleAttribute>();
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "AnyoneWithRoleAttribute not found");
            Assert.AreEqual(1, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method edit post contains expected attributes1.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodEditPostContainsExpectedAttributes1()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Edit");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<AnyoneWithRoleAttribute>();
            var allAttributes = controllerMethod.ElementAt(1).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "AnyoneWithRoleAttribute not found");
            Assert.AreEqual(2, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method edit post contains expected attributes2.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodEditPostContainsExpectedAttributes2()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Edit");
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
        /// Tests the controller method payment result contains expected attributes1.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodPaymentResultContainsExpectedAttributes1()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethod("PaymentResult");
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
        /// Tests the controller method payment result contains expected attributes2.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodPaymentResultContainsExpectedAttributes2()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethod("PaymentResult");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.GetCustomAttributes(true).OfType<BypassAntiForgeryTokenAttribute>();
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "BypassAntiForgeryTokenAttribute not found");
            Assert.AreEqual(2, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method lookup get contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodLookupGetContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Lookup");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method lookup post contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodLookupPostContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Lookup");
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
        /// Tests the controller method refund get contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodRefundGetContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Refund");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(0).GetCustomAttributes(true).OfType<RefunderOnlyAttribute>();
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "RefunderOnlyAttribute not found");
            Assert.AreEqual(1, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method refund post contains expected attributes1.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodRefundPostContainsExpectedAttributes1()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Refund");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<RefunderOnlyAttribute>();
            var allAttributes = controllerMethod.ElementAt(1).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "RefunderOnlyAttribute not found");
            Assert.AreEqual(2, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method refund post contains expected attributes2.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodRefundPostContainsExpectedAttributes2()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Refund");
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
        /// Tests the controller method details refund contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodDetailsRefundContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = ControllerClass;
            var controllerMethod = controllerClass.GetMethod("DetailsRefund");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }
        #endregion Controller Method Tests

        #endregion Reflection Tests
    }
}
