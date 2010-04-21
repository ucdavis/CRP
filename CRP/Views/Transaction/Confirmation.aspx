<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.PaymentConfirmationViewModel>" %>
<%@ Import Namespace="CRP.Core.Resources"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Confirmation
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">

</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="PageHeader" runat="server">
    <% Html.RenderPartial(StaticValues.View_PageHeader, Model.DisplayProfile ?? new DisplayProfile()); %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Confirmation</h2>

    <% if(Model.Transaction.Credit) { %>
    
        <p>
            <label>Item: </label>
            <%= Html.Encode(Model.Transaction.Item.Name) %>
        </p>
        <p>
            <label>Amount: </label>
            <%= Html.Encode(Model.Transaction.AmountTotal) %>
        </p>
        
        <p>
            
            <!-- This form submits the payment information to uPay for processing -->
            <form method="post" action="<%= Model.PaymentGatewayUrl %>">
                <%= Html.Hidden(StaticValues.Upay_TransactionId, Model.Transaction.Id) %>
                <%= Html.Hidden(StaticValues.Upay_Amount, Model.Transaction.Total) %>
                <%= Html.Hidden(StaticValues.Upay_SiteId, 0) %>
                
                <%= Html.SubmitButton("Submit", "Make Payment") %>
            </form>
        </p>
    
    <% } else { %>
    
        <p>
            Thank you for your purchase!
            
            Please mail your payment to:
            
                John Zoidberg
                150 Mrak Hall
                One Shields Ave.
                Davis, CA 94534
        </p>
    
    <% } %>
    

</asp:Content>




