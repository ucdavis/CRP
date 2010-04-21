using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CRP.Controllers;
using UCDArch.Web.Controller;

namespace CRP.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest : SuperController
    {
        [TestMethod]
        public void Index()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            //ViewDataDictionary viewData = result.ViewData;
            //Assert.AreEqual("Welcome to ASP.NET MVC!", viewData["Message"]);
            Assert.IsNotNull(result);

        }

        [TestMethod]
        public void About()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.About() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
