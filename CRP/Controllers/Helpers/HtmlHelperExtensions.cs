using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web.Mvc;
using System.Web.UI;
using Telerik.Web.Mvc.UI;

namespace CRP.Helpers
{
    public static class HtmlHelperExtensions
    {
        public static CustomGridBuilder<T> Grid<T>(this HtmlHelper htmlHelper, IEnumerable<T> dataModel) where T : class
        {
            var builder = htmlHelper.Telerik().Grid(dataModel);

            return new CustomGridBuilder<T>(builder);
        }

        public static string GenerateCaptcha(this HtmlHelper helper)
        {

            var captchaControl = new Recaptcha.RecaptchaControl
            {
                ID = "recaptcha",
                Theme = "red",
                PublicKey = ConfigurationManager.AppSettings["RecaptchaPublicKey"],
                PrivateKey = ConfigurationManager.AppSettings["RecaptchaPrivateKey"]
            };

            var htmlWriter = new HtmlTextWriter(new StringWriter());

            captchaControl.RenderControl(htmlWriter);

            return htmlWriter.InnerWriter.ToString();
        }
    }
}