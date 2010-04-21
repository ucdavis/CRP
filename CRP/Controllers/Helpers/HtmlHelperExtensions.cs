﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using CRP.Controllers.Helpers;
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
                Theme = "clean",
                PublicKey = ConfigurationManager.AppSettings["RecaptchaPublicKey"],
                PrivateKey = ConfigurationManager.AppSettings["RecaptchaPrivateKey"]
            };

            var htmlWriter = new HtmlTextWriter(new StringWriter());

            captchaControl.RenderControl(htmlWriter);

            return htmlWriter.InnerWriter.ToString();
        }

        private const string htmlTag = @"&lt;{0}&gt;";

        /// <summary>
        /// This allows limited html encoding, while still encoding the rest of the string
        /// </summary>
        /// <remarks>
        /// The selected tags are what have been allowed with the TinyMce Text Editor
        /// 
        /// Allows:
        ///     <p></p>
        ///     <strong></strong>
        ///     <em></em>
        ///     <span style="text-decoration:underline;"></span>    // based on what is generated by TinyMce
        ///     <ul></ul>
        ///     <ol></ol>
        ///     <li></li>
        ///     <h1></h1>
        ///     &nbsp;
        ///     <address></address>
        /// </remarks>
        /// <param name="helper"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string HtmlEncode(this HtmlHelper helper, string text)
        {
            // encode the string
            string encodedText = HttpUtility.HtmlEncode(text);

            // put the text in a string builder
            StringBuilder formattedEncodedText = new StringBuilder(encodedText);

            // replace the escaped characters with the correct strings to allow formatting
            ReplaceTagContents(formattedEncodedText, "p");
            ReplaceTagContents(formattedEncodedText, "strong");
            ReplaceTagContents(formattedEncodedText, "em");
            ReplaceTagContents(formattedEncodedText, "ul");
            ReplaceTagContents(formattedEncodedText, "ol");
            ReplaceTagContents(formattedEncodedText, "li");
            ReplaceTagContents(formattedEncodedText, "address");
            ReplaceTagContents(formattedEncodedText, "h1");
            ReplaceTagContents(formattedEncodedText, "h2");
            ReplaceTagContents(formattedEncodedText, "h3");
            ReplaceTagContents(formattedEncodedText, "h4");
            ReplaceTagContents(formattedEncodedText, "h5");
            ReplaceTagContents(formattedEncodedText, "h6");
            ReplaceSingleTagContents(formattedEncodedText, "br");


            // <span style="text-decoration:underline;">
            //string underline = @"&lt;span style=&quot;text-decoration: underline;&quot;&gt;";
            string underline = string.Format(htmlTag, @"span style=&quot;text-decoration: underline;&quot;");
            string underlineReplacement = @"<span style=""text-decoration:underline;"">";
            formattedEncodedText.Replace(underline, underlineReplacement);

            // </span>
            // only find the spans that are related to the span for underlining
            var temp = formattedEncodedText.ToString();
            // for each instance of underline
            foreach (int i in temp.IndexOfAll(underlineReplacement))
            {
                // find the first instance of </span> after the underline span and replace
                var index = temp.IndexOf(string.Format(htmlTag, @"/span"), i);
                
                // delete the string at that location
                temp = temp.Remove(index, string.Format(htmlTag, @"/span").Length);

                // add in the new string at that location
                temp = temp.Insert(index, @"</span>");
            }

            formattedEncodedText = new StringBuilder(temp);

            return formattedEncodedText.ToString();
        }

        public static void ReplaceTagContents(StringBuilder formattedText, string tag)
        {
            // opening tag
            formattedText.Replace(string.Format(htmlTag, tag), @"<"+tag+">");
            // closing tag
            formattedText.Replace(string.Format(htmlTag, @"/" + tag), @"</"+tag+">");
        }
        public static void ReplaceSingleTagContents(StringBuilder formattedText, string tag)
        {
            // opening tag
            formattedText.Replace(string.Format(htmlTag, tag + @" /"), @"<" + tag + @" />");
        }
    }
}