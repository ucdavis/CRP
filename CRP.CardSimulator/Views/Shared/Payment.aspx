<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.CardSimulator.Controllers.PaymentViewModel>" %>
<%@ Import Namespace="CRP.CardSimulator.Controllers"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Payment
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Touch Net Simulator</h2>

    <fieldset>
        <legend>Fields</legend>
        <p>
            UpaySiteId:
            <%= Html.Encode(Model.UpaySiteId) %>
        </p>
        <p>
            TransactionId:
            <%= Html.Encode(Model.TransactionId) %>
        </p>
        <p>
            Amount:
            <%= Html.Encode(String.Format("{0:F}", Model.Amount)) %>
        </p>
        <p>
            BillName:
            <%= Html.Encode(Model.BillName) %>
        </p>
        <p>
            BillEmailAddress:
            <%= Html.Encode(Model.BillEmailAddress) %>
        </p>
        <p>
            BillStreet1:
            <%= Html.Encode(Model.BillStreet1) %>
        </p>
        <p>
            BillStreet2:
            <%= Html.Encode(Model.BillStreet2) %>
        </p>
        <p>
            BillCity:
            <%= Html.Encode(Model.BillCity) %>
        </p>
        <p>
            BillState:
            <%= Html.Encode(Model.BillState) %>
        </p>
        <p>
            BillPostalCode:
            <%= Html.Encode(Model.BillPostalCode) %>
        </p>
        <p>
            BillCountry:
            <%= Html.Encode(Model.BillCountry) %>
        </p>
        <p>
            ValidationKey:
            <%= Html.Encode(Model.ValidationKey) %>
        </p>
    </fieldset>

    
    <form method="post" action="<%= ConfigurationManager.AppSettings["ReturnUrl"] %>">
    
        <%= Html.Hidden("PMT_STATUS", "success")%>
        <%= Html.Hidden("EXT_TRANS_ID", Model.TransactionId)%>
        <%= Html.Hidden("PMT_AMT", Model.Amount)%>
        <%= Html.Hidden("TPG_TRANS_ID", "123456")%>
        <%= Html.Hidden("UPAY_SITE_ID", "0")%>
        <input type="submit" value="Simulate Successful Payment" />
        
    </form>
    
    <form method="post" action="<%= ConfigurationManager.AppSettings["ReturnUrl"] %>">
    
        <%= Html.Hidden("PMT_STATUS", "cancelled")%>
        <%= Html.Hidden("EXT_TRANS_ID", Model.TransactionId)%>
        <%= Html.Hidden("PMT_AMT", Model.Amount)%>
        <input type="submit" value="Simulate Unsuccessful Payment" />

    </form>

</asp:Content>

