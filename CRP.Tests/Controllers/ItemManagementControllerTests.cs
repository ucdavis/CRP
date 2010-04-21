using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Web;
using CRP.Controllers;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using CRP.Core.Resources;
using CRP.Tests.Core.Extensions;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Web.ActionResults;

namespace CRP.Tests.Controllers
{
    /// <summary>
    /// ItemManagement Controller Tests
    /// </summary>
    [TestClass]
    public class ItemManagementControllerTests : ControllerTestBase<ItemManagementController>
    {
        protected readonly IPrincipal Principal = new MockPrincipal();
        protected List<User> Users { get; set; }
        protected IRepository<User> UserRepository { get; set; }
        protected List<Item> Items { get; set; }
        protected IRepository<Item> ItemRepository { get; set; }
        protected List<Editor> Editors { get; set; }
        protected IRepository<Editor> EditorRepository { get; set; }
        protected List<ItemType> ItemTypes { get; set; }
        protected IRepository<ItemType> ItemTypeRepository { get; set; }
        protected List<ExtendedProperty> ExtendedProperties { get; set; }
        protected IRepository<ExtendedProperty> ExtendedPropertyRepository { get; set; }
        protected List<Tag> Tags { get; set; }
        protected IRepository<Tag> TagRepository { get; set; }
        protected List<QuestionSet> QuestionSets { get; set; }
        protected IRepository<QuestionSet> QuestionSetRepository { get; set; }
        protected List<Unit> Units { get; set; }
        protected IRepository<Unit> UnitRepository { get; set; }
        protected List<ItemTypeQuestionSet> ItemTypeQuestionSets { get; set; }
        protected IRepository<ItemTypeQuestionSet> ItemTypeQuestionSetRepository { get; set; }
       
        #region Init
        public ItemManagementControllerTests()
        {
            Controller.ControllerContext.HttpContext.User = Principal;

            Items = new List<Item>();
            ItemRepository = FakeRepository<Item>();
            Controller.Repository.Expect(a => a.OfType<Item>()).Return(ItemRepository).Repeat.Any();

            Users = new List<User>();
            UserRepository = FakeRepository<User>();
            Controller.Repository.Expect(a => a.OfType<User>()).Return(UserRepository).Repeat.Any();

            Editors = new List<Editor>();
            EditorRepository = FakeRepository<Editor>();
            Controller.Repository.Expect(a => a.OfType<Editor>()).Return(EditorRepository).Repeat.Any();

            ItemTypes = new List<ItemType>();
            ItemTypeRepository = FakeRepository<ItemType>();
            Controller.Repository.Expect(a => a.OfType<ItemType>()).Return(ItemTypeRepository).Repeat.Any();

            ExtendedProperties = new List<ExtendedProperty>();
            ExtendedPropertyRepository = FakeRepository<ExtendedProperty>();
            Controller.Repository.Expect(a => a.OfType<ExtendedProperty>()).Return(ExtendedPropertyRepository).Repeat.Any();

            Tags = new List<Tag>();
            TagRepository = FakeRepository<Tag>();
            Controller.Repository.Expect(a => a.OfType<Tag>()).Return(TagRepository).Repeat.Any();

            QuestionSets = new List<QuestionSet>();
            QuestionSetRepository = FakeRepository<QuestionSet>();
            Controller.Repository.Expect(a => a.OfType<QuestionSet>()).Return(QuestionSetRepository).Repeat.Any();

            Units = new List<Unit>();
            UnitRepository = FakeRepository<Unit>();
            Controller.Repository.Expect(a => a.OfType<Unit>()).Return(UnitRepository).Repeat.Any();

            ItemTypeQuestionSets = new List<ItemTypeQuestionSet>();
            ItemTypeQuestionSetRepository = FakeRepository<ItemTypeQuestionSet>();
            Controller.Repository.Expect(a => a.OfType<ItemTypeQuestionSet>()).Return(ItemTypeQuestionSetRepository).Repeat.Any();
        
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
            Controller = new TestControllerBuilder().CreateController<ItemManagementController>();
        }

        #endregion Init

        #region Route Tests

        /// <summary>
        /// Tests the index mapping.
        /// </summary>
        [TestMethod]
        public void TestIndexMapping()
        {
            "~/ItemManagement/Index".ShouldMapTo<ItemManagementController>(a => a.Index());    
        }

        /// <summary>
        /// Tests the list mapping.
        /// </summary>
        [TestMethod]
        public void TestListMapping()
        {
            "~/ItemManagement/List".ShouldMapTo<ItemManagementController>(a => a.List());
        }

        /// <summary>
        /// Tests the create mapping.
        /// </summary>
        [TestMethod]
        public void TestCreateMapping()
        {
            "~/ItemManagement/Create".ShouldMapTo<ItemManagementController>(a => a.Create());
        }

        /// <summary>
        /// Tests the create with parameters mapping.
        /// </summary>
        [TestMethod]
        public void TestCreateWithParametersMapping()
        {
            "~/ItemManagement/Create".ShouldMapTo<ItemManagementController>(a => a.Create(new Item(),new ExtendedPropertyParameter[1],new string[1], string.Empty), true);
        }

        /// <summary>
        /// Tests the get extended properties mapping.
        /// </summary>
        [TestMethod]
        public void TestGetExtendedPropertiesMapping()
        {
            "~/ItemManagement/GetExtendedProperties/5".ShouldMapTo<ItemManagementController>(a => a.GetExtendedProperties(5));
        }

        /// <summary>
        /// Tests the edit mapping.
        /// </summary>
        [TestMethod]
        public void TestEditMapping()
        {
            "~/ItemManagement/Edit/5".ShouldMapTo<ItemManagementController>(a => a.Edit(5));
        }

        /// <summary>
        /// Tests the edit with parameters mapping.
        /// </summary>
        [TestMethod]
        public void TestEditWithParametersMapping()
        {
            "~/ItemManagement/Edit/5".ShouldMapTo<ItemManagementController>(a => a.Edit(5, new Item(),new ExtendedPropertyParameter[1],new string[1], string.Empty), true);
        }

        /// <summary>
        /// Tests the remove editor with parameters mapping.
        /// </summary>
        [TestMethod]
        public void TestRemoveEditorWithParametersMapping()
        {
            "~/ItemManagement/RemoveEditor/5".ShouldMapTo<ItemManagementController>(a => a.RemoveEditor(5, 1), true);
        }

        /// <summary>
        /// Tests the add editor mapping.
        /// </summary>
        [TestMethod]
        public void TestAddEditorMapping()
        {
            "~/ItemManagement/AddEditor/5".ShouldMapTo<ItemManagementController>(a => a.AddEditor(5, null));
        }

        /// <summary>
        /// Tests the save template with parameters mapping.
        /// </summary>
        [TestMethod]
        public void TestSaveTemplateWithParametersMapping()
        {
            "~/ItemManagement/SaveTemplate/5".ShouldMapTo<ItemManagementController>(a => a.SaveTemplate(5, "Test"), true);
        }

        /// <summary>
        /// Tests the details mapping.
        /// </summary>
        [TestMethod]
        public void TestDetailsMapping()
        {
            "~/ItemManagement/Details/5".ShouldMapTo<ItemManagementController>(a => a.Details(5));
        }
        #endregion Route Tests

        #region Index Tests

        [TestMethod]
        public void TestIndexReturnsView()
        {
            Controller.Index()
                .AssertViewRendered();
        }
        

        #endregion Index Tests

        #region List Tests

        [TestMethod]
        public void TestListReturnsView()
        {
            FakeItems(2);
            FakeUsers(2);
            FakeEditors(1);
            Users[1].LoginID = "UserName";
            Editors[0].User = Users[1];
            Items[1].AddEditor(Editors[0]);

            UserRepository.Expect(a => a.Queryable).Return(Users.AsQueryable()).Repeat.Any();
            ItemRepository.Expect(a => a.Queryable).Return(Items.AsQueryable()).Repeat.Any();

            var result = Controller.List()
                .AssertViewRendered()
                .WithViewData<IQueryable<Item>>();
            Assert.IsNotNull(result);
            Assert.AreEqual("Name2", result.FirstOrDefault().Name);
        }
        

        #endregion List Tests

        #region Create Tests

        /// <summary>
        /// Tests the create without parameter returns item view model.
        /// </summary>
        [TestMethod]
        public void TestCreateWithoutParameterReturnsItemViewModel()
        {
            FakeUsers(2);
            FakeItemTypes(1);
            Users[1].LoginID = "UserName";
            ItemTypes[0].IsActive = true;
            UserRepository.Expect(a => a.Queryable).Return(Users.AsQueryable()).Repeat.Any();
            ItemTypeRepository.Expect(a => a.Queryable).Return(ItemTypes.AsQueryable()).Repeat.Any();

            Controller.Create()
                .AssertViewRendered()
                .WithViewData<ItemViewModel>();
        }


        /// <summary>
        /// Tests the create with valid data saves.
        /// </summary>
        [TestMethod]
        public void TestCreateWithValidDataSaves()
        {
            SetupDataForCreateTests();

            var epp = new ExtendedPropertyParameter[2];
            epp[0] = new ExtendedPropertyParameter();
            epp[1] = new ExtendedPropertyParameter();
            epp[0].propertyId = 1;
            epp[0].value = "Answer1";
            epp[1].propertyId = 3;
            epp[1].value = "Answer3";

            const string mapLinkText = "<iframe width=\"425\" height=\"350\" frameborder=\"0\" scrolling=\"no\" marginheight=\"0\" marginwidth=\"0\" src=\"http://maps.google.com/maps?f=q&amp;source=s_q&amp;hl=en&amp;geocode=&amp;q=davis+ca&amp;sll=37.0625,-95.677068&amp;sspn=51.887315,79.013672&amp;ie=UTF8&amp;hq=&amp;hnear=Davis,+Yolo,+California&amp;ll=38.544906,-121.740517&amp;spn=0.00158,0.002411&amp;t=h&amp;z=19&amp;output=embed\"></iframe><br /><small><a href=\"http://maps.google.com/maps?f=q&amp;source=embed&amp;hl=en&amp;geocode=&amp;q=davis+ca&amp;sll=37.0625,-95.677068&amp;sspn=51.887315,79.013672&amp;ie=UTF8&amp;hq=&amp;hnear=Davis,+Yolo,+California&amp;ll=38.544906,-121.740517&amp;spn=0.00158,0.002411&amp;t=h&amp;z=19\" style=\"color:#0000FF;text-align:left\">View Larger Map</a></small>";

            Controller.Create(Items[0], epp, new[] {"Name1"}, mapLinkText)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List());

            ItemRepository.AssertWasCalled(a => a.EnsurePersistent(Items[0]));
            Assert.AreEqual("Item has been created successfully.", Controller.Message);
            Assert.IsTrue(Controller.ModelState.IsValid);
            Assert.AreEqual("45678", Items[0].Image.ByteArrayToString());
            Assert.AreEqual(2, Items[0].ExtendedPropertyAnswers.Count);
            Assert.AreEqual("Answer1", Items[0].ExtendedPropertyAnswers.ToList()[0].Answer);
            Assert.AreEqual("Answer3", Items[0].ExtendedPropertyAnswers.ToList()[1].Answer);
            Assert.AreEqual(1, Items[0].Tags.Count);
            Assert.AreEqual("Name1", Items[0].Tags.ToList()[0].Name);
            Assert.IsTrue(Items[0].MapLink.StartsWith("http://maps.google.com/maps?f=q&source=s_q&"));
            Assert.IsTrue(Items[0].LinkLink.StartsWith("http://maps.google.com/maps?f=q&source=embed"));
            Assert.AreEqual(1, Items[0].Editors.Count);
            Assert.AreEqual("UserName", Items[0].Editors.ToList()[0].User.LoginID);
        }


        /// <summary>
        /// Tests the create with no image saves.
        /// </summary>
        [TestMethod]
        public void TestCreateWithNoImageSaves()
        {
            SetupDataForCreateTests();
            Controller.ControllerContext.HttpContext = new MockHttpContext(0);

            var epp = new ExtendedPropertyParameter[2];
            epp[0] = new ExtendedPropertyParameter();
            epp[1] = new ExtendedPropertyParameter();
            epp[0].propertyId = 1;
            epp[0].value = "Answer1";
            epp[1].propertyId = 3;
            epp[1].value = "Answer3";

            const string mapLinkText = "<iframe width=\"425\" height=\"350\" frameborder=\"0\" scrolling=\"no\" marginheight=\"0\" marginwidth=\"0\" src=\"http://maps.google.com/maps?f=q&amp;source=s_q&amp;hl=en&amp;geocode=&amp;q=davis+ca&amp;sll=37.0625,-95.677068&amp;sspn=51.887315,79.013672&amp;ie=UTF8&amp;hq=&amp;hnear=Davis,+Yolo,+California&amp;ll=38.544906,-121.740517&amp;spn=0.00158,0.002411&amp;t=h&amp;z=19&amp;output=embed\"></iframe><br /><small><a href=\"http://maps.google.com/maps?f=q&amp;source=embed&amp;hl=en&amp;geocode=&amp;q=davis+ca&amp;sll=37.0625,-95.677068&amp;sspn=51.887315,79.013672&amp;ie=UTF8&amp;hq=&amp;hnear=Davis,+Yolo,+California&amp;ll=38.544906,-121.740517&amp;spn=0.00158,0.002411&amp;t=h&amp;z=19\" style=\"color:#0000FF;text-align:left\">View Larger Map</a></small>";

            Controller.Create(Items[0], epp, new[] { "Name1" }, mapLinkText)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List());

            ItemRepository.AssertWasCalled(a => a.EnsurePersistent(Items[0]));

            Assert.IsNull(Items[0].Image);
        }

        /// <summary>
        /// Tests the create with no extended properties saves.
        /// </summary>
        [TestMethod]
        public void TestCreateWithNoExtendedPropertiesSaves()
        {
            SetupDataForCreateTests();

            const string mapLinkText = "<iframe width=\"425\" height=\"350\" frameborder=\"0\" scrolling=\"no\" marginheight=\"0\" marginwidth=\"0\" src=\"http://maps.google.com/maps?f=q&amp;source=s_q&amp;hl=en&amp;geocode=&amp;q=davis+ca&amp;sll=37.0625,-95.677068&amp;sspn=51.887315,79.013672&amp;ie=UTF8&amp;hq=&amp;hnear=Davis,+Yolo,+California&amp;ll=38.544906,-121.740517&amp;spn=0.00158,0.002411&amp;t=h&amp;z=19&amp;output=embed\"></iframe><br /><small><a href=\"http://maps.google.com/maps?f=q&amp;source=embed&amp;hl=en&amp;geocode=&amp;q=davis+ca&amp;sll=37.0625,-95.677068&amp;sspn=51.887315,79.013672&amp;ie=UTF8&amp;hq=&amp;hnear=Davis,+Yolo,+California&amp;ll=38.544906,-121.740517&amp;spn=0.00158,0.002411&amp;t=h&amp;z=19\" style=\"color:#0000FF;text-align:left\">View Larger Map</a></small>";

            Controller.Create(Items[0], null, new[] { "Name1" }, mapLinkText)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List());

            ItemRepository.AssertWasCalled(a => a.EnsurePersistent(Items[0]));

            Assert.AreEqual(0, Items[0].ExtendedPropertyAnswers.Count);
        }

        /// <summary>
        /// Tests the create only adds new extended properties.
        /// </summary>
        [TestMethod]
        public void TestCreateOnlyAddsNewExtendedProperties()
        {
            SetupDataForCreateTests();

            var epp = new ExtendedPropertyParameter[2];
            epp[0] = new ExtendedPropertyParameter();
            epp[1] = new ExtendedPropertyParameter();
            epp[0].propertyId = 1;
            epp[0].value = "Answer1";
            epp[1].propertyId = 3;
            epp[1].value = "Answer2";

            var epa = new ExtendedPropertyAnswer(epp[1].value, Items[0], ExtendedProperties[2]);
            Items[0].AddExtendedPropertyAnswer(epa);

            epp[1].value = "New Question";
            Assert.AreNotEqual("New Question", Items[0].ExtendedPropertyAnswers.ToList()[0].Answer);

            const string mapLinkText = "<iframe width=\"425\" height=\"350\" frameborder=\"0\" scrolling=\"no\" marginheight=\"0\" marginwidth=\"0\" src=\"http://maps.google.com/maps?f=q&amp;source=s_q&amp;hl=en&amp;geocode=&amp;q=davis+ca&amp;sll=37.0625,-95.677068&amp;sspn=51.887315,79.013672&amp;ie=UTF8&amp;hq=&amp;hnear=Davis,+Yolo,+California&amp;ll=38.544906,-121.740517&amp;spn=0.00158,0.002411&amp;t=h&amp;z=19&amp;output=embed\"></iframe><br /><small><a href=\"http://maps.google.com/maps?f=q&amp;source=embed&amp;hl=en&amp;geocode=&amp;q=davis+ca&amp;sll=37.0625,-95.677068&amp;sspn=51.887315,79.013672&amp;ie=UTF8&amp;hq=&amp;hnear=Davis,+Yolo,+California&amp;ll=38.544906,-121.740517&amp;spn=0.00158,0.002411&amp;t=h&amp;z=19\" style=\"color:#0000FF;text-align:left\">View Larger Map</a></small>";

            Controller.Create(Items[0], epp, new[] { "Name1" }, mapLinkText)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List());

            ItemRepository.AssertWasCalled(a => a.EnsurePersistent(Items[0]));

            Assert.AreEqual(2, Items[0].ExtendedPropertyAnswers.Count);
            Assert.AreEqual("New Question", Items[0].ExtendedPropertyAnswers.ToList()[0].Answer);
            Assert.AreEqual("Answer1", Items[0].ExtendedPropertyAnswers.ToList()[1].Answer);

        }

        /// <summary>
        /// Tests the create adds new extended properties when existing ones are different.
        /// </summary>
        [TestMethod]
        public void TestCreateAddsNewExtendedPropertiesWhenExistingOnesAreDifferent()
        {
            SetupDataForCreateTests();

            var epp = new ExtendedPropertyParameter[2];
            epp[0] = new ExtendedPropertyParameter();
            epp[1] = new ExtendedPropertyParameter();
            epp[0].propertyId = 1;
            epp[0].value = "Answer1";
            epp[1].propertyId = 3;
            epp[1].value = "Answer2";

            var existingExtendedPropertyParameter = new ExtendedPropertyParameter();
            existingExtendedPropertyParameter.propertyId = 2;
            existingExtendedPropertyParameter.value = "Existing";

            var epa = new ExtendedPropertyAnswer(existingExtendedPropertyParameter.value, Items[0], ExtendedProperties[1]);
            Items[0].AddExtendedPropertyAnswer(epa);
            Assert.AreEqual("Existing", Items[0].ExtendedPropertyAnswers.ToList()[0].Answer);

            const string mapLinkText = "<iframe width=\"425\" height=\"350\" frameborder=\"0\" scrolling=\"no\" marginheight=\"0\" marginwidth=\"0\" src=\"http://maps.google.com/maps?f=q&amp;source=s_q&amp;hl=en&amp;geocode=&amp;q=davis+ca&amp;sll=37.0625,-95.677068&amp;sspn=51.887315,79.013672&amp;ie=UTF8&amp;hq=&amp;hnear=Davis,+Yolo,+California&amp;ll=38.544906,-121.740517&amp;spn=0.00158,0.002411&amp;t=h&amp;z=19&amp;output=embed\"></iframe><br /><small><a href=\"http://maps.google.com/maps?f=q&amp;source=embed&amp;hl=en&amp;geocode=&amp;q=davis+ca&amp;sll=37.0625,-95.677068&amp;sspn=51.887315,79.013672&amp;ie=UTF8&amp;hq=&amp;hnear=Davis,+Yolo,+California&amp;ll=38.544906,-121.740517&amp;spn=0.00158,0.002411&amp;t=h&amp;z=19\" style=\"color:#0000FF;text-align:left\">View Larger Map</a></small>";

            Controller.Create(Items[0], epp, new[] { "Name1" }, mapLinkText)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List());

            ItemRepository.AssertWasCalled(a => a.EnsurePersistent(Items[0]));

            Assert.AreEqual(3, Items[0].ExtendedPropertyAnswers.Count);
            Assert.AreEqual("Existing", Items[0].ExtendedPropertyAnswers.ToList()[0].Answer);
            Assert.AreEqual("Answer1", Items[0].ExtendedPropertyAnswers.ToList()[1].Answer);
            Assert.AreEqual("Answer2", Items[0].ExtendedPropertyAnswers.ToList()[2].Answer);
        }

        /// <summary>
        /// Tests the create removes all existing tags when empty array of strings is passed.
        /// </summary>
        [TestMethod]
        public void TestCreateRemovesAllExistingTagsWhenEmptyArrayOfStringsIsPassed()
        {
            SetupDataForCreateTests();

            var epp = new ExtendedPropertyParameter[2];
            epp[0] = new ExtendedPropertyParameter();
            epp[1] = new ExtendedPropertyParameter();
            epp[0].propertyId = 1;
            epp[0].value = "Answer1";
            epp[1].propertyId = 3;
            epp[1].value = "Answer3";

            const string mapLinkText = "<iframe width=\"425\" height=\"350\" frameborder=\"0\" scrolling=\"no\" marginheight=\"0\" marginwidth=\"0\" src=\"http://maps.google.com/maps?f=q&amp;source=s_q&amp;hl=en&amp;geocode=&amp;q=davis+ca&amp;sll=37.0625,-95.677068&amp;sspn=51.887315,79.013672&amp;ie=UTF8&amp;hq=&amp;hnear=Davis,+Yolo,+California&amp;ll=38.544906,-121.740517&amp;spn=0.00158,0.002411&amp;t=h&amp;z=19&amp;output=embed\"></iframe><br /><small><a href=\"http://maps.google.com/maps?f=q&amp;source=embed&amp;hl=en&amp;geocode=&amp;q=davis+ca&amp;sll=37.0625,-95.677068&amp;sspn=51.887315,79.013672&amp;ie=UTF8&amp;hq=&amp;hnear=Davis,+Yolo,+California&amp;ll=38.544906,-121.740517&amp;spn=0.00158,0.002411&amp;t=h&amp;z=19\" style=\"color:#0000FF;text-align:left\">View Larger Map</a></small>";

            Items[0].AddTag(Tags[0]);
            Items[0].AddTag(Tags[1]);

            Assert.AreEqual(2, Items[0].Tags.Count);

            Controller.Create(Items[0], epp, new[] { "" }, mapLinkText)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List());

            ItemRepository.AssertWasCalled(a => a.EnsurePersistent(Items[0]));

            Assert.AreEqual(0, Items[0].Tags.Count);
         
        }

        /// <summary>
        /// Tests the create with existing and new tag only adds new tag.
        /// </summary>
        [TestMethod]
        public void TestCreateWithExistingAndNewTagOnlyAddsNewTag()
        {
            SetupDataForCreateTests();

            var epp = new ExtendedPropertyParameter[2];
            epp[0] = new ExtendedPropertyParameter();
            epp[1] = new ExtendedPropertyParameter();
            epp[0].propertyId = 1;
            epp[0].value = "Answer1";
            epp[1].propertyId = 3;
            epp[1].value = "Answer3";

            const string mapLinkText = "<iframe width=\"425\" height=\"350\" frameborder=\"0\" scrolling=\"no\" marginheight=\"0\" marginwidth=\"0\" src=\"http://maps.google.com/maps?f=q&amp;source=s_q&amp;hl=en&amp;geocode=&amp;q=davis+ca&amp;sll=37.0625,-95.677068&amp;sspn=51.887315,79.013672&amp;ie=UTF8&amp;hq=&amp;hnear=Davis,+Yolo,+California&amp;ll=38.544906,-121.740517&amp;spn=0.00158,0.002411&amp;t=h&amp;z=19&amp;output=embed\"></iframe><br /><small><a href=\"http://maps.google.com/maps?f=q&amp;source=embed&amp;hl=en&amp;geocode=&amp;q=davis+ca&amp;sll=37.0625,-95.677068&amp;sspn=51.887315,79.013672&amp;ie=UTF8&amp;hq=&amp;hnear=Davis,+Yolo,+California&amp;ll=38.544906,-121.740517&amp;spn=0.00158,0.002411&amp;t=h&amp;z=19\" style=\"color:#0000FF;text-align:left\">View Larger Map</a></small>";

            //Items[0].AddTag(Tags[0]);
            Items[0].AddTag(Tags[1]);

            Assert.AreEqual(1, Items[0].Tags.Count);

            Controller.Create(Items[0], epp, new[] { Tags[0].Name, Tags[1].Name }, mapLinkText)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List());

            ItemRepository.AssertWasCalled(a => a.EnsurePersistent(Items[0]));

            Assert.AreEqual(2, Items[0].Tags.Count);
            Assert.AreEqual(Tags[1].Name, Items[0].Tags.ToList()[0].Name);
            Assert.AreEqual(Tags[0].Name, Items[0].Tags.ToList()[1].Name);
        }

        /// <summary>
        /// Tests the create with existing tags removes tag not passed.
        /// </summary>
        [TestMethod]
        public void TestCreateWithExistingTagsRemovesTagNotPassed()
        {
            SetupDataForCreateTests();

            var epp = new ExtendedPropertyParameter[2];
            epp[0] = new ExtendedPropertyParameter();
            epp[1] = new ExtendedPropertyParameter();
            epp[0].propertyId = 1;
            epp[0].value = "Answer1";
            epp[1].propertyId = 3;
            epp[1].value = "Answer3";

            const string mapLinkText = "<iframe width=\"425\" height=\"350\" frameborder=\"0\" scrolling=\"no\" marginheight=\"0\" marginwidth=\"0\" src=\"http://maps.google.com/maps?f=q&amp;source=s_q&amp;hl=en&amp;geocode=&amp;q=davis+ca&amp;sll=37.0625,-95.677068&amp;sspn=51.887315,79.013672&amp;ie=UTF8&amp;hq=&amp;hnear=Davis,+Yolo,+California&amp;ll=38.544906,-121.740517&amp;spn=0.00158,0.002411&amp;t=h&amp;z=19&amp;output=embed\"></iframe><br /><small><a href=\"http://maps.google.com/maps?f=q&amp;source=embed&amp;hl=en&amp;geocode=&amp;q=davis+ca&amp;sll=37.0625,-95.677068&amp;sspn=51.887315,79.013672&amp;ie=UTF8&amp;hq=&amp;hnear=Davis,+Yolo,+California&amp;ll=38.544906,-121.740517&amp;spn=0.00158,0.002411&amp;t=h&amp;z=19\" style=\"color:#0000FF;text-align:left\">View Larger Map</a></small>";

            Items[0].AddTag(Tags[0]);
            Items[0].AddTag(Tags[1]);

            Assert.AreEqual(2, Items[0].Tags.Count);

            Controller.Create(Items[0], epp, new[] { Tags[1].Name }, mapLinkText)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List());

            ItemRepository.AssertWasCalled(a => a.EnsurePersistent(Items[0]));

            Assert.AreEqual(1, Items[0].Tags.Count);
            Assert.AreEqual(Tags[1].Name, Items[0].Tags.ToList()[0].Name);  
        }

        /// <summary>
        /// Tests the create with tag that does not exist creates new tag.
        /// </summary>
        [TestMethod]
        public void TestCreateWithTagThatDoesNotExistCreatesNewTag()
        {
            SetupDataForCreateTests();

            var epp = new ExtendedPropertyParameter[2];
            epp[0] = new ExtendedPropertyParameter();
            epp[1] = new ExtendedPropertyParameter();
            epp[0].propertyId = 1;
            epp[0].value = "Answer1";
            epp[1].propertyId = 3;
            epp[1].value = "Answer3";

            const string mapLinkText = "<iframe width=\"425\" height=\"350\" frameborder=\"0\" scrolling=\"no\" marginheight=\"0\" marginwidth=\"0\" src=\"http://maps.google.com/maps?f=q&amp;source=s_q&amp;hl=en&amp;geocode=&amp;q=davis+ca&amp;sll=37.0625,-95.677068&amp;sspn=51.887315,79.013672&amp;ie=UTF8&amp;hq=&amp;hnear=Davis,+Yolo,+California&amp;ll=38.544906,-121.740517&amp;spn=0.00158,0.002411&amp;t=h&amp;z=19&amp;output=embed\"></iframe><br /><small><a href=\"http://maps.google.com/maps?f=q&amp;source=embed&amp;hl=en&amp;geocode=&amp;q=davis+ca&amp;sll=37.0625,-95.677068&amp;sspn=51.887315,79.013672&amp;ie=UTF8&amp;hq=&amp;hnear=Davis,+Yolo,+California&amp;ll=38.544906,-121.740517&amp;spn=0.00158,0.002411&amp;t=h&amp;z=19\" style=\"color:#0000FF;text-align:left\">View Larger Map</a></small>";

            Controller.Create(Items[0], epp, new[] { "NewTag" }, mapLinkText)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List());

            ItemRepository.AssertWasCalled(a => a.EnsurePersistent(Items[0]));

            Assert.AreEqual(1, Items[0].Tags.Count);
            Assert.AreEqual("NewTag", Items[0].Tags.ToList()[0].Name);    
        }

        /// <summary>
        /// Tests the create with invalid data does not save.
        /// </summary>
        [TestMethod]
        public void TestCreateWithInvalidDataDoesNotSave()
        {
            SetupDataForCreateTests();
            ItemTypeRepository.Expect(a => a.Queryable).Return(ItemTypes.AsQueryable()).Repeat.Any();

            var epp = new ExtendedPropertyParameter[2];
            epp[0] = new ExtendedPropertyParameter();
            epp[1] = new ExtendedPropertyParameter();
            epp[0].propertyId = 1;
            epp[0].value = "Answer1";
            epp[1].propertyId = 3;
            epp[1].value = "Answer3";

            const string mapLinkText = "<iframe width=\"425\" height=\"350\" frameborder=\"0\" scrolling=\"no\" marginheight=\"0\" marginwidth=\"0\" src=\"http://maps.google.com/maps?f=q&amp;source=s_q&amp;hl=en&amp;geocode=&amp;q=davis+ca&amp;sll=37.0625,-95.677068&amp;sspn=51.887315,79.013672&amp;ie=UTF8&amp;hq=&amp;hnear=Davis,+Yolo,+California&amp;ll=38.544906,-121.740517&amp;spn=0.00158,0.002411&amp;t=h&amp;z=19&amp;output=embed\"></iframe><br /><small><a href=\"http://maps.google.com/maps?f=q&amp;source=embed&amp;hl=en&amp;geocode=&amp;q=davis+ca&amp;sll=37.0625,-95.677068&amp;sspn=51.887315,79.013672&amp;ie=UTF8&amp;hq=&amp;hnear=Davis,+Yolo,+California&amp;ll=38.544906,-121.740517&amp;spn=0.00158,0.002411&amp;t=h&amp;z=19\" style=\"color:#0000FF;text-align:left\">View Larger Map</a></small>";

            Items[0].Name = " "; //Invalid

            Controller.Create(Items[0], epp, new[] {"Name1"}, mapLinkText)
                .AssertViewRendered()
                .WithViewData<ItemViewModel>();

            ItemRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Item>.Is.Anything));
            Assert.AreNotEqual("Item has been created successfully.", Controller.Message);
            Assert.IsFalse(Controller.ModelState.IsValid);
            Controller.ModelState.AssertErrorsAre("Name: may not be null or empty");
        }

        #endregion Create Tests

        #region GetExtendedProperties Tests

        /// <summary>
        /// Tests the get extended properties return json false when id not found.
        /// </summary>
        [TestMethod]
        public void TestGetExtendedPropertiesReturnJsonFalseWhenIdNotFound()
        {
            ItemTypeRepository.Expect(a => a.GetNullableByID(2)).Return(null).Repeat.Any();
            var result = Controller.GetExtendedProperties(2)
                .AssertResultIs<JsonNetResult>();
            Assert.AreEqual(false, result.Data);
        }

        /// <summary>
        /// Tests the get extended properties returns json net result with extended properties enumeration for item.
        /// </summary>
        [TestMethod]
        public void TestGetExtendedPropertiesReturnsJsonNetResultWithExtendedPropertiesEnumerationForItem()
        {
            FakeItemTypes(2);
            FakeExtendedProperties(3);

            ItemTypes[1].AddExtendedProperty(ExtendedProperties[0]);
            ItemTypes[1].AddExtendedProperty(ExtendedProperties[2]);

            ItemTypeRepository.Expect(a => a.GetNullableByID(2)).Return(ItemTypes[1]).Repeat.Any();

            var result = Controller.GetExtendedProperties(2)
                .AssertResultIs<JsonNetResult>();
            Assert.AreNotEqual(false, result.Data);
            var extendedProperties = (List<ExtendedProperty>)result.Data;
            Assert.AreEqual(2, extendedProperties.Count);
            Assert.AreEqual(ExtendedProperties[0].Name, extendedProperties[0].Name);
            Assert.AreEqual(ExtendedProperties[2].Name, extendedProperties[1].Name);


        }



        #endregion GetExtendedProperties Tests

        #region Edit Tests

        /// <summary>
        /// Tests the edit with one parameter redirects to list when id not found.
        /// </summary>
        [TestMethod]
        public void TestEditWithOneParameterRedirectsToListWhenIdNotFound()
        {
            ItemRepository.Expect(a => a.GetNullableByID(2)).Return(null).Repeat.Any();
            Controller.Edit(2)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List());
        }

        /// <summary>
        /// Tests the edit with one parameter return view when id found.
        /// </summary>
        [TestMethod]
        public void TestEditWithOneParameterReturnViewWhenIdFound()
        {
            FakeItems(3);
            FakeItemTypes(2);
            FakeUsers(3);
            Users[1].LoginID = "UserName";

            ItemRepository.Expect(a => a.GetNullableByID(2)).Return(Items[1]).Repeat.Any();
            ItemTypeRepository.Expect(a => a.Queryable).Return(ItemTypes.AsQueryable()).Repeat.Any();
            UserRepository.Expect(a => a.Queryable).Return(Users.AsQueryable()).Repeat.Any();

            var result = Controller.Edit(2)
                .AssertViewRendered()
                .WithViewData<ItemViewModel>();
            Assert.IsNotNull(result);
            Assert.AreEqual(Users[1], result.CurrentUser);
            Assert.AreEqual(3, result.Users.Count());
            Assert.AreEqual(2, result.ItemTypes.Count());
            Assert.AreEqual(Items[1], result.Item);
        }

        /// <summary>
        /// Tests the edit when id not found does not save.
        /// </summary>
        [TestMethod]
        public void TestEditWhenIdNotFoundDoesNotSave()
        {             
            ItemRepository.Expect(a => a.GetNullableByID(2)).Return(null).Repeat.Any();

            var epp = new ExtendedPropertyParameter[2];
            epp[0] = new ExtendedPropertyParameter();
            epp[1] = new ExtendedPropertyParameter();
            epp[0].propertyId = 1;
            epp[0].value = "Answer1";
            epp[1].propertyId = 3;
            epp[1].value = "Answer3";

            Controller.Edit(2, new Item(), epp, new [] { "Test" }, null)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List());
            Assert.AreEqual("You do not have editor rights to that item.", Controller.Message);
            ItemRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Item>.Is.Anything));
        }

        /// <summary>
        /// Tests the edit redirects to list when user does not have editor rights.
        /// </summary>
        [TestMethod]
        public void TestEditRedirectsToListWhenUserDoesNotHaveEditorRights()
        {
            Assert.AreEqual("UserName", Controller.CurrentUser.Identity.Name);
            FakeUsers(3);
            foreach (var user in Users)
            {
                Assert.AreNotEqual("UserName", user.LoginID);
            }
            FakeItems(1);
            Items[0].AddEditor(new Editor(Items[0], Users[0]));
            Items[0].AddEditor(new Editor(Items[0], Users[1]));
            Items[0].AddEditor(new Editor(Items[0], Users[2]));

            ItemRepository.Expect(a => a.GetNullableByID(1)).Return(Items[0]).Repeat.Any();

            var epp = new ExtendedPropertyParameter[2];
            epp[0] = new ExtendedPropertyParameter();
            epp[1] = new ExtendedPropertyParameter();
            epp[0].propertyId = 1;
            epp[0].value = "Answer1";
            epp[1].propertyId = 3;
            epp[1].value = "Answer3";

            Controller.Edit(1, Items[0], epp, new []{"NewTag"},null  )
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List());
            Assert.AreEqual("You do not have editor rights to that item.", Controller.Message);
            ItemRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Item>.Is.Anything));
        }

        /// <summary>
        /// Tests the edit where user has editor rights saves.
        /// </summary>
        [TestMethod]
        public void TestEditWhereUserHasEditorRightsSaves()
        {
            //Mock one file
            Controller.ControllerContext.HttpContext = new MockHttpContext(1);

            Assert.AreEqual("UserName", Controller.CurrentUser.Identity.Name);
            FakeUsers(3);
            Users[1].LoginID = Controller.CurrentUser.Identity.Name;
            FakeItems(1);
            Items[0].AddEditor(new Editor(Items[0], Users[0]));
            Items[0].AddEditor(new Editor(Items[0], Users[1]));
            Items[0].AddEditor(new Editor(Items[0], Users[2]));

            FakeItemTypes(1);
            FakeTags(2);
            TagRepository.Expect(a => a.GetAll()).Return(Tags).Repeat.Any();

            ItemTypeRepository.Expect(a => a.Queryable).Return(ItemTypes.AsQueryable()).Repeat.Any();
            UserRepository.Expect(a => a.Queryable).Return(Users.AsQueryable()).Repeat.Any();
            ItemRepository.Expect(a => a.GetNullableByID(1)).Return(Items[0]).Repeat.Any();

            var epp = new ExtendedPropertyParameter[2];
            epp[0] = new ExtendedPropertyParameter();
            epp[1] = new ExtendedPropertyParameter();
            epp[0].propertyId = 1;
            epp[0].value = "Answer1";
            epp[1].propertyId = 3;
            epp[1].value = "Answer3";

            Controller.Edit(1, Items[0], epp, new[] {Tags[0].Name}, null)
                .AssertViewRendered()
                .WithViewData<ItemViewModel>();
            Assert.AreEqual("Item has been saved successfully.", Controller.Message);
            ItemRepository.AssertWasCalled(a => a.EnsurePersistent(Items[0]));
        }



        #endregion Edit Tests
        
        #region Helper Methods

        /// <summary>
        /// Setups the data for create tests.
        /// </summary>
        private void SetupDataForCreateTests()
        {
            //Mock one file
            Controller.ControllerContext.HttpContext = new MockHttpContext(1);
            //Fakes
            FakeExtendedProperties(4);
            FakeTags(2);
            FakeItems(1);
            FakeUsers(2);
            FakeQuestionSets(1);
            FakeUnits(2);
            FakeItemTypes(3);
            FakeItemTypeQuestionSets(4);

            TagRepository.Expect(a => a.GetAll()).Return(Tags.ToList()).Repeat.Any();
            UserRepository.Expect(a => a.Queryable).Return(Users.AsQueryable()).Repeat.Any();
            QuestionSetRepository.Expect(a => a.Queryable).Return(QuestionSets.AsQueryable()).Repeat.Any();

            QuestionSets[0].Name = StaticValues.QuestionSet_ContactInformation;

            Users[1].LoginID = "UserName";
            Users[1].Units.Add(Units[0]);
            Users[1].Units.Add(Units[1]);

            ItemTypeQuestionSets[0].TransactionLevel = true;
            ItemTypeQuestionSets[0].QuantityLevel = false;
            ItemTypeQuestionSets[1].TransactionLevel = false;
            ItemTypeQuestionSets[1].QuantityLevel = true;

            ItemTypes[0].QuestionSets.Add(ItemTypeQuestionSets[0]);
            ItemTypes[0].QuestionSets.Add(ItemTypeQuestionSets[1]);
            ItemTypes[2].QuestionSets.Add(ItemTypeQuestionSets[3]);

            Items[0].ItemType = ItemTypes[0];
        }


        #region Fakes

        /// <summary>
        /// Fakes the items.
        /// </summary>
        /// <param name="count">The count.</param>
        private void FakeItems(int count)
        {
            var offSet = Items.Count;
            for (int i = 0; i < count; i++)
            {
                Items.Add(CreateValidEntities.Item(i + 1 + offSet));
                Items[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }

        private void FakeUsers(int count)
        {
            var offSet = Users.Count;
            for (int i = 0; i < count; i++)
            {
                Users.Add(CreateValidEntities.User(i + 1 + offSet));
                Users[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }

        private void FakeEditors(int count)
        {
            var offSet = Editors.Count;
            for (int i = 0; i < count; i++)
            {
                Editors.Add(CreateValidEntities.Editor(i + 1 + offSet));
                Editors[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }

        /// <summary>
        /// Fakes the item types.
        /// </summary>
        /// <param name="count">The count.</param>
        private void FakeItemTypes(int count)
        {
            var offSet = ItemTypes.Count;
            for (int i = 0; i < count; i++)
            {
                ItemTypes.Add(CreateValidEntities.ItemType(i + 1 + offSet));
                ItemTypes[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }

        /// <summary>
        /// Fakes the ExtendedProperties.
        /// Author: Sylvestre, Jason
        /// Create: 2010/02/01
        /// </summary>
        /// <param name="count">The number of ExtendedProperties to add.</param>
        private void FakeExtendedProperties(int count)
        {
            var offSet = ExtendedProperties.Count;
            for (int i = 0; i < count; i++)
            {
                ExtendedProperties.Add(CreateValidEntities.ExtendedProperty(i + 1 + offSet));
                ExtendedProperties[i + offSet].SetIdTo(i + 1 + offSet);
                var i1 = i;
                ExtendedPropertyRepository.Expect(a => a.GetNullableByID(i1 + 1 + offSet))
                    .Return(ExtendedProperties[i1 + offSet]).Repeat.Any();
            }
        }

        /// <summary>
        /// Fakes the Tags.
        /// Author: Sylvestre, Jason
        /// Create: 2010/02/01
        /// </summary>
        /// <param name="count">The number of Tags to add.</param>
        private void FakeTags(int count)
        {
            var offSet = Tags.Count;
            for (int i = 0; i < count; i++)
            {
                Tags.Add(CreateValidEntities.Tag(i + 1 + offSet));
                Tags[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }
        /// <summary>
        /// Fakes the QuestionSets.
        /// Author: Sylvestre, Jason
        /// Create: 2010/02/01
        /// </summary>
        /// <param name="count">The number of QuestionSets to add.</param>
        private void FakeQuestionSets(int count)
        {
            var offSet = QuestionSets.Count;
            for (int i = 0; i < count; i++)
            {
            QuestionSets.Add(CreateValidEntities.QuestionSet(i + 1 + offSet));
            QuestionSets[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }
        /// <summary>
        /// Fakes the Units.
        /// Author: Sylvestre, Jason
        /// Create: 2010/02/01
        /// </summary>
        /// <param name="count">The number of Units to add.</param>
        private void FakeUnits(int count)
        {
            var offSet = Units.Count;
            for (int i = 0; i < count; i++)
            {
                Units.Add(CreateValidEntities.Unit(i + 1 + offSet));
                Units[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }
        /// <summary>
        /// Fakes the ItemTypeQuestionSets.
        /// Author: Sylvestre, Jason
        /// Create: 2010/02/01
        /// </summary>
        /// <param name="count">The number of ItemTypeQuestionSets to add.</param>
        private void FakeItemTypeQuestionSets(int count)
        {
            var offSet = ItemTypeQuestionSets.Count;
            for (int i = 0; i < count; i++)
            {
            ItemTypeQuestionSets.Add(CreateValidEntities.ItemTypeQuestionSet(i + 1 + offSet));
            ItemTypeQuestionSets[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }

        #endregion Fakes

        #region mocks
        /// <summary>
        /// Mock the Identity. Used for getting the current user name
        /// </summary>
        public class MockIdentity : IIdentity
        {
            public string AuthenticationType
            {
                get
                {
                    return "MockAuthentication";
                }
            }

            public bool IsAuthenticated
            {
                get
                {
                    return true;
                }
            }

            public string Name
            {
                get
                {
                    return "UserName";
                }
            }
        }


        /// <summary>
        /// Mock the Principal. Used for getting the current user name
        /// </summary>
        public class MockPrincipal : IPrincipal
        {
            IIdentity _identity;

            public IIdentity Identity
            {
                get
                {
                    if (_identity == null)
                    {
                        _identity = new MockIdentity();
                    }
                    return _identity;
                }
            }

            public bool IsInRole(string role)
            {
                return false;
            }
        }

        /// <summary>
        /// Mock the HttpContext. Used for getting the current user name
        /// </summary>
        public class MockHttpContext : HttpContextBase
        {
            private IPrincipal _user;
            private int _count;
            public MockHttpContext(int count)
            {
                _count = count;
            }

            public override IPrincipal User
            {
                get
                {
                    if (_user == null)
                    {
                        _user = new MockPrincipal();
                    }
                    return _user;
                }
                set
                {
                    _user = value;
                }
            }

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
            MockHttpFileCollectionBase Mocked { get; set; }

            public MockHttpRequest(int count)
            {
                Mocked = new MockHttpFileCollectionBase(count);
            }
            public override HttpFileCollectionBase Files
            {
                get
                {
                    return Mocked;
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
                    BaseAdd("Test" + (i + 1), new byte[]{4,5,6,7,8});
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
            public override Stream InputStream
            {
                get
                {
                    var memStream = new MemoryStream(new byte[] { 4, 5, 6, 7, 8 });
                    return memStream;
                }
            }
        }

        #endregion

        #endregion Helper Methods
    }
}
