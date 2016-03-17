<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="CRP.Core.Resources" %>
    
    <div id="Confirmation_Word_Bank">
        <strong style="font-family:Trebuchet MS,Arial,Georgia; font-size:1.2em;">Template Fields: </strong> <%=Html.Image("~/images/question_blue.png", new { @id = "ConfirmationWordBankHelp" })%><br>
        <a href="javascript:;" class="add-token" name = "{FirstName}">First Name</a><br>         
        <a href="javascript:;" class="add-token" name = "{LastName}">Last Name</a><br>
        <a href="javascript:;" class="add-token" name = "{TotalPaid}">Total Paid</a><br>         
        <a href="javascript:;" class="add-token" name = "{Quantity}">Quantity</a><br>          
        <a href="javascript:;" class="add-token" name = "{QuantityName}">Quantity Name</a><br>      
        <a href="javascript:;" class="add-token" name = "{TransactionNumber}">Transaction Number</a><br> 
        <a href="javascript:;" class="add-token" name = "{PaymentMethod}">Payment Method</a><br>     
        <a href="javascript:;" class="add-token" name = "{DonationThanks}">Donation Thanks</a><br>    
   </div>
       <div id="Confirmation_Word_Bank_Unpaid">
        <strong style="font-family:Trebuchet MS,Arial,Georgia; font-size:1.2em;">Template Fields: </strong> <%=Html.Image("~/images/question_blue.png", new { @id = "ConfirmationWordBankHelpUnpaid" })%><br>
        <a href="javascript:;" class="add-token-unpaid" name = "{FirstName}">First Name</a><br>         
        <a href="javascript:;" class="add-token-unpaid" name = "{LastName}">Last Name</a><br>
        <a href="javascript:;" class="add-token-unpaid" name = "{TotalPaid}">Total Paid</a><br>         
        <a href="javascript:;" class="add-token-unpaid" name = "{Quantity}">Quantity</a><br>          
        <a href="javascript:;" class="add-token-unpaid" name = "{QuantityName}">Quantity Name</a><br>      
        <a href="javascript:;" class="add-token-unpaid" name = "{TransactionNumber}">Transaction Number</a><br> 
        <a href="javascript:;" class="add-token-unpaid" name = "{PaymentMethod}">Payment Method</a><br>     
        <a href="javascript:;" class="add-token-unpaid" name = "{DonationThanks}">Donation Thanks</a><br>    
   </div>
        <%--<ul> The following are a list of values that can be clicked on to insert into the text where the cursor is. The values will be dynamically replaced when used.:
        <li><a href="javascript:;" class="add-token" style="Color:Blue">{FirstName}</a>         The Contact Information's First Name.</li>
        <li><a href="javascript:;" class="add-token" style="Color:Blue">{LastName}</a>          The Contact Information's Last Name.</li>
        <li><a href="javascript:;" class="add-token" style="Color:Blue">{TotalPaid}</a>         The total amount of accepted payments.</li>
        <li><a href="javascript:;" class="add-token" style="Color:Blue">{Quantity}</a>          The number of items purchased.</li>
        <li><a href="javascript:;" class="add-token" style="Color:Blue">{QuantityName}</a>      The Name of the items.</li>
        <li><a href="javascript:;" class="add-token" style="Color:Blue">{TransactionNumber}</a> The transaction number. Can be used with the email to lookup the transaction.</li>
        <li><a href="javascript:;" class="add-token" style="Color:Blue">{PaymentMethod}</a>     "Check" or "Credit Card" sans quotes.</li>
        <li><a href="javascript:;" class="add-token" style="Color:Blue">{DonationThanks}</a>    If a donation is detected the text "<%=String.Format(Html.Encode(ScreenText.STR_DonationText), "xxx") %>" Sans quotes where xxx is replaced with the amount.</li>
        </ul>--%>
        
   <script type="text/javascript">
       $(document).ready(function() {
        $("#ConfirmationWordBankHelp").attr("title", "");
        $("#ConfirmationWordBankHelpUnpaid").attr("title", "");
        $("#ConfirmationWordBankHelp").bt("The following are a list of values that can be clicked on to insert into the text where the cursor is on the Paid Template. The values will be dynamically replaced when used.<br><h3>First Name: </h3>The Contact Information's First Name.<br><h3>Last Name: </h3>The Contact Information's Last Name.<br><h3>Total Paid: </h3>The total amount of accepted payments.<br><h3>Quantity: </h3>The number of items purchased.<br><h3>Quantity Name: </h3>The Name of the items.<br><h3>Transaction Number: </h3>The transaction number. Can be used with the email to lookup the transaction.<br><h3>Payment Method: </h3>'Check' or 'Credit Card' sans quotes.<br><h3>Donation Thanks: </h3>If a donation is detected the text '<%=String.Format(Html.Encode(ScreenText.STR_DonationText), "xxx") %>' Sans quotes where xxx is replaced with the amount.", { width: '550px' });
        $("#ConfirmationWordBankHelpUnpaid").bt("The following are a list of values that can be clicked on to insert into the text where the cursor is on the Unpaid Template. The values will be dynamically replaced when used.<br><h3>First Name: </h3>The Contact Information's First Name.<br><h3>Last Name: </h3>The Contact Information's Last Name.<br><h3>Total Paid: </h3>The total amount of accepted payments.<br><h3>Quantity: </h3>The number of items purchased.<br><h3>Quantity Name: </h3>The Name of the items.<br><h3>Transaction Number: </h3>The transaction number. Can be used with the email to lookup the transaction.<br><h3>Payment Method: </h3>'Check' or 'Credit Card' sans quotes.<br><h3>Donation Thanks: </h3>If a donation is detected the text '<%=String.Format(Html.Encode(ScreenText.STR_DonationText), "xxx") %>' Sans quotes where xxx is replaced with the amount.", { width: '550px' });
       });
   </script>

