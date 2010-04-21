<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.ItemDetailViewModel>" %>
<%@ Import Namespace="CRP.Controllers"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Details
</asp:Content>

<asp:Content ID="pageHeader" ContentPlaceHolderID="PageHeader" runat="server">
    <% Html.RenderPartial("~/Views/Shared/PageHeader.ascx", Model.DisplayProfile); %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2><%= Html.Encode(Model.Item.Name)%></h2>

    <% Html.RenderPartial("~/Views/Shared/TagView.ascx", Model.Item.Tags); %>

    <fieldset>
        <legend>Details</legend>
        
        <span style="float:right;">
            <img src='<%= Url.Action("GetImage", "Item", new {id = Model.Item.Id}) %>' />
        </span>
        
        <p>
            <strong>CostPerItem:</strong>
            <%= Html.Encode(String.Format("{0:C}", Model.Item.CostPerItem))%>
        </p>
        <p>
            <strong>Expiration:</strong>
            <%= Html.Encode(String.Format("{0:d}", Model.Item.Expiration))%>
        </p>

        <% foreach (var ep in Model.Item.ExtendedPropertyAnswers)
           {%>
        
            <p>
                <strong><%= Html.Encode(ep.ExtendedProperty.Name) %>:</strong>
                <%= Html.Encode(ep.ExtendedProperty.QuestionType.Name == "Text Box"
                                            ? ep.Answer : String.Format("{0:d}", ep.Answer)) %>
            </p>
        
        <% } %>
        
        <p>
        <a href='<%= Url.Action("Checkout", "Transaction", new {id=Model.Item.Id} ) %>'><img src="../../Images/register.png" style="border:0;" /></a>
        </p>
        
        <p><%= Model.Item.Description%></p>
        
        <p>
            <strong>Link:</strong>
            <%= Html.Encode(Model.Item.Link)%>
        </p>
        
    </fieldset>
    <p>
        <%= Html.ActionLink<ItemController>(a => a.List(), "Back to List") %>
    </p>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
