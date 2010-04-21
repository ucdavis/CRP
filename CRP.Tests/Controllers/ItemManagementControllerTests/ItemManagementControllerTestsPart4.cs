using System.Linq;
using CRP.Controllers;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using CRP.Tests.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;

namespace CRP.Tests.Controllers.ItemManagementControllerTests
{
    public partial class ItemManagementControllerTests
    {
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

            Controller.Create(Items[0], epp, new[] { "Name1" }, mapLinkText)
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

            Controller.Create(Items[0], epp, new[] { "Name1" }, mapLinkText)
                .AssertViewRendered()
                .WithViewData<ItemViewModel>();

            ItemRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Item>.Is.Anything));
            Assert.AreNotEqual("Item has been created successfully.", Controller.Message);
            Assert.IsFalse(Controller.ModelState.IsValid);
            Controller.ModelState.AssertErrorsAre("Name: may not be null or empty");
        }

        #endregion Create Tests
    }
}
