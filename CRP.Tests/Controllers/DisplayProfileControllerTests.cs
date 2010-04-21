using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRP.Controllers;
using CRP.Controllers.Filter;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
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
    public class DisplayProfileControllerTests : ControllerTestBase<DisplayProfileController>
    {
        protected List<DisplayProfile> DisplayProfiles { get; set; }
        protected IRepository<DisplayProfile> DisplayProfileRepository { get; set; }
        protected List<Unit> Units { get; set; }
        protected IRepository<Unit> UnitRepository { get; set; }
        protected List<School> Schools { get; set; }
        protected IRepositoryWithTypedId<School, string> SchoolRepository { get; set; }
        private readonly Type _controllerClass = typeof(DisplayProfileController);

        #region Init

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayProfileControllerTests"/> class.
        /// </summary>
        public DisplayProfileControllerTests()
        {
            DisplayProfiles = new List<DisplayProfile>();
            DisplayProfileRepository = FakeRepository<DisplayProfile>();
            Controller.Repository.Expect(a => a.OfType<DisplayProfile>()).Return(DisplayProfileRepository).Repeat.Any();

            Units = new List<Unit>();
            UnitRepository = FakeRepository<Unit>();
            Controller.Repository.Expect(a => a.OfType<Unit>()).Return(UnitRepository).Repeat.Any();

            Schools = new List<School>();
            

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
            SchoolRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<School, string>>();
            Controller = new TestControllerBuilder().CreateController<DisplayProfileController>(SchoolRepository);
        }

        #endregion Init

        #region Route Tests

        /// <summary>
        /// Tests the index mapping.
        /// </summary>
        [TestMethod]
        public void TestIndexMapping()
        {
            "~/DisplayProfile/Index".ShouldMapTo<DisplayProfileController>(a => a.Index());            
        }

        /// <summary>
        /// Tests the list mapping.
        /// </summary>
        [TestMethod]
        public void TestListMapping()
        {
            "~/DisplayProfile/List".ShouldMapTo<DisplayProfileController>(a => a.List());
        }

        /// <summary>
        /// Tests the create mapping.
        /// </summary>
        [TestMethod]
        public void TestCreateMapping()
        {
            "~/DisplayProfile/Create".ShouldMapTo<DisplayProfileController>(a => a.Create());
        }

        /// <summary>
        /// Tests the create with parameters mapping.
        /// </summary>
        [TestMethod]
        public void TestCreateWithParametersMapping()
        {
            "~/DisplayProfile/Create/".ShouldMapTo<DisplayProfileController>(a => a.Create(new DisplayProfile()),true);
        }

        /// <summary>
        /// Tests the edit mapping.
        /// </summary>
        [TestMethod]
        public void TestEditMapping()
        {
            "~/DisplayProfile/Edit/5".ShouldMapTo<DisplayProfileController>(a => a.Edit(5));
        }

        /// <summary>
        /// Tests the edit with parameters mapping.
        /// </summary>
        [TestMethod]
        public void TestEditWithParametersMapping()
        {
            "~/DisplayProfile/Edit/5".ShouldMapTo<DisplayProfileController>(a => a.Edit(5, new DisplayProfile()), true);
        }

        /// <summary>
        /// Tests the get logo mapping.
        /// </summary>
        [TestMethod]
        public void TestGetLogoMapping()
        {
            "~/DisplayProfile/GetLogo/5".ShouldMapTo<DisplayProfileController>(a => a.GetLogo(5));
        }
        #endregion Route Tests

        #region Index Tests

        /// <summary>
        /// Tests the index returns view.
        /// </summary>
        [TestMethod]
        public void TestIndexReturnsView()
        {
            Controller.Index()
                .AssertActionRedirect()
                .ToAction<DisplayProfileController>(a => a.List());
        }

        #endregion Index Tests

        #region ListTests

        /// <summary>
        /// Tests the list return view with queryable display profile.
        /// </summary>
        [TestMethod]
        public void TestListReturnViewWithQueryableDisplayProfile()
        {
            FakeDisplayProfiles(3);
            DisplayProfileRepository.Expect(a => a.Queryable).Return(DisplayProfiles.AsQueryable()).Repeat.Any();
            var result = Controller.List()
                .AssertViewRendered()
                .WithViewData<IQueryable<DisplayProfile>>();

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count());
        }

        #endregion ListTests

        #region Create Tests

        /// <summary>
        /// Tests the create without parameters returns view.
        /// </summary>
        [TestMethod]
        public void TestCreateWithoutParametersReturnsView()
        {
            FakeUnits(2);
            FakeSchools(3);
            UnitRepository.Expect(a => a.GetAll()).Return(Units).Repeat.Any();
            SchoolRepository.Expect(a => a.GetAll()).Return(Schools).Repeat.Any();
            var result = Controller.Create()
                .AssertViewRendered()
                .WithViewData<DisplayProfileViewModel>();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Units.Count());
            Assert.AreEqual(3, result.Schools.Count());
        }


        /// <summary>
        /// Tests the create with valid data saves.
        /// </summary>
        [TestMethod]
        public void TestCreateWithValidDataSaves1()
        {
            FakeDisplayProfiles(3);
            DisplayProfileRepository.Expect(a => a.Queryable).Return(DisplayProfiles.AsQueryable()).Repeat.Any();
            //Mock two files
            Controller.ControllerContext.HttpContext = new MockHttpContext(2);
            var newDisplayProfile = CreateValidEntities.DisplayProfile(1);
            newDisplayProfile.Unit = CreateValidEntities.Unit(1);
            newDisplayProfile.School = null;

            Controller.Create(newDisplayProfile)
                .AssertActionRedirect()
                .ToAction<DisplayProfileController>(a => a.List());

            DisplayProfileRepository.AssertWasCalled(a => a.EnsurePersistent(newDisplayProfile));
            Assert.AreEqual("Display Profile has been created successfully.", Controller.Message);
            Assert.IsFalse(newDisplayProfile.SchoolMaster);
            Assert.IsNotNull(newDisplayProfile.Logo);
        }
        /// <summary>
        /// Tests the create with valid data saves.
        /// </summary>
        [TestMethod]
        public void TestCreateWithValidDataSaves2()
        {
            //Mock two files
            Controller.ControllerContext.HttpContext = new MockHttpContext(2);
            var newDisplayProfile = CreateValidEntities.DisplayProfile(1);
            newDisplayProfile.Unit = null;
            newDisplayProfile.School = CreateValidEntities.School(1);

            Controller.Create(newDisplayProfile)
                .AssertActionRedirect()
                .ToAction<DisplayProfileController>(a => a.List());

            DisplayProfileRepository.AssertWasCalled(a => a.EnsurePersistent(newDisplayProfile));
            Assert.AreEqual("Display Profile has been created successfully.", Controller.Message);
            Assert.IsTrue(newDisplayProfile.SchoolMaster);
            Assert.IsNotNull(newDisplayProfile.Logo);
        }

        /// <summary>
        /// Tests the create with valid data saves.
        /// </summary>
        [TestMethod]
        public void TestCreateWithValidDataSaves3()
        {
            //Mock No files
            Controller.ControllerContext.HttpContext = new MockHttpContext(0);
            var newDisplayProfile = CreateValidEntities.DisplayProfile(1);
            newDisplayProfile.Unit = null;
            newDisplayProfile.School = CreateValidEntities.School(1);

            Controller.Create(newDisplayProfile)
                .AssertActionRedirect()
                .ToAction<DisplayProfileController>(a => a.List());

            DisplayProfileRepository.AssertWasCalled(a => a.EnsurePersistent(newDisplayProfile));
            Assert.AreEqual("Display Profile has been created successfully.", Controller.Message);
            Assert.IsTrue(newDisplayProfile.SchoolMaster);
            Assert.IsNull(newDisplayProfile.Logo);
        }

        /// <summary>
        /// Tests the create with invalid data does not save.
        /// </summary>
        [TestMethod]
        public void TestCreateWithInvalidDataDoesNotSave1()
        {
            FakeDisplayProfiles(3);
            DisplayProfileRepository.Expect(a => a.Queryable).Return(DisplayProfiles.AsQueryable()).Repeat.Any();
            FakeUnits(1);
            FakeSchools(1);
            UnitRepository.Expect(a => a.GetAll()).Return(Units).Repeat.Any();
            SchoolRepository.Expect(a => a.GetAll()).Return(Schools).Repeat.Any();

            //Mock No files
            Controller.ControllerContext.HttpContext = new MockHttpContext(0);
            var newDisplayProfile = CreateValidEntities.DisplayProfile(1);
            newDisplayProfile.Unit = CreateValidEntities.Unit(1);
            newDisplayProfile.School = CreateValidEntities.School(1);

            var result = Controller.Create(newDisplayProfile)
                .AssertViewRendered()
                .WithViewData<DisplayProfileViewModel>();
            Assert.IsNotNull(result);
            Assert.AreSame(newDisplayProfile, result.DisplayProfile);
            Assert.AreEqual(1, result.Units.Count());
            Assert.AreEqual(1, result.Schools.Count());

            DisplayProfileRepository.AssertWasNotCalled(a => a.EnsurePersistent(newDisplayProfile));
            Assert.AreNotEqual("Display Profile has been created successfully.", Controller.Message);
            Assert.IsFalse(Controller.ModelState.IsValid);
            Controller.ModelState.AssertErrorsAre("UnitAndSchool: Unit and School cannot be selected together.");
        }

        /// <summary>
        /// Tests the create with invalid data does not save.
        /// </summary>
        [TestMethod]
        public void TestCreateWithInvalidDataDoesNotSave2()
        {
            FakeUnits(1);
            FakeSchools(1);
            UnitRepository.Expect(a => a.GetAll()).Return(Units).Repeat.Any();
            SchoolRepository.Expect(a => a.GetAll()).Return(Schools).Repeat.Any();

            //Mock No files
            Controller.ControllerContext.HttpContext = new MockHttpContext(0);
            var newDisplayProfile = CreateValidEntities.DisplayProfile(1);
            newDisplayProfile.Unit = null;
            newDisplayProfile.School = null;

            var result = Controller.Create(newDisplayProfile)
                .AssertViewRendered()
                .WithViewData<DisplayProfileViewModel>();
            Assert.IsNotNull(result);
            Assert.AreSame(newDisplayProfile, result.DisplayProfile);
            Assert.AreEqual(1, result.Units.Count());
            Assert.AreEqual(1, result.Schools.Count());

            DisplayProfileRepository.AssertWasNotCalled(a => a.EnsurePersistent(newDisplayProfile));
            Assert.AreNotEqual("Display Profile has been created successfully.", Controller.Message);
            Assert.IsFalse(Controller.ModelState.IsValid);
            Controller.ModelState.AssertErrorsAre("UnitOrSchool: A Unit or School must be specified.");
        }

        /// <summary>
        /// Tests the create with invalid data does not save.
        /// </summary>
        [TestMethod]
        public void TestCreateWithInvalidDataDoesNotSave3()
        {
            FakeDisplayProfiles(3);
            DisplayProfileRepository.Expect(a => a.Queryable).Return(DisplayProfiles.AsQueryable()).Repeat.Any();
            FakeUnits(1);
            FakeSchools(1);
            UnitRepository.Expect(a => a.GetAll()).Return(Units).Repeat.Any();
            SchoolRepository.Expect(a => a.GetAll()).Return(Schools).Repeat.Any();

            //Mock No files
            Controller.ControllerContext.HttpContext = new MockHttpContext(0);
            var newDisplayProfile = CreateValidEntities.DisplayProfile(1);
            newDisplayProfile.Unit = Units[0];
            newDisplayProfile.School = null;
            newDisplayProfile.Name = " "; //Invalid

            var result = Controller.Create(newDisplayProfile)
                .AssertViewRendered()
                .WithViewData<DisplayProfileViewModel>();
            Assert.IsNotNull(result);
            Assert.AreSame(newDisplayProfile, result.DisplayProfile);
            Assert.AreEqual(1, result.Units.Count());
            Assert.AreEqual(1, result.Schools.Count());

            DisplayProfileRepository.AssertWasNotCalled(a => a.EnsurePersistent(newDisplayProfile));
            Assert.AreNotEqual("Display Profile has been created successfully.", Controller.Message);
            Assert.IsFalse(Controller.ModelState.IsValid);
            Controller.ModelState.AssertErrorsAre("Name: may not be null or empty");
        }

        /// <summary>
        /// Tests the create with duplicate unit does not save.
        /// </summary>
        [TestMethod]
        public void TestCreateWithDuplicateUnitDoesNotSave()
        {
            #region Arrange
            FakeUnits(3);
            FakeDisplayProfiles(3);
            for (int i = 0; i < 3; i++)
            {
                DisplayProfiles[i].Unit = Units[i];
            }
            DisplayProfileRepository.Expect(a => a.Queryable).Return(DisplayProfiles.AsQueryable()).Repeat.Any();
            FakeSchools(1);
            UnitRepository.Expect(a => a.GetAll()).Return(Units).Repeat.Any();
            SchoolRepository.Expect(a => a.GetAll()).Return(Schools).Repeat.Any();
            Controller.ControllerContext.HttpContext = new MockHttpContext(0);
            var newDisplayProfile = CreateValidEntities.DisplayProfile(4);
            newDisplayProfile.Unit = Units[1];
            #endregion Arrange

            #region Act
            var result = Controller.Create(newDisplayProfile)
                .AssertViewRendered()
                .WithViewData<DisplayProfileViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            DisplayProfileRepository.AssertWasNotCalled(a => a.EnsurePersistent(newDisplayProfile));
            Assert.AreNotEqual("Display Profile has been created successfully.", Controller.Message);
            Assert.IsFalse(Controller.ModelState.IsValid);
            Controller.ModelState.AssertErrorsAre("Display Profile has already been created for this unit.");
            #endregion Assert		
        }
        #endregion Create Tests

        #region Edit Tests

        /// <summary>
        /// Tests the edit with one parameter and id not found redirects to list.
        /// </summary>
        [TestMethod]
        public void TestEditWithOneParameterAndIdNotFoundRedirectsToList()
        {
            DisplayProfileRepository.Expect(a => a.GetNullableByID(1)).Return(null).Repeat.Any();
            Controller.Edit(1)
                .AssertActionRedirect()
                .ToAction<DisplayProfileController>(a => a.List());
        }

        /// <summary>
        /// Tests the edit with one parameter and id found returns view.
        /// </summary>
        [TestMethod]
        public void TestEditWithOneParameterAndIdFoundReturnsView()
        {
            FakeDisplayProfiles(1);
            DisplayProfileRepository.Expect(a => a.GetNullableByID(1)).Return(DisplayProfiles[0]).Repeat.Any();
            var result = Controller.Edit(1)
                .AssertViewRendered()
                .WithViewData<DisplayProfile>();
            Assert.IsNotNull(result);
            Assert.AreSame(DisplayProfiles[0], result);
        }

        /// <summary>
        /// Tests the edit when id not found.
        /// </summary>
        [TestMethod]
        public void TestEditWhenIdNotFound()
        {
            FakeDisplayProfiles(1);
            DisplayProfileRepository.Expect(a => a.GetNullableByID(1)).Return(null).Repeat.Any();
            Controller.Edit(1, DisplayProfiles[0])
                .AssertActionRedirect()
                .ToAction<DisplayProfileController>(a => a.List());
        }

        /// <summary>
        /// Tests the edit with valid data saves.
        /// </summary>
        [TestMethod]
        public void TestEditWithValidDataSaves()
        {
            //Mock one file
            Controller.ControllerContext.HttpContext = new MockHttpContext(1);
            var updateDisplayProfile = CreateValidEntities.DisplayProfile(99);
            FakeDisplayProfiles(3);
            DisplayProfileRepository.Expect(a => a.GetNullableByID(2)).Return(DisplayProfiles[1]).Repeat.Any();

            Controller.Edit(2, updateDisplayProfile)
                .AssertActionRedirect()
                .ToAction<DisplayProfileController>(a => a.List());
            DisplayProfileRepository.AssertWasCalled(a => a.EnsurePersistent(DisplayProfiles[1]));
            Assert.AreEqual("Display Profile has been saved successfully.", Controller.Message);
            Assert.IsNotNull(DisplayProfiles[1].Logo);
            Assert.AreEqual(updateDisplayProfile.Name, DisplayProfiles[1].Name);
        }

        /// <summary>
        /// Tests the edit does not change unit.
        /// </summary>
        [TestMethod]
        public void TestEditDoesNotChangeUnit()
        {
            //Mock one file
            Controller.ControllerContext.HttpContext = new MockHttpContext(1);
            FakeUnits(2);
            FakeDisplayProfiles(1);
            var updateDisplayProfile = CreateValidEntities.DisplayProfile(99);
            DisplayProfileRepository.Expect(a => a.GetNullableByID(1)).Return(DisplayProfiles[0]).Repeat.Any();
            DisplayProfiles[0].Unit = Units[0];
            updateDisplayProfile.Unit = Units[1];
            Assert.AreNotSame(DisplayProfiles[0].Unit, updateDisplayProfile.Unit);
            Assert.AreNotEqual(DisplayProfiles[0].Unit.FullName, updateDisplayProfile.Unit.FullName);
            Assert.AreNotEqual(DisplayProfiles[0].Name, updateDisplayProfile.Name);

            Controller.Edit(1, updateDisplayProfile)
                .AssertActionRedirect()
                .ToAction<DisplayProfileController>(a => a.List());
            DisplayProfileRepository.AssertWasCalled(a => a.EnsurePersistent(DisplayProfiles[0]));
            Assert.AreEqual("Display Profile has been saved successfully.", Controller.Message);
            Assert.IsNotNull(DisplayProfiles[0].Logo);
            Assert.AreEqual(updateDisplayProfile.Name, DisplayProfiles[0].Name);
            Assert.AreNotSame(DisplayProfiles[0].Unit, updateDisplayProfile.Unit);
            Assert.AreNotEqual(DisplayProfiles[0].Unit.FullName, updateDisplayProfile.Unit.FullName);
        }

        /// <summary>
        /// Tests the edit does not change school.
        /// </summary>
        [TestMethod]
        public void TestEditDoesNotChangeSchool()
        {
            //Mock one file
            Controller.ControllerContext.HttpContext = new MockHttpContext(1);
            FakeSchools(2);
            FakeDisplayProfiles(1);
            var updateDisplayProfile = CreateValidEntities.DisplayProfile(99);
            DisplayProfileRepository.Expect(a => a.GetNullableByID(1)).Return(DisplayProfiles[0]).Repeat.Any();
            DisplayProfiles[0].School = Schools[0];
            DisplayProfiles[0].Unit = null;
            DisplayProfiles[0].SchoolMaster = true;
            updateDisplayProfile.School = Schools[1];
            updateDisplayProfile.Unit = null;
            updateDisplayProfile.SchoolMaster = true;
            Assert.AreNotSame(DisplayProfiles[0].School, updateDisplayProfile.School);
            Assert.AreNotEqual(DisplayProfiles[0].School.LongDescription, updateDisplayProfile.School.LongDescription);
            Assert.AreNotEqual(DisplayProfiles[0].Name, updateDisplayProfile.Name);

            Controller.Edit(1, updateDisplayProfile)
                .AssertActionRedirect()
                .ToAction<DisplayProfileController>(a => a.List());
            DisplayProfileRepository.AssertWasCalled(a => a.EnsurePersistent(DisplayProfiles[0]));
            Assert.AreEqual("Display Profile has been saved successfully.", Controller.Message);
            Assert.IsNotNull(DisplayProfiles[0].Logo);
            Assert.AreEqual(updateDisplayProfile.Name, DisplayProfiles[0].Name);
            Assert.AreNotSame(DisplayProfiles[0].School, updateDisplayProfile.School);
            Assert.AreNotEqual(DisplayProfiles[0].School.LongDescription, updateDisplayProfile.School.LongDescription);
        }      

        #endregion Edit Tests

        #region GetLogo Tests

        /// <summary>
        /// Tests the get logo does not throw exception when id not found.
        /// </summary>
        [TestMethod, Ignore]
        public void TestGetLogoDoesNotThrowExceptionWhenIdNotFound()
        {
            DisplayProfileRepository.Expect(a => a.GetNullableByID(1)).Return(null).Repeat.Any();
            Controller.GetLogo(1);
            Assert.Inconclusive("Once GetLogo test gets to here, assert what is returned and remove this line.");
        }

        /// <summary>
        /// Tests the get logo when id found but logo is null.
        /// </summary>
        [TestMethod, Ignore]
        public void TestGetLogoWhenIdFoundButLogoIsNull()
        {
            FakeDisplayProfiles(1);
            DisplayProfileRepository.Expect(a => a.GetNullableByID(1)).Return(DisplayProfiles[0]).Repeat.Any();
            DisplayProfiles[0].Logo = null;
            var result = Controller.GetLogo(1);
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Tests the get logo returns the file contents of logo when it is not null.
        /// </summary>
        [TestMethod, Ignore]
        public void TestGetLogoReturnsTheFileContentsOfLogoWhenItIsNotNull()
        {
            FakeDisplayProfiles(1);
            DisplayProfiles[0].Logo = new byte[]{0,5,3,2};
            DisplayProfileRepository.Expect(a => a.GetNullableByID(1)).Return(DisplayProfiles[0]).Repeat.Any();
            var result = Controller.GetLogo(1).AssertResultIs<FileContentResult>();
            Assert.IsNotNull(result);
            Assert.AreEqual("image/jpg", result.ContentType);
            Assert.AreEqual("0532", result.FileContents.ByteArrayToString());
        }
        #endregion GetLogo Tests


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
        /// Tests the controller has only two attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerHasOnlyTWoAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
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
            Assert.AreEqual(7, result.Count(), "It looks like a method was added or removed from the controller.");
            #endregion Assert
        }
       

        /// <summary>
        /// Tests the controller method Index contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodIndexContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("Index");
            #endregion Arrange

            #region Act
            var result = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, result.Count());
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
            var expectedAttribute = controllerMethod.GetCustomAttributes(true).OfType<AdminOnlyAttribute>();
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "AdminOnlyAttribute not found");
            Assert.AreEqual(1, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }        

        /// <summary>
        /// Tests the controller method create contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodCreateContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Create");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(0).GetCustomAttributes(true).OfType<AdminOnlyAttribute>();
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "AdminOnlyAttribute not found");
            Assert.AreEqual(1, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }
        
        /// <summary>
        /// Tests the controller method create contains expected attributes2.
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
        /// Tests the controller method create contains expected attributes2.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodCreateContainsExpectedAttributes3()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Create");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<AuthorizeAttribute>();
            var allAttributes = controllerMethod.ElementAt(1).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "AuthorizeAttribute not found");
            Assert.AreEqual("Admin", expectedAttribute.ElementAt(0).Roles);
            Assert.AreEqual(2, allAttributes.Count(), "More than expected custom attributes found.");
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
            var expectedAttribute = controllerMethod.ElementAt(0).GetCustomAttributes(true).OfType<AuthorizeAttribute>();
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "AuthorizeAttribute not found");
            Assert.AreEqual("Admin", expectedAttribute.ElementAt(0).Roles);
            Assert.AreEqual(1, allAttributes.Count(), "More than expected custom attributes found.");
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
            var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<AuthorizeAttribute>();
            var allAttributes = controllerMethod.ElementAt(1).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "AuthorizeAttribute not found");
            Assert.AreEqual("Admin", expectedAttribute.ElementAt(0).Roles);
            Assert.AreEqual(2, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }
        /// <summary>
        /// Tests the controller method edit contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodEditContainsExpectedAttributes3()
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
        /// <summary>
        /// Tests the controller method get logo contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodGetLogoContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("GetLogo");
            #endregion Arrange

            #region Act
            var result = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, result.Count());
            #endregion Assert
        }   
        #endregion Controller Method Tests

        #endregion Reflection Tests


        #region Helper Methods

        /// <summary>
        /// Fakes the display profiles.
        /// </summary>
        /// <param name="count">The count.</param>
        private void FakeDisplayProfiles(int count)
        {
            var offSet = DisplayProfiles.Count;
            for (int i = 0; i < count; i++)
            {
                DisplayProfiles.Add(CreateValidEntities.DisplayProfile(i + 1 + offSet));
                DisplayProfiles[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }

        /// <summary>
        /// Fakes the schools.
        /// </summary>
        /// <param name="count">The count.</param>
        private void FakeSchools(int count)
        {
            var offSet = Schools.Count;
            for (int i = 0; i < count; i++)
            {
                Schools.Add(CreateValidEntities.School(i + 1 + offSet));
                Schools[i + offSet].SetIdTo((i + 1 + offSet).ToString());
            }
        }

        /// <summary>
        /// Fakes the units.
        /// </summary>
        /// <param name="count">The count.</param>
        private void FakeUnits(int count)
        {
            var offSet = Units.Count;
            for (int i = 0; i < count; i++)
            {
                Units.Add(CreateValidEntities.Unit(i + 1 + offSet));
                Units[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }

        #region File Mocks

        /// <summary>
        /// Mock the HttpContext. Used for getting the current user name
        /// </summary>
        public class MockHttpContext : HttpContextBase
        {
            private int _count;
            public MockHttpContext(int count)
            {
                _count = count;
            }

            //private IPrincipal _user;

            //Don't need to do this in these tests, just the file stuff.
            //public override IPrincipal User
            //{
            //    get
            //    {
            //        if (_user == null)
            //        {
            //            _user = new MockPrincipal();
            //        }
            //        return _user;
            //    }
            //    set
            //    {
            //        _user = value;
            //    }
            //}

            public override HttpRequestBase Request
            {
                get
                {
                    return new MockHttpRequest(_count);
                }
            }

        }

        public class MockHttpRequest : HttpRequestBase
        {
            MockHttpFileCollectionBase _mocked { get; set; }

            public MockHttpRequest(int count)
            {
                _mocked = new MockHttpFileCollectionBase(count);
            }
            public override HttpFileCollectionBase Files
            {
                get
                {
                    return _mocked;
                }
            }
        }

        public class MockHttpFileCollectionBase : HttpFileCollectionBase
        {
            public int Counter { get; set; }

            public MockHttpFileCollectionBase(int count)
            {
                Counter = count;
                for (int i = 0; i < count; i++)
                {
                    BaseAdd("Test" + (i + 1), new byte[5]);
                }

            }
      
            public override int Count
            {
                get
                {
                    return Counter;
                }
            }
            public override HttpPostedFileBase Get(string name)
            {
                return new MockHttpPostedFileBase();
            }
            public override HttpPostedFileBase this[string name]
            {
                get
                {
                    return new MockHttpPostedFileBase();
                }
            }
            public override HttpPostedFileBase this[int index]
            {
                get
                {
                    return new MockHttpPostedFileBase();
                }
            }
        }

        public class MockHttpPostedFileBase : HttpPostedFileBase
        {
            public override int ContentLength
            {
                get
                {
                    return 5;
                }
            }
            public override string FileName
            {
                get
                {
                    return "Mocked File Name";
                }
            }
            public override System.IO.Stream InputStream
            {
                get
                {
                    var memStream = new MemoryStream(new byte[5]);
                    return memStream;
                }
            }
        }

        #endregion File Mocks

        #endregion Helper Methods
    }
}
