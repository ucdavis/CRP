using System.Xml;

namespace CRP.Controllers.Helpers
{
    public class GoogleMapHelper
    {
        /// <summary>
        /// Extract the link that generates the embedded version of the map
        /// </summary>
        /// <param name="linkText"></param>
        /// <returns></returns>
        public static string ParseEmbeddedLink(string linkText)
        {
            try
            {
                linkText = "<google>" + linkText + "</google>";

                var xDoc = new XmlDocument();
                xDoc.LoadXml(linkText);

                var iframe = xDoc.GetElementsByTagName("iframe")[0];
                var link = iframe.Attributes["src"];

                return link.Value;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Extract the link that generates a link to google maps
        /// </summary>
        /// <param name="linkText"></param>
        /// <returns></returns>
        public static string ParseLinkLink(string linkText)
        {
            try
            {
                linkText = "<google>" + linkText + "</google>";

                var xDoc = new XmlDocument();
                xDoc.LoadXml(linkText);

                var anchor = xDoc.GetElementsByTagName("a")[0];
                var link = anchor.Attributes["href"];

                return link.Value;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
