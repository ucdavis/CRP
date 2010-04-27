using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRP.Controllers.Helpers
{
    public static class ValidateParameters
    {
        /// <summary>
        /// Validates the parameters.
        /// </summary>
        /// <param name="pageName">The page name.</param>
        /// <param name="sort">The sort.</param>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        public static Dictionary<string, string> PageAndSort(string pageName, string sort, string page)
        {
            var rtValue = new Dictionary<string, string>(2);

            Int32 validPage;
            if (!int.TryParse(page, out validPage))
            {
                validPage = 1;
            }
            rtValue.Add("page", validPage.ToString());

            var validSort = new List<string>();
            validSort.Add("TransactionNumber-asc");
            validSort.Add("TransactionNumber-desc");
            validSort.Add("Quantity-asc");
            validSort.Add("Quantity-desc");
            validSort.Add("Paid-asc");
            validSort.Add("Paid-desc");
            validSort.Add("IsActive-asc");
            validSort.Add("IsActive-desc");
            if (validSort.Contains(sort))
            {
                rtValue.Add("sort", sort);
            }
            else
            {
                rtValue.Add("sort", string.Empty);
            }

            return rtValue;
        }  
    }
}
