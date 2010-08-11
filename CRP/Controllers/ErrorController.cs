using System.Web.Mvc;
using MvcContrib;
using UCDArch.Web.Controller;

namespace CRP.Controllers
{
    public class ErrorController : SuperController
    {
        //
        // GET: /Error/

        public ActionResult Index(ErrorType? errorType)
        {
            string title, description;

            // set the default error type
            if (errorType == null) { errorType = ErrorType.UnknownError; }

            switch (errorType)
            {
                case ErrorType.UnauthorizedAccess:
                    title = "Unauthorized Access";
                    description = "You are not authorized for your request.";
                    break;
                case ErrorType.FileDoesNotExist:
                    title = "File Does Not Exist";
                    description = "File not found.";
                    break;
                case ErrorType.FileNotFound:
                    title = "File Does Not Exist";
                    description = "File not found.";
                    break;
                case ErrorType.StudentNotFound:
                    title = "Student not found";
                    description = "The student you are looking for was not found.";
                    break;
                default:
                    title = "Unknown Error.";
                    description = "An unknown error has occurred.  IT has been notified of the issue.";
                    break;
            };

            return View(ErrorViewModel.Create(title, description));
        }

        public RedirectToRouteResult FileNotFound()
        {
            return this.RedirectToAction(a => a.Index(ErrorType.FileNotFound));
        }

        public RedirectToRouteResult NotAuthorized()
        {
            return this.RedirectToAction(a => a.Index(ErrorType.UnauthorizedAccess));
        }

        public enum ErrorType
        {
            UnauthorizedAccess = 0,
            FileDoesNotExist,
            FileNotFound,
            UnknownError,
            StudentNotFound
        }

    }

    public class ErrorViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public static ErrorViewModel Create(string title, string description)
        {
            return new ErrorViewModel() { Title = title, Description = description };
        }
    }
}
