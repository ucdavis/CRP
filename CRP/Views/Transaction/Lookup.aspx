<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.LookupViewModel>" %>
<%@ Import Namespace="CRP.Controllers"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Lookup
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Lookup</h2>

    <% using (Html.BeginForm("Lookup", "Transaction", FormMethod.Post)) { %>
        <%= Html.AntiForgeryToken() %>
        <p>
            Order Number:
            <%= Html.TextBox("orderNumber", Model.TransactionNumber)%>
        </p>
        <p>
            Email:
            <%= Html.TextBox("email", Model.Email) %>
        </p>
        <p>
            <%= Html.SubmitButton("Submit", "Lookup Order") %>
        </p>
    <% } %>
    
    <p>If you logged in using an openid account (such as google or yahoo accounts) please login to view your order history.
        <%= Html.ActionLink<AccountController>(a => a.OpenIdAccount(), "Here") %>
    </p>

    <% if (Model.Transaction != null) { %>
    
        <p>
            Amount of Order:
            <%= Html.Encode(Model.Transaction.Total.ToString("C")) %>
        </p>
        <p>
            Amount Paid:
            <%= Html.Encode(Model.Transaction.TotalPaid.ToString("C")) %>
        </p>
        <p>
            Paid by:
            <%= Html.Encode(Model.Transaction.Credit ? "Credit Card" : "Check") %>
        </p>
    
    <% } %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
