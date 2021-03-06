using System.IO;
using System.Web.Mvc;
//using CRP.App_GlobalResources;
using CRP.Controllers.Filter;
using CRP.Controllers.Helpers;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using CRP.Core.Resources;
using MvcContrib.Attributes;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.Controller;
using UCDArch.Web.Validator;
using MvcContrib;
using System.Linq;

namespace CRP.Controllers
{
    public class DisplayProfileController : ApplicationController
    {
        private readonly IRepositoryWithTypedId<School, string> _schoolRepository;

        public DisplayProfileController(IRepositoryWithTypedId<School, string> schoolRepository)
        {
            _schoolRepository = schoolRepository;
        }

        //
        // GET: /DisplayProfile/

        public ActionResult Index()
        {
            return this.RedirectToAction(a => a.List());
        }

        /// <summary>
        /// GET: /ApplicationManagement/ListDisplayProfiles
        /// </summary>
        /// <returns></returns>
        [AdminOnlyAttribute]
        public ActionResult List()
        {
            return View(Repository.OfType<DisplayProfile>().Queryable);
        }

        /// <summary>
        /// GET: /ApplicationManagement/CreateDisplayProfile
        /// </summary>
        /// <returns></returns>
        [AdminOnlyAttribute]
        public ActionResult Create()
        {
            return View(DisplayProfileViewModel.Create(Repository, _schoolRepository));
        }

        /// <summary>
        /// POST: /ApplicationManagement/CreateDisplayProfile
        /// </summary>
        /// <remarks>
        /// Description:
        ///     Creates a display profile
        /// PreCondition:
        ///     No other display profile exists for the selected unit
        /// PostCondition:
        ///     Display profile is created
        /// </remarks>
        /// <param name="displayProfile"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(DisplayProfile displayProfile)
        {
            if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0)
            {
                var file = Request.Files[0];
                var reader = new BinaryReader(file.InputStream);
                displayProfile.Logo = reader.ReadBytes(file.ContentLength);
            }

            if (displayProfile.School != null)
            {
                displayProfile.SchoolMaster = true;
            }
            if (displayProfile.Unit != null)
            {
                //TODO: Do test to make sure a duplicate is not being created.

                if(Repository.OfType<DisplayProfile>().Queryable.Where(a => a.Unit == displayProfile.Unit).Any())
                {
                    ModelState.AddModelError("Department", "Display Profile has already been created for this Department.");
                }
            }

            MvcValidationAdapter.TransferValidationMessagesTo(ModelState, displayProfile.ValidationResults());

            //Moved to domain validation
            // atleast one must be selected
            //if (displayProfile.Unit == null && displayProfile.School == null)
            //{
            //    ModelState.AddModelError("Unit/School", "A Unit or School must be specified.");
            //}

            //// but not both
            //if (displayProfile.Unit != null && displayProfile.School != null)
            //{
            //    ModelState.AddModelError("Unit/School", "Unit and School cannot be selected together.");
            //}

            if (ModelState.IsValid)
            {
                Repository.OfType<DisplayProfile>().EnsurePersistent(displayProfile);
                Message = NotificationMessages.STR_ObjectCreated.Replace(NotificationMessages.ObjectType, "Display Profile");
                return this.RedirectToAction(a => List());
            }
            else
            {
                var viewModel = DisplayProfileViewModel.Create(Repository, _schoolRepository);
                viewModel.DisplayProfile = displayProfile;
                return View(viewModel);
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            var displayProfile = Repository.OfType<DisplayProfile>().GetNullableById(id);

            if (displayProfile == null)
            {
                return this.RedirectToAction(a => a.List());
            }

            return View(displayProfile);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id, [Bind(Exclude = "Id")]DisplayProfile displayProfile)
        {
            // get the original item out
            var destProfile = Repository.OfType<DisplayProfile>().GetNullableById(id);

            //Done: Suggest fix for when a passed Id is not found.
            if (destProfile == null)
            {
                Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "DisplayProfile");
                return this.RedirectToAction(a => a.List());
            }

            // copy the display profile properties
            destProfile = Copiers.CopyDisplayProfile(displayProfile, destProfile);

            if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0)
            {
                var file = Request.Files[0];
                var reader = new BinaryReader(file.InputStream);
                destProfile.Logo = reader.ReadBytes(file.ContentLength);
            }

            MvcValidationAdapter.TransferValidationMessagesTo(ModelState, destProfile.ValidationResults());

            if (ModelState.IsValid)
            {
                Repository.OfType<DisplayProfile>().EnsurePersistent(destProfile);
                Message = NotificationMessages.STR_ObjectSaved.Replace(NotificationMessages.ObjectType, "Display Profile");
                return this.RedirectToAction(a => List());
            }
            else
            {
                //var viewModel = DisplayProfileViewModel.Create(Repository, _schoolRepository);
                //viewModel.DisplayProfile = displayProfile;
                return View(destProfile);
            }
        }

        /// <summary>
        /// GET: /DisplayProfile/GetLogo/{id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetLogo(int id)
        {
            var displayProfile = Repository.OfType<DisplayProfile>().GetNullableById(id);
            //if(displayProfile == null)
            //{
            //    return null;
            //}
            return File(displayProfile.Logo, "image/jpg");
        }
    }
}
