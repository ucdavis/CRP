using System;
using System.Collections.Generic;
using System.Linq;
using CRP.Controllers;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using CRP.Tests.Core.Extensions;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;

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
                .AssertViewRendered();
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
        //[TestMethod]
        //public void TestCreateWithoutParametersReturnsView()
        //{
        //    FakeUnits(2);
        //    FakeSchools(3);
        //    Controller.Create()
        //        .AssertViewRendered()
        //        .WithViewData<DisplayProfileViewModel>();
        //}       

        #endregion Create Tests


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

        ///// <summary>
        ///// Fakes the schools.
        ///// </summary>
        ///// <param name="count">The count.</param>
        //private void FakeSchools(int count)
        //{
        //    var offSet = Schools.Count;
        //    for (int i = 0; i < count; i++)
        //    {
        //        Schools.Add(CreateValidEntities.School(i + 1 + offSet));
        //        //Schools[i + offSet].SetIdTo(i + 1 + offSet);
        //    }
        //}

        ///// <summary>
        ///// Fakes the units.
        ///// </summary>
        ///// <param name="count">The count.</param>
        //private void FakeUnits(int count)
        //{
        //    var offSet = Units.Count;
        //    for (int i = 0; i < count; i++)
        //    {
        //        Units.Add(CreateValidEntities.Unit(i + 1 + offSet));
        //        Units[i + offSet].SetIdTo(i + 1 + offSet);
        //    }
        //}

        #endregion Helper Methods
    }
}
