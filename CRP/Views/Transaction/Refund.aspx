<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.EditTransactionViewModel>" %>
<%@ Import Namespace="CRP.Controllers"%>
<%@ Import Namespace="CRP.Controllers.Helpers" %>
<%@ Import Namespace="CRP.Core.Resources" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Refund
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Refund</h2>
    <h3>Internal use only</h3>

    <%= Html.ValidationSummary("Refund was unsuccessful. Please correct the errors and try again.") %>

    <% using (Html.BeginForm()) {%>
    <%= Html.AntiForgeryToken() %>
    <%= Html.Hidden("RefundSort", Model.Sort) %>
    <%= Html.Hidden("RefundPage", Model.Page)%>
        <fieldset>
            <legend>Fields</legend>
            <ul>
                <li>
                    Transaction Number: <%= Html.Encode(Model.TransactionValue.TransactionNumber)%>
                </li>               
                <li>
                    Transaction Unique Identifier (Needed for Credit Card Refund): 
                    <%= Html.Encode(Model.TransactionValue.TransactionGuid + Model.Fid)%>
                </li>
                <li>
                    Transaction Date:
                    <%= Html.Encode(Model.TransactionValue.TransactionDate)%>
                </li>    
                <li>
                    Contact Name:
                    <%= Html.Encode(Model.ContactName)%>
                </li>
                <li>
                    Contact Email:
                    <%= Html.Encode(Model.ContactEmail)%>
                </li> 
                <li>
                    Payment Method: <%= Model.TransactionValue.Check ? "Check" : "Credit Card" %>
                </li>                
                <li>
                    Amount:
                    <%= Html.Encode(Model.TransactionValue.AmountTotal.ToString("C"))%>
                </li>
                <li>
                    Donation Total:
                    <%= Html.Encode(Model.TransactionValue.DonationTotal.ToString("C"))%>
                </li>
                <li>
                    Total:
                    <%= Html.Encode(Model.TransactionValue.Total.ToString("C"))%>
                </li>
                <li>
                    Total Paid:
                    <%= Html.Encode(Model.TransactionValue.TotalPaid.ToString("C"))%>
                </li>

                <li>
                    <label for="Amount">Refund Amount:</label>
                    <%= Html.TextBox("Amount", Model.Amount != 0 ? string.Format("{0:0.00}", Model.Amount) : string.Empty, new {@class="amount"}) %>
                    <%= Html.ValidationMessage("Amount", "*") %>
                </li>
                
                <li>
                    <label for="CorrectionReason">Refund Reason:</label>
                    <%= Html.TextArea("CorrectionReason", Model.CorrectionReason)%>
                    <%= Html.ValidationMessage("CorrectionReason", "*")%>
                </li>
            <li><input type="submit" value="Refund" /></li>   
            </ul>
        </fieldset>

    <% } %>

    <div>
        <%= Url.DetailItemLink(Model.TransactionValue.Item.Id, StaticValues.Tab_Refunds, Model.Sort, Model.Page)%>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="PageHeader" runat="server">
</asp:Content>

