<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.EditTransactionViewModel>" %>
<%@ Import Namespace="CRP.Controllers"%>
<%@ Import Namespace="CRP.Controllers.Helpers" %>
<%@ Import Namespace="CRP.Core.Resources" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Edit
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Edit</h2>

    <%= Html.ValidationSummary("Edit was unsuccessful. Please correct the errors and try again.") %>

    <% using (Html.BeginForm()) {%>
    <%= Html.AntiForgeryToken() %>
    <%= Html.Hidden("CheckSort", Model.Sort) %>
    <%= Html.Hidden("CheckPage", Model.Page) %>
        <fieldset>
            <legend>Fields</legend>
            <ul>
                <li>
                    Transaction Number: <%= Html.Encode(Model.TransactionValue.TransactionNumber)%>
                </li>
                <li>
                    Transaction Date:
                    <%= Html.Encode(Model.TransactionValue.TransactionDate)%>
                </li>    
                <li>
                    Amount:
                    <%= Html.Encode(Model.TransactionValue.AmountTotal.ToString("C"))%>
                </li>
                <li>
                    Correction Total:
                    <%= Html.Encode(Model.TransactionValue.CorrectionTotal.ToString("C"))%>
                </li>
                <li>
                    Uncorrected Donation Total:
                    <%= Html.Encode(Model.TransactionValue.UncorrectedDonationTotal.ToString("C"))%>
                </li>
                <li>
                    Current Donation Total:
                    <%= Html.Encode(Model.TransactionValue.DonationTotal.ToString("C"))%>
                </li>
                <li>
                    Total Paid:
                    <%= Html.Encode(Model.TransactionValue.TotalPaid.ToString("C"))%>
                </li>
                 <li>
                    Total:
                    <%= Html.Encode(Model.TransactionValue.Total.ToString("C"))%>
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
                    <label for="Amount">Correction:</label>
                    <%= Html.TextBox("Amount", Model.Amount != 0 ? string.Format("{0:0.00}", Model.Amount) : string.Empty, new {@class="amount"}) %>
                    <%= Html.ValidationMessage("Amount", "*") %>
                </li>
                
                <li>
                    <label for="CorrectionReason">Correction Reason:</label>
                    <%= Html.TextArea("CorrectionReason", Model.CorrectionReason) %>
                    <%= Html.ValidationMessage("CorrectionReason", "*")%>
                </li>
            <li><input type="submit" value="Add Correction" /></li>   
            </ul>
        </fieldset>

    <% } %>

    <div>
        <%--<%=Html.ActionLink<ItemManagementController>(a => a.Details(Model.TransactionValue.Item.Id), "Back to List") %>--%>
        <%= Url.DetailItemLink(Model.TransactionValue.Item.Id, StaticValues.Tab_Checks, Model.Sort, Model.Page)%>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="PageHeader" runat="server">
</asp:Content>

