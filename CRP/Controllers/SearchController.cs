using System.Net;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using CRP.Controllers.ViewModels;
using UCDArch.Web.Controller;

namespace CRP.Controllers
{
    public class SearchController : SuperController
    {
        //
        // GET: /Search/

        public ActionResult Index(string searchTerm)
        {
            var viewModel = SearchViewModel.Create(Repository);

            viewModel.Suggestion = GetSpellingSuggestion(searchTerm);

            return View(viewModel);
        }

        const string ApiKey = "9F72873DCAD0A2B5FBAEF8B94D138924A0A24ECD";

        private HttpWebRequest BuildRequest(string searchTerm)
        {
            string requestString = "http://api.bing.net/xml.aspx?"

                // Common request fields (required)
                + "AppId=" + ApiKey
                + "&Query=" + searchTerm
                + "&Sources=Spell"

                // Common request fields (optional)
                + "&Version=2.0"
                + "&Market=en-us"
                + "&Options=EnableHighlighting";

            // Create and initialize the request.
            var request = (HttpWebRequest)HttpWebRequest.Create(requestString);

            return request;
        }
        private string GetSpellingSuggestion(string searchTerm)
        {
            var response = BuildRequest(searchTerm).GetResponse();

            // Load the response into an XmlDocument.
            XmlDocument document = new XmlDocument();
            document.Load(response.GetResponseStream());

            // Add the default namespace to the namespace manager.
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(
                document.NameTable);
            nsmgr.AddNamespace(
                "api",
                "http://schemas.microsoft.com/LiveSearch/2008/04/XML/element");

            XmlNodeList errors = document.DocumentElement.SelectNodes(
                "./api:Errors/api:Error",
                nsmgr);

            if (errors.Count > 0)
            {
                // There are errors in the response. Display error details.
                return DisplayErrors(errors);
            }
            else
            {
                // There were no errors in the response. Display the
                // Spell results.
                return DisplayResults(document.DocumentElement, nsmgr);
            }
        }

        private string DisplayResults(XmlNode root, XmlNamespaceManager nsmgr)
        {
            var result = new StringBuilder();

            string version = root.SelectSingleNode("./@Version", nsmgr).InnerText;
            string searchTerms = root.SelectSingleNode(
                "./api:Query/api:SearchTerms",
                nsmgr).InnerText;

            // Display the results header.
            result.Append("Bing API Version " + version);
            result.Append("Spell results for " + searchTerms);
            result.Append("<br/>");

            // Add the Spell SourceType namespace to the namespace manager.
            nsmgr.AddNamespace(
                "spl",
                "http://schemas.microsoft.com/LiveSearch/2008/04/XML/spell");

            XmlNodeList results = root.SelectNodes(
                "./spl:Spell/spl:Results/spl:SpellResult",
                nsmgr);

            // Display the Spell results.
            foreach (XmlNode r in results)
            {
                result.Append(r.SelectSingleNode("./spl:Value", nsmgr).InnerText);
                result.Append("<br/>");
            }

            return result.ToString();
        }

        static string DisplayErrors(XmlNodeList errors)
        {
            var result = new StringBuilder();

            // Iterate over the list of errors and display error details.
            result.Append("Errors:");
            result.Append("<br/>");
            foreach (XmlNode error in errors)
            {
                foreach (XmlNode detail in error.ChildNodes)
                {
                    result.Append(detail.Name + ": " + detail.InnerText);
                }

                result.Append("<br/>");
            }

            return result.ToString();
        }

    }
}