using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using CRP.App_GlobalResources;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using MvcContrib.Attributes;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Controller;
using UCDArch.Web.Validator;
using MvcContrib;

namespace CRP.Controllers
{
    [Authorize(Roles="User")]
    public class ItemManagementController : SuperController
    {
        //
        // GET: /ItemManagement/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            // list items that the user has editor rights to

            var user = Repository.OfType<User>().Queryable.Where(a => a.LoginID == CurrentUser.Identity.Name).FirstOrDefault();

            var query = from i in Repository.OfType<Item>().Queryable
                        where i.Editors.Any(a => a.User == user)
                        select i;

            return View(query);
        }

        public ActionResult Create()
        {
            var viewModel = ItemViewModel.Create(Repository);

            return View(viewModel);
        }

        /// <summary>
        /// POST: /ItemManagement/Create/
        /// </summary>
        /// <remarks>
        /// Description:   
        ///     Create a new item
        /// PreCondition:
        ///     Item type was created
        /// PostCondition:
        ///     Item is created
        ///     Extended properties are created
        /// </remarks>
        /// <param name="item"></param>
        /// <returns></returns>
        [AcceptPost]
        public ActionResult Create(Item item, ExtendedPropertyParameter[] ExtendedProperties, string[] Tags)
        {
            // get the file and add it into the item
            if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0)
            {
                var file = Request.Files[0];
                var reader = new BinaryReader(file.InputStream);
                item.Image = reader.ReadBytes(file.ContentLength);
            }

            // get the extended properties
            foreach(var exp in ExtendedProperties)
            {
                if (!string.IsNullOrEmpty(exp.value))
                {
                    // get the extended property object
                    var extendedProperty = Repository.OfType<ExtendedProperty>().GetNullableByID(exp.propertyId);

                    var answer = new ExtendedPropertyAnswer(exp.value, item, extendedProperty);
                    item.AddExtendedPropertyAnswer(answer);
                }
            }

            // go through and deal with the tags
            var tags = Repository.OfType<Tag>().GetAll();

            foreach(var s in Tags)
            {
                var tag = tags.FirstOrDefault(a => a.Name == s);
                if (tag == null)
                {
                    // create a new one
                    tag = new Tag(s);
                }
                
                item.AddTag(tag);
            }

            var user = Repository.OfType<User>().Queryable.Where(a => a.LoginID == CurrentUser.Identity.Name).FirstOrDefault();

            // add permissions
            var editor = new Editor(item, user);
            editor.Owner = true;
            item.AddEditor(editor);

            // set the unit
            item.Unit = user.Units.FirstOrDefault();

            MvcValidationAdapter.TransferValidationMessagesTo(ModelState, item.ValidationResults());

            if (ModelState.IsValid)
            {
                Repository.OfType<Item>().EnsurePersistent(item);
                Message = NotificationMessages.STR_ObjectCreated.Replace(NotificationMessages.ObjectType, "Item");
                return this.RedirectToAction(a => a.List());
            }
            else
            {
                var viewModel = ItemViewModel.Create(Repository);
                viewModel.Item = item;
                return View(viewModel);
            }
        }

        public JsonNetResult GetExtendedProperties(int id)
        {
            var itemType = Repository.OfType<ItemType>().GetNullableByID(id);

            if (itemType == null)
            {
                return new JsonNetResult(false);
            }

            return new JsonNetResult(itemType.ExtendedProperties);
        }

        public ActionResult Edit(int id)
        {
            var item = Repository.OfType<Item>().GetNullableByID(id);
            if (item == null)
            {
                return this.RedirectToAction(a => a.List());
            }
            
            var viewModel = ItemViewModel.Create(Repository);
            viewModel.Item = item;

            return View(viewModel);
        }

        /// <summary>
        /// POST: /ItemManagement/Edit/{id}
        /// </summary>
        /// <remarks>
        /// Description:
        /// PreCondition:
        /// PostCondition:
        /// </remarks>
        /// <param name="item"></param>
        /// <param name="extendedProperties"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        [AcceptPost]
        public ActionResult Edit([Bind(Exclude="Id")]Item item, ExtendedPropertyParameter[] extendedProperties, string[] tags)
        {
            throw new NotImplementedException();
        }
    }

    public class ExtendedPropertyParameter
    {
        public string value { get; set; }
        public int propertyId { get; set; }
    }
}
