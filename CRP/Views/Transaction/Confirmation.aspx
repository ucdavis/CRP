<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.PaymentConfirmationViewModel>" %>
<%@ Import Namespace="CRP.Core.Resources"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Confirmation
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function() {

            // submit the form automatically
            $("form#PaymentForm").submit();

        });
        
        
    </script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="PageHeader" runat="server">
    <% Html.RenderPartial(StaticValues.Partial_PageHeader, Model.DisplayProfile ?? new DisplayProfile()); %>
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
            <%= Html.Encode(string.Format("{0:C}", Model.Transaction.Total))%>
        </p>
        
        <p>
            
            <!-- This form submits the payment information to uPay for processing -->
            <form method="post" action="<%= Model.PaymentGatewayUrl %>" id="PaymentForm">
                <%= Html.Hidden(StaticValues.Upay_TransactionId, Model.Transaction.Id) %>
                <%= Html.Hidden(StaticValues.Upay_Amount, Model.Transaction.Total) %>
                <%= Html.Hidden(StaticValues.Upay_SiteId, Model.SiteId) %>
                <%= Html.Hidden(StaticValues.Upay_ValidationKey, Model.ValidationKey) %>
                <%= Html.Hidden(StaticValues.Upay_SuccessLink, Model.SuccessLink) %>
                <%= Html.Hidden(StaticValues.Upay_CancelLink, Model.CancelLink ) %>
                <%= Html.Hidden(StaticValues.Upay_ErrorLink, Model.ErrorLink) %>
                <%= Html.SubmitButton("Submit", "Click here to be taken to our payment site.") %>
            </form>
        </p>
    
    <% } else { %>
    
        <p>        
           <%=Model.Transaction.Item.CheckPaymentInstructions %>
        </p>
    
    <% } %>
    

</asp:Content>




