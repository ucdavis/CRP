using System.Web;
using System.Web.Optimization;

namespace CRP.Mvc
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //Needs <script src="https://ajax.microsoft.com/ajax/jquery.validate/1.8.1/jquery.validate.min.js"></script>
            bundles.Add(new ScriptBundle("~/bundles/checkout").Include(
                        //"~/Scripts/xVal.jquery.validate.js", //Don't need this anymore?
                        "~/Scripts/RenameForArray.js",
                        "~/Scripts/jquery.PhoneValidator.js",
                        "~/Scripts/jquery.ZipValidator.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/watermark").Include(
                "~/Scripts/jquery.watermark.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/map").Include(
                "~/Scripts/jquery.gPositions.js"));

            bundles.Add(new ScriptBundle("~/bundles/qtip").Include(
                "~/Scripts/qTip/jquery.qtip.min.js"));

            bundles.Add(new StyleBundle("~/Content/qtipcss").Include(
                "~/Scripts/qTip/jquery.qtip.min.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/public-css").Include(
                "~/Content/public-main.css"));

            bundles.Add(new StyleBundle("~/Content/gPositions").Include(
                "~/Content/jquery.gPositions.css"));

            bundles.Add(new StyleBundle("~/Content/gPositionsNew").Include(
                "~/Content/jquery.gPositionsNew.css"));
        }
    }
}
