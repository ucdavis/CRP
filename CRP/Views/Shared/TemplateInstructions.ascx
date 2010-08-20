<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="CRP.Core.Resources" %>
        <ul> The following are a list of values that can be clicked on to insert into the text where the cursor is. The values will be dynamically replaced when used.:
        <li><a href="javascript:;" class="add-token" style="Color:Blue">{FirstName}</a>         The Contact Information's First Name.</li>
        <li><a href="javascript:;" class="add-token" style="Color:Blue">{LastName}</a>          The Contact Information's Last Name.</li>
        <li><a href="javascript:;" class="add-token" style="Color:Blue">{TotalPaid}</a>         The total amount of accepted payments.</li>
        <li><a href="javascript:;" class="add-token" style="Color:Blue">{Quantity}</a>          The number of items purchased.</li>
        <li><a href="javascript:;" class="add-token" style="Color:Blue">{QuantityName}</a>      The Name of the items.</li>
        <li><a href="javascript:;" class="add-token" style="Color:Blue">{TransactionNumber}</a> The transaction number. Can be used with the email to lookup the transaction.</li>
        <li><a href="javascript:;" class="add-token" style="Color:Blue">{PaymentMethod}</a>     "Check" or "Credit Card" sans quotes.</li>
        <li><a href="javascript:;" class="add-token" style="Color:Blue">{DonationThanks}</a>    If a donation is detected the text "<%=String.Format(Html.Encode(ScreenText.STR_DonationText), "xxx") %>" Sans quotes where xxx is replaced with the amount.</li>
        </ul>
        

