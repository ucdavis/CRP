<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.EditTransactionViewModel>" %>
<%@ Import Namespace="CRP.Controllers"%>
<%@ Import Namespace="CRP.Controllers.Helpers" %>
<%@ Import Namespace="CRP.Core.Resources" %>
<%@ Import Namespace="System.Security.Policy" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Refund
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Refund Details</h2>


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
                    Refund Amount:
                    <%= Html.Encode(Model.RefundAmount.ToString("C"))%>
                </li>
                <li>
                    Refund Date:
                    <%= Html.Encode(Model.CreateDate.ToString())%>
                </li>
                <li>
                    Refund Created By:
                    <%= Html.Encode(Model.CreatedBy)%>
                </li>
                <li>
                    Refund Reason:
                    <%= Html.Encode(Model.CorrectionReason)%>
                </li>
                
            </ul>
        </fieldset>

    <div>
        <%= Url.DetailItemLink(Model.TransactionValue.Item.Id, StaticValues.Tab_Refunds, Model.Sort, Model.Page)%>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="PageHeader" runat="server">
</asp:Content>

