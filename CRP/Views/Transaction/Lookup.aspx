<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.LookupViewModel>" %>
<%@ Import Namespace="CRP.Controllers"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Lookup
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Lookup</h2>

    <% using (Html.BeginForm("Lookup", "Transaction", FormMethod.Post)) { %>
        <%= Html.AntiForgeryToken() %>
        <ul>
        <li>
            Order Number:
            <%= Html.TextBox("orderNumber", Model.TransactionNumber)%>
        </li>
        <li>
            Email:
            <%= Html.TextBox("email", Model.Email) %>
        </li>
        <li>
            <%= Html.SubmitButton("Submit", "Lookup Order") %>
        </li>
        </ul>
    <% } %>
    
    <ul>
    <li>If you logged in using an openid account (such as google or yahoo accounts) please login to view your order history.
        <%= Html.ActionLink<AccountController>(a => a.OpenIdAccount(), "Here") %>
    </li>
    </ul>
    <% if (Model.Transaction != null) { %>
        <ul>
        <li>
            Amount of Order:
            <%= Html.Encode(Model.Transaction.Total.ToString("C")) %>
        </li>
        <li>
            Amount Paid:
            <%= Html.Encode(Model.Transaction.TotalPaid.ToString("C")) %>
        </li>
        <li>
            Paid by:
            <%= Html.Encode(Model.Transaction.Credit ? "Credit Card" : "Check") %>
        </li>        
        <%if (Model.ShowCreditCardReSubmit) {%>
            <%= Html.ActionLink<TransactionController>(a => a.Confirmation(Model.Transaction.Id), "Resubmit Credit Card Payment") %>
        <%} %>
        </ul>
    <% } %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
