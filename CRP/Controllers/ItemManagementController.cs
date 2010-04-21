using System.IO;
using System.Linq;
using System.Web.Mvc;
using CRP.Controllers.Filter;
using CRP.Controllers.Helpers;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using CRP.Core.Resources;
using MvcContrib;
using MvcContrib.Attributes;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Controller;
using UCDArch.Web.Validator;

namespace CRP.Controllers
{
    [UserOnlyAttribute]
    public class ItemManagementController : SuperController
    {
        //
        // GET: /ItemManagement/

        public ActionResult Index()
        {
            return this.RedirectToAction(a => a.List());
        }

        /// <summary>
        /// GET: /ItemManagement/List
        /// </summary>
        /// <returns></returns>
        public ActionResult List()
        {
            // list items that the user has editor rights to
            var user = Repository.OfType<User>().Queryable.Where(a => a.LoginID == CurrentUser.Identity.Name).FirstOrDefault();

            var query = Repository.OfType<Item>().Queryable.Where(a => a.Editors.Any(b => b.User == user));
            // admins can see all
            if (CurrentUser.IsInRole(RoleNames.Admin))
            {
                query = Repository.OfType<Item>().Queryable;
            }

            return View(query);
        }

        /// <summary>
        /// GET: /ItemManagement/Create
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            var viewModel = ItemViewModel.Create(Repository, CurrentUser, null);

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
        ///     Default question sets are added ("Contact Information" and Item Type defaults)
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="extendedProperties">The extended properties.</param>
        /// <param name="tags">The tags.</param>
        /// <param name="mapLink">The map link.</param>
        /// <returns></returns>
        [AcceptPost]
        [ValidateInput(false)]
        public ActionResult Create(Item item, ExtendedPropertyParameter[] extendedProperties, string[] tags, string mapLink)
        {
            // get the file and add it into the item
            if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0)
            {
                var file = Request.Files[0];
                var reader = new BinaryReader(file.InputStream);
                item.Image = reader.ReadBytes(file.ContentLength);
            }

            // process the extended properties and tags
            //item = PopulateObject.Item(Repository, item, extendedProperties, tags);
            item = Copiers.PopulateItem(Repository, item, extendedProperties, tags, mapLink);

            var user = Repository.OfType<User>().Queryable.Where(a => a.LoginID == CurrentUser.Identity.Name).FirstOrDefault();

            // add permissions
            var editor = new Editor(item, user);
            editor.Owner = true;
            item.AddEditor(editor);

            // set the unit
            //item.Unit = user.Units.FirstOrDefault();

            // add the required question set
            var questionSet =
                Repository.OfType<QuestionSet>().Queryable.Where(
                    a => a.Name == StaticValues.QuestionSet_ContactInformation).FirstOrDefault();

            // this should really not be null at any point.
            if (questionSet != null)
            {
                item.AddTransactionQuestionSet(questionSet);
            }

            // add the default question sets
            foreach(var qs in item.ItemType.QuestionSets)
            {
                if (qs.TransactionLevel)
                {
                    item.AddTransactionQuestionSet(qs.QuestionSet);
                }
                else
                {
                    item.AddQuantityQuestionSet(qs.QuestionSet);
                }
            }

            MvcValidationAdapter.TransferValidationMessagesTo(ModelState, item.ValidationResults());

            if (ModelState.IsValid)
            {
                Repository.OfType<Item>().EnsurePersistent(item);
                Message = NotificationMessages.STR_ObjectCreated.Replace(NotificationMessages.ObjectType, "Item");
                return this.RedirectToAction(a => a.List());
            }
            else
            {
                var viewModel = ItemViewModel.Create(Repository, CurrentUser, item);                
                return View(viewModel);
            }
        }

        /// <summary>
        /// GET: /ItemManagement/GetExtendedProperties/{id}
        /// </summary>
        /// <param name="id">Id of the item type</param>
        /// <returns></returns>
        public JsonNetResult GetExtendedProperties(int id)
        {
            var itemType = Repository.OfType<ItemType>().GetNullableByID(id);

            if (itemType == null)
            {
                return new JsonNetResult(false);
            }

            return new JsonNetResult(itemType.ExtendedProperties);
        }

        /// <summary>
        /// GET: /ItemManagement/Edit/{id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            var item = Repository.OfType<Item>().GetNullableByID(id);
            if (item == null || !Access.HasItemAccess(CurrentUser, item))
            {
                return this.RedirectToAction(a => a.List());
            }

            var viewModel = ItemViewModel.Create(Repository, CurrentUser, item);


            return View(viewModel);
        }

        /// <summary>
        /// POST: /ItemManagement/Edit/{id}
        /// </summary>
        /// <remarks>
        /// Description:
        ///     Updates an existing item
        /// PreCondition:
        ///     Item already exists
        ///     User has editor rights
        /// PostCondition:
        ///     Item is updated
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="item">The item.</param>
        /// <param name="extendedProperties">The extended properties.</param>
        /// <param name="tags">The tags.</param>
        /// <param name="mapLink">The map link.</param>
        /// <returns></returns>
        [AcceptPost]
        [ValidateInput(false)]
        public ActionResult Edit(int id, [Bind(Exclude="Id")]Item item, ExtendedPropertyParameter[] extendedProperties, string[] tags, string mapLink)
        {
            var destinationItem = Repository.OfType<Item>().GetNullableByID(id);
            
            // check rights to edit
            if(!Access.HasItemAccess(CurrentUser, destinationItem))
            {
                //Don't Have editor rights
                Message = NotificationMessages.STR_NoEditorRights;
                return this.RedirectToAction(a => a.List());
            }

            destinationItem = Copiers.CopyItem(Repository, item, destinationItem, extendedProperties, tags, mapLink);//PopulateObject.Item(Repository, item, extendedProperties, tags);

            // get the file and add it into the item
            if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0)
            {
                var file = Request.Files[0];
                var reader = new BinaryReader(file.InputStream);
                destinationItem.Image = reader.ReadBytes(file.ContentLength);
            }

            MvcValidationAdapter.TransferValidationMessagesTo(ModelState, destinationItem.ValidationResults());

            if (ModelState.IsValid)
            {
                Repository.OfType<Item>().EnsurePersistent(destinationItem);
                Message = NotificationMessages.STR_ObjectSaved.Replace(NotificationMessages.ObjectType, "Item");
            }

            var viewModel = ItemViewModel.Create(Repository, CurrentUser, destinationItem);
            //viewModel.Item = destinationItem;
            return View(viewModel);
        }

        /// <summary>
        /// POST: /ItemManagement/RemoveEditor
        /// </summary>
        /// <remarks>
        /// Description:
        ///     Removes an editor's permission from the item
        /// PreCondition:
        ///     Editor is associated with the item
        ///     Item is valid
        /// PostCondition:
        ///     Item does not contain specified editor
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="editorId"></param>
        /// <returns></returns>
        [AcceptPost]
        public ActionResult RemoveEditor(int id, int editorId)
        {
            var item = Repository.OfType<Item>().GetNullableByID(id);
            var editor = Repository.OfType<Editor>().GetNullableByID(editorId);

            if (item == null || editor == null)
            {
                return this.RedirectToAction(a => a.List());
            }
            //Only allow an editor to be removed if the current user is in the editors -JCS
            //if (item.Editors == null || !item.Editors.Where(a => a.User.LoginID == CurrentUser.Identity.Name).Any())
            if (!Access.HasItemAccess(CurrentUser, item))
            {
                //Don't Have editor rights
                Message = NotificationMessages.STR_NoEditorRights;
                return this.RedirectToAction(a => a.List());
            }

            if (editor.Owner)
            {
                Message = "Can not remove owner from item.";
            }
            else
            {
                item.RemoveEditor(editor);

                MvcValidationAdapter.TransferValidationMessagesTo(ModelState, item.ValidationResults());

                if (ModelState.IsValid)
                {
                    Repository.OfType<Item>().EnsurePersistent(item);
                    Message = NotificationMessages.STR_ObjectSaved.Replace(NotificationMessages.ObjectType, "Editor");
                }
            }

            //return Redirect(ReturnUrlGenerator.EditItemUrl(id, StaticValues.Tab_Editors));
            return Redirect(Url.EditItemUrl(id, StaticValues.Tab_Editors));
        }

        /// <summary>
        /// POST: /ItemManagement/AddEditor
        /// </summary>
        /// <remarks>
        /// Description:
        ///     Add an editor's permission to the item
        /// PreCondition:
        ///     Editor is not associated with the item
        ///     Item is valid
        ///     User is valid
        /// PostCondition:
        ///     Item has editor added with specified user
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [AcceptPost]
        public ActionResult AddEditor(int id, int? userId)
        {
            if (!userId.HasValue)
            {
                Message = NotificationMessages.STR_SelectUserFirst;
                return Redirect(Url.EditItemUrl(id, StaticValues.Tab_Editors));
            }

            var item = Repository.OfType<Item>().GetNullableByID(id);
            var user = Repository.OfType<User>().GetNullableByID(userId.Value);
            
            if (item == null || user == null)
            {
                return this.RedirectToAction(a => a.List());
            }


            //if (item.Editors == null || !item.Editors.Where(a => a.User.LoginID == CurrentUser.Identity.Name).Any())
            if (!Access.HasItemAccess(CurrentUser, item))
            {
                //Don't Have editor rights
                Message = NotificationMessages.STR_NoEditorRights;
                return this.RedirectToAction(a => a.List());
            }

            //TODO; Review if this is how it should behave if an editor is already attached to the item
            if(item.Editors.Where(a => a.User.LoginID == user.LoginID).Any())
            {
                Message = NotificationMessages.STR_EditorAlreadyExists;
                return Redirect(Url.EditItemUrl(id, StaticValues.Tab_Editors));

            }

            item.AddEditor(new Editor(item, user));

            MvcValidationAdapter.TransferValidationMessagesTo(ModelState, item.ValidationResults());

            if (ModelState.IsValid)
            {
                Repository.OfType<Item>().EnsurePersistent(item);
                Message = NotificationMessages.STR_ObjectSaved.Replace(NotificationMessages.ObjectType, "Editor");
            }
            else
            {
                Message = "Unable to add editor.";
            }

            //return Redirect(ReturnUrlGenerator.EditItemUrl(id, StaticValues.Tab_Editors));
            return Redirect(Url.EditItemUrl(id, StaticValues.Tab_Editors));
        }

        /// <summary>
        /// POST: /ItemManagement/SaveTemplate
        /// </summary>
        /// <remarks>
        /// Description:
        ///     Saves the template
        /// PreCondition:
        ///     Item is valid
        ///     String is not empty
        /// PostCondition:
        ///     Item's template is updated
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        [AcceptPost]
        [ValidateInput(false)]
        public JsonNetResult SaveTemplate(int id, string text)
        {
            var item = Repository.OfType<Item>().GetNullableByID(id);
            
            if (item == null || string.IsNullOrEmpty(text) || !Access.HasItemAccess(CurrentUser, item))
            {
                return new JsonNetResult(false);
            }

            var template = new Template(text);
            item.Template = template;

            MvcValidationAdapter.TransferValidationMessagesTo(ModelState, item.ValidationResults());
            MvcValidationAdapter.TransferValidationMessagesTo(ModelState, template.ValidationResults()); //TODO: Review this extra call

            if (ModelState.IsValid)
            {
                Repository.OfType<Item>().EnsurePersistent(item);
                return new JsonNetResult(true);
            }

            return new JsonNetResult(false);
        }

        /// <summary>
        /// GET: /ItemManagement/Details/{id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int id)
        {
            var item = Repository.OfType<Item>().GetNullableByID(id);
            if (item == null)
            {
                return this.RedirectToAction(a => a.List());
            }

            var viewModel = UserItemDetailViewModel.Create(Repository, item);

            return View(viewModel);
        }
    }

    public class ExtendedPropertyParameter
    {
        // ReSharper disable InconsistentNaming
        public string value { get; set; }
        public int propertyId { get; set; }
        // ReSharper restore InconsistentNaming
    }
}
