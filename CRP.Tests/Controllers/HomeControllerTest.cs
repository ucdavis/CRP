using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CRP.Controllers;
using UCDArch.Web.Controller;

namespace CRP.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest : SuperController
    {

        /// <summary>
        /// Tests the index.
        /// </summary>
        [TestMethod]
        public void TestIndex()
        {
            // Arrange
            var controller = new HomeController();

            // Act
            var result = controller.Index() as ViewResult;

            // Assert
            //ViewDataDictionary viewData = result.ViewData;
            //Assert.AreEqual("Welcome to ASP.NET MVC!", viewData["Message"]);
            Assert.IsNotNull(result);

        }

        /// <summary>
        /// Tests the about.
        /// </summary>
        [TestMethod]
        public void TestAbout()
        {
            // Arrange
            var controller = new HomeController();

            // Act
            var result = controller.About() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
