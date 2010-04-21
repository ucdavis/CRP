<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.ItemDetailViewModel>" %>
<%@ Import Namespace="CRP.Core.Resources"%>
<%@ Import Namespace="CRP.Controllers"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Details
</asp:Content>

<asp:Content ID="pageHeader" ContentPlaceHolderID="PageHeader" runat="server">
    <% Html.RenderPartial(StaticValues.View_PageHeader, Model.DisplayProfile ?? new DisplayProfile()); %>
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
        
        <% if (Model.Item.Expiration > DateTime.Now) { %>
        <p>
            <% if (Model.Item.IsAvailableForReg) { %>
                <a href='<%= Url.Action("Checkout", "Transaction", new {id=Model.Item.Id} ) %>'><img src="../../Images/register.png" style="border:0;" /></a>
            <% } %>
        </p>
        <% } %>
        
        <p><%= Model.Item.Description%></p>
        
        <p>
            <strong>Link:</strong>
            <%= Html.Encode(Model.Item.Link)%>
        </p>
        
        <iframe width="425" height="350" frameborder="0" scrolling="no" marginheight="0" 
            marginwidth="0" 
            src="<%= Model.Item.MapLink %>"></iframe><br />
        <small>
            <a href="<%= Model.Item.LinkLink %>" style="color:#0000FF;text-align:left">View Larger Map</a>
        </small>
        
    </fieldset>
    <p>
        <%= Html.ActionLink<ItemController>(a => a.List(), "Back to List") %>
    </p>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

