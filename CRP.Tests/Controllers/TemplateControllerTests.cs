using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CRP.Controllers;
using CRP.Controllers.Filter;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using CRP.Core.Resources;
using CRP.Tests.Core.Extensions;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.Attributes;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Web.Attributes;

namespace CRP.Tests.Controllers
{
    [TestClass]
    public class TemplateControllerTests : ControllerTestBase<TemplateController>
    {
        private readonly Type _controllerClass = typeof(TemplateController);
        protected List<Template> Templates { get; set; }
        protected IRepository<Template> TemplateRepository { get; set; }        

        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateControllerTests"/> class.
        /// </summary>
        public TemplateControllerTests()
        {
            Templates = new List<Template>();
            TemplateRepository = FakeRepository<Template>();
            Controller.Repository.Expect(a => a.OfType<Template>()).Return(TemplateRepository).Repeat.Any();
        }
        /// <summary>
        /// Registers the routes.
        /// </summary>
        protected override void RegisterRoutes()
        {
            new RouteConfigurator().RegisterRoutes();
        }

        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            Controller = new TestControllerBuilder().CreateController<TemplateController>();
        }

        #endregion Init

        #region Mapping/Route Tests

        /// <summary>
        /// Tests the edit get mapping.
        /// </summary>
        [TestMethod]
        public void TestEditGetMapping()
        {
            "~/Template/Edit".ShouldMapTo<TemplateController>(a => a.Edit());	
        }

        /// <summary>
        /// Tests the edit post mapping.
        /// </summary>
        [TestMethod]
        public void TestEditPostMapping()
        {
            "~/Template/Edit/Text".ShouldMapTo<TemplateController>(a => a.Edit("Text", "text2"), true);	
        }
        #endregion Mapping/Route Tests

        #region Edit Get Tests

        /// <summary>
        /// Tests the edit get when no template is found creates A new template and returns view.
        /// </summary>
        [TestMethod]
        public void TestEditGetWhenNoTemplateIsFoundCreatesANewTemplateAndReturnsView()
        {
            #region Arrange
            ControllerRecordFakes.FakeTemplates(Templates, 3);
            TemplateRepository.Expect(a => a.Queryable).Return(Templates.AsQueryable()).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Edit()
                .AssertViewRendered()
                .WithViewData<ConfirmationTemplateViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Template.Id);
            Assert.IsNull(result.Template.Text);
            Assert.AreEqual(string.Empty, result.PaidText);
            Assert.AreEqual(string.Empty, result.UnpaidText);
            #endregion Assert		
        }


        /// <summary>
        /// Tests the edit get returns the first default template in A view.
        /// </summary>
        [TestMethod]
        public void TestEditGetReturnsTheFirstDefaultTemplateInAView()
        {
            #region Arrange
            ControllerRecordFakes.FakeTemplates(Templates, 5);
            Templates[3].Default = true;
            Templates[4].Default = true;
            TemplateRepository.Expect(a => a.Queryable).Return(Templates.AsQueryable()).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Edit()
                .AssertViewRendered()
                .WithViewData<ConfirmationTemplateViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Template.Id);
            Assert.AreEqual(Templates[3].Text, result.PaidText);
            Assert.AreSame(Templates[3], result.Template);
            #endregion Assert		
        }
        #endregion Edit Get Tests

        #region Edit Post Tests

        /// <summary>
        /// Tests the edit post when A default template is found and text is valid template is updated.
        /// </summary>
        [TestMethod]
        public void TestEditPostWhenADefaultTemplateIsFoundAndTextIsValidTemplateIsUpdated()
        {
            #region Arrange
            ControllerRecordFakes.FakeTemplates(Templates, 5);
            Templates[3].Default = true;
            Templates[4].Default = true;
            TemplateRepository.Expect(a => a.Queryable).Return(Templates.AsQueryable()).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Edit("Updated Template Text", string.Empty)
                .AssertViewRendered()
                .WithViewData<ConfirmationTemplateViewModel>();
            #endregion Act

            #region Assert
            TemplateRepository.AssertWasCalled(a => a.EnsurePersistent(Templates[3]));
            Assert.AreEqual("Template has been saved successfully.", Controller.Message);
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Template.Id);
            Assert.AreEqual(Templates[3].Text, result.PaidText + StaticValues.ConfirmationTemplateDelimiter);
            Assert.AreSame(Templates[3], result.Template);
            Assert.AreEqual("Updated Template Text", result.PaidText);
            #endregion Assert		
        }

        [TestMethod]
        public void TestEditPostWhenADefaultTemplateIsFoundAndTextIsValidTemplateIsUpdated2()
        {
            #region Arrange
            ControllerRecordFakes.FakeTemplates(Templates, 5);
            Templates[3].Default = true;
            Templates[4].Default = true;
            TemplateRepository.Expect(a => a.Queryable).Return(Templates.AsQueryable()).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Edit("Updated Template Text", "Updated Unpaid Text")
                .AssertViewRendered()
                .WithViewData<ConfirmationTemplateViewModel>();
            #endregion Act

            #region Assert
            TemplateRepository.AssertWasCalled(a => a.EnsurePersistent(Templates[3]));
            Assert.AreEqual("Template has been saved successfully.", Controller.Message);
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Template.Id);
            Assert.AreEqual(Templates[3].Text, "Updated Template Text" + StaticValues.ConfirmationTemplateDelimiter + "Updated Unpaid Text");
            Assert.AreSame(Templates[3], result.Template);
            Assert.AreEqual("Updated Template Text", result.PaidText);
            Assert.AreEqual("Updated Unpaid Text", result.UnpaidText);
            #endregion Assert
        }

        /// <summary>
        /// Tests the edit post when A default template is not found creates A new default template.
        /// </summary>
        [TestMethod]
        public void TestEditPostWhenADefaultTemplateIsNotFoundCreatesANewDefaultTemplate()
        {
            #region Arrange
            ControllerRecordFakes.FakeTemplates(Templates, 3);
            TemplateRepository.Expect(a => a.Queryable).Return(Templates.AsQueryable()).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Edit("New Template Text", string.Empty)
                .AssertViewRendered()
                .WithViewData<ConfirmationTemplateViewModel>();
            #endregion Act

            #region Assert
            TemplateRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Template>.Is.Anything));
            Assert.AreEqual("Template has been saved successfully.", Controller.Message);
            Assert.IsNotNull(result);
            Assert.AreEqual("New Template Text", result.PaidText);
            Assert.IsTrue(result.Template.Default);
            foreach (var template in Templates)
            {
                Assert.IsFalse(template.Default, "An existing template was updated in error.");
                Assert.AreNotEqual("New Template Text", template.Text, "An existing template was updated in error.");
                Assert.AreNotEqual("New Template Text" + StaticValues.ConfirmationTemplateDelimiter, template.Text, "An existing template was updated in error.");
            }
            #endregion Assert		
        }
        
        /// <summary>
        /// Tests the edit post does not save with invalid text.
        /// </summary>
        [TestMethod]
        public void TestEditPostDoesNotSaveWithInvalidText()
        {
            #region Arrange
            ControllerRecordFakes.FakeTemplates(Templates, 5);
            Templates[3].Default = true;
            TemplateRepository.Expect(a => a.Queryable).Return(Templates.AsQueryable()).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Edit("   ", string.Empty)
                .AssertViewRendered()
                .WithViewData<ConfirmationTemplateViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            TemplateRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Template>.Is.Anything));
            Assert.AreEqual("Template was unable to update.", Controller.Message);
            Controller.ModelState.AssertErrorsAre("text may not be null or empty");
            #endregion Assert		
        }

        #endregion Edit Post Tests

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
        /// Tests the controller has admin only attribute.
        /// </summary>
        [TestMethod]
        public void TestControllerHasAdminOnlyAttribute()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<AdminOnlyAttribute>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Count() > 0, "AdminOnlyAttribute not found.");
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
            Assert.AreEqual(2, result.Count(), "It looks like a method was added or removed from the controller.");
            #endregion Assert
        }

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
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method create contains expected attributes2.
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
            Assert.AreEqual(2, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        [TestMethod]
        public void TestControllerMethodEditContainsExpectedAttributes3()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Edit");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<ValidateInputAttribute>();
            var allAttributes = controllerMethod.ElementAt(1).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "ValidateInputAttribute not found");
            Assert.IsFalse(expectedAttribute.ElementAt(0).EnableValidation, "ValidateInputAttribute Param should be false");
            Assert.AreEqual(2, allAttributes.Count());
            #endregion Assert
        }
        #endregion Controller Method Tests

        #endregion Reflection Tests
    }
}
