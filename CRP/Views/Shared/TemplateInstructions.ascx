<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="CRP.Core.Resources" %>
        <ul> The following are a list of values that can be entered into the text and be dynamically replaced. Include the "{}":
        <li><a href="javascript:;" class="add_token">{StudentId}</a></li>
        <li>{FirstName}         The Contact Information's First Name.</li>
        <li>{LastName}          The Contact Information's Last Name.</li>
        <li>{TotalPaid}         The total amount of accepted payments.</li>
        <li>{Quantity}          The number of items purchased.</li>
        <li>{QuantityName}      The Name of the items.</li>
        <li>{TransactionNumber} The transaction number. Can be used with the email to lookup the transaction.</li>
        <li>{PaymentMethod}     "Check" or "Credit Card" sans quotes.</li>
        <li>{DonationThanks}    If a donation is detected the text "<%=String.Format(Html.Encode(ScreenText.STR_DonationText), "xxx") %>" Sans quotes where xxx is replaced with the amount.</li>
        </ul>
        

