using System;
using System.Linq;
using CRP.Controllers;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using CRP.Tests.Core.Extensions;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;

namespace CRP.Tests.Controllers.ItemManagementControllerTests
{
    public partial class ItemManagementControllerTests
    {
        

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
            FakeEditors(1);
            Editors[0].User = Users[1];
            Items[1].AddEditor(Editors[0]);


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
        [TestMethod, Ignore] //Need to re write test because a null Item now throws a precondition exception
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

            Controller.Edit(2, new Item(), epp, new[] { "Test" }, null)
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

            Controller.Edit(1, Items[0], epp, new[] { "NewTag" }, null)
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

            Controller.Edit(1, Items[0], epp, new[] { Tags[0].Name }, null)
                .AssertViewRendered()
                .WithViewData<ItemViewModel>();
            Assert.AreEqual("Item has been saved successfully.", Controller.Message);
            ItemRepository.AssertWasCalled(a => a.EnsurePersistent(Items[0]));
        }

        /// <summary>
        /// Tests the edit with invalid data does not save.
        /// </summary>
        [TestMethod]
        public void TestEditWithInvalidDataDoesNotSave()
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

            Items[0].Name = " "; //Invalid

            Controller.Edit(1, Items[0], epp, new[] { Tags[0].Name }, null)
                .AssertViewRendered()
                .WithViewData<ItemViewModel>();
            Assert.AreNotEqual("Item has been saved successfully.", Controller.Message);
            ItemRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Item>.Is.Anything));
            Controller.ModelState.AssertErrorsAre("Name: may not be null or empty");
        }

        /// <summary>
        /// Tests the edit updates expected fields and saves.
        /// </summary>
        [TestMethod]
        public void TestEditUpdatesExpectedFieldsAndSaves()
        {
            const string mapLinkText = "<iframe width=\"425\" height=\"350\" frameborder=\"0\" scrolling=\"no\" marginheight=\"0\" marginwidth=\"0\" src=\"http://maps.google.com/maps?f=q&amp;source=s_q&amp;hl=en&amp;geocode=&amp;q=davis+ca&amp;sll=37.0625,-95.677068&amp;sspn=51.887315,79.013672&amp;ie=UTF8&amp;hq=&amp;hnear=Davis,+Yolo,+California&amp;ll=38.544906,-121.740517&amp;spn=0.00158,0.002411&amp;t=h&amp;z=19&amp;output=embed\"></iframe><br /><small><a href=\"http://maps.google.com/maps?f=q&amp;source=embed&amp;hl=en&amp;geocode=&amp;q=davis+ca&amp;sll=37.0625,-95.677068&amp;sspn=51.887315,79.013672&amp;ie=UTF8&amp;hq=&amp;hnear=Davis,+Yolo,+California&amp;ll=38.544906,-121.740517&amp;spn=0.00158,0.002411&amp;t=h&amp;z=19\" style=\"color:#0000FF;text-align:left\">View Larger Map</a></small>";

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
            FakeExtendedProperties(4);

            var itemToUpdate = CreateValidEntities.Item(99);
            itemToUpdate.Description = "DescriptionUpdate";
            itemToUpdate.CostPerItem = 10.77m;
            itemToUpdate.Quantity = 99;
            itemToUpdate.Expiration = new DateTime(2010, 02, 02);
            itemToUpdate.Link = "LinkUpdate";
            itemToUpdate.Available = !Items[0].Available;
            itemToUpdate.Private = !Items[0].Private;
            itemToUpdate.Image = new byte[] { 1, 2, 3 };
            Items[0].Image = new byte[] { 4, 5, 6 };
            itemToUpdate.AddExtendedPropertyAnswer(new ExtendedPropertyAnswer(epp[0].value, itemToUpdate, ExtendedProperties[1]));
            //itemToUpdate.AddExtendedPropertyAnswer(new ExtendedPropertyAnswer(epp[1].value, itemToUpdate, ExtendedProperties[2]));
            itemToUpdate.AddTag(Tags[0]);
            itemToUpdate.AddTag(Tags[1]);
            itemToUpdate.LinkLink = "ThisWillBeChanged";
            itemToUpdate.MapLink = "ThisWillBeChanged";

            Assert.AreNotEqual(Items[0].Name, itemToUpdate.Name);
            Assert.AreNotEqual(Items[0].Description, itemToUpdate.Description);
            Assert.AreNotEqual(Items[0].CostPerItem, itemToUpdate.CostPerItem);
            Assert.AreNotEqual(Items[0].Quantity, itemToUpdate.Quantity);
            Assert.AreNotEqual(Items[0].Expiration, itemToUpdate.Expiration);
            Assert.AreNotEqual(Items[0].Link, itemToUpdate.Link);
            Assert.AreNotEqual(Items[0].Available, itemToUpdate.Available);
            Assert.AreNotEqual(Items[0].Private, itemToUpdate.Private);
            Assert.AreNotEqual(Items[0].Image.ByteArrayToString(), itemToUpdate.Image.ByteArrayToString());
            Assert.AreNotEqual(Items[0].ExtendedPropertyAnswers.Count, itemToUpdate.ExtendedPropertyAnswers.Count);
            Assert.AreNotEqual(Items[0].Tags.Count, itemToUpdate.Tags.Count);
            Assert.AreNotEqual(Items[0].LinkLink, itemToUpdate.LinkLink);
            Assert.AreNotEqual(Items[0].MapLink, itemToUpdate.MapLink);

            Controller.Edit(1, itemToUpdate, epp, new[] { Tags[0].Name }, mapLinkText)
                .AssertViewRendered()
                .WithViewData<ItemViewModel>();
            Assert.AreEqual("Item has been saved successfully.", Controller.Message);
            ItemRepository.AssertWasCalled(a => a.EnsurePersistent(Items[0]));

            Assert.AreEqual(Items[0].Name, itemToUpdate.Name);
            Assert.AreEqual(Items[0].Description, itemToUpdate.Description);
            Assert.AreEqual(Items[0].CostPerItem, itemToUpdate.CostPerItem);
            Assert.AreEqual(Items[0].Quantity, itemToUpdate.Quantity);
            Assert.AreEqual(Items[0].Expiration, itemToUpdate.Expiration);
            Assert.AreEqual(Items[0].Link, itemToUpdate.Link);
            Assert.AreEqual(Items[0].Available, itemToUpdate.Available);
            Assert.AreEqual(Items[0].Private, itemToUpdate.Private);
            Assert.AreEqual("45678", Items[0].Image.ByteArrayToString());
            Assert.AreEqual(Items[0].ExtendedPropertyAnswers.Count, 2);
            Assert.AreEqual(Items[0].Tags.Count, 1);
            Assert.IsTrue(Items[0].MapLink.StartsWith("http://maps.google.com/maps?f=q&source=s_q&"));
            Assert.IsTrue(Items[0].LinkLink.StartsWith("http://maps.google.com/maps?f=q&source=embed"));
        }


        #endregion Edit Tests
    }
}
