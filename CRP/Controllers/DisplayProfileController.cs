using System.IO;
using System.Web.Mvc;
using CRP.App_GlobalResources;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using MvcContrib.Attributes;
using UCDArch.Web.Controller;
using UCDArch.Web.Validator;
using MvcContrib;

namespace CRP.Controllers
{
    public class DisplayProfileController : SuperController
    {
        //
        // GET: /DisplayProfile/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// GET: /ApplicationManagement/ListDisplayProfiles
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles="Admin")]
        public ActionResult List()
        {
            return View(Repository.OfType<DisplayProfile>().Queryable);
        }

        /// <summary>
        /// GET: /ApplicationManagement/CreateDisplayProfile
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View(DisplayProfileViewModel.Create(Repository));
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
        [AcceptPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(DisplayProfile displayProfile)
        {
            var test = Request;

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

            MvcValidationAdapter.TransferValidationMessagesTo(ModelState, displayProfile.ValidationResults());

            // atleast one must be selected
            if (displayProfile.Unit == null && displayProfile.School == null)
            {
                ModelState.AddModelError("Unit/School", "A Unit or School must be specified.");
            }

            // but not both
            if (displayProfile.Unit != null && displayProfile.School != null)
            {
                ModelState.AddModelError("Unit/School", "Unit and School cannot be selected together.");
            }

            if (ModelState.IsValid)
            {
                Repository.OfType<DisplayProfile>().EnsurePersistent(displayProfile);
                Message = NotificationMessages.STR_ObjectCreated.Replace(NotificationMessages.ObjectType, "Display Profile");
                return this.RedirectToAction(a => List());
            }
            else
            {
                var viewModel = DisplayProfileViewModel.Create(Repository);
                viewModel.DisplayProfile = displayProfile;
                return View(viewModel);
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            var displayProfile = Repository.OfType<DisplayProfile>().GetNullableByID(id);

            if (displayProfile == null)
            {
                return this.RedirectToAction(a => a.List());
            }

            return View(displayProfile);
        }

        /// <summary>
        /// GET: /DisplayProfile/GetLogo/{id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetLogo(int id)
        {
            var displayProfile = Repository.OfType<DisplayProfile>().GetNullableByID(id);

            return File(displayProfile.Logo, "image/jpg");
        }
    }
}
