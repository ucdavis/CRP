<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.MapPinViewModel>" %>
<%@ Import Namespace="CRP.Core.Resources" %>
<%@ Import Namespace="CRP.Controllers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Edit
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Edit</h2>

    <% Html.RenderPartial("MapPinView"); %>

    <%--<%= Html.ValidationSummary("Edit was unsuccessful. Please correct the errors and try again.") %>

    <% using (Html.BeginForm()) {%>
        <%= Html.AntiForgeryToken() %>
    
        <%= Html.ClientSideValidation<MapPin>("") %>
        <fieldset>
            <legend>Fields</legend>
            <ul>
            <li>
                <label for="Title">Title:</label>
                <%= Html.TextBox("Title", Model != null && Model.MapPin != null ? Model.MapPin.Title : string.Empty)%>
                <%= Html.ValidationMessage("MapPinTitle") %>
            </li>
            <li>
                <label for="Description">Description:</label>
                <%= Html.TextArea("Description", Model != null && Model.MapPin != null ? Model.MapPin.Description : string.Empty)%>
                <%= Html.ValidationMessage("MapPinDescription")%>
            </li>
            </ul>
            <p>
                <input type="submit" value="Save" />
            </p>
        </fieldset>

    <% } %>
--%>
    <div>
       <%=Html.ActionLink<ItemManagementController>(a => a.Map(Model.Item.Id), "Back to Map") %>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="PageHeader" runat="server">
    <% Html.RenderPartial(StaticValues.Partial_PageHeader, new DisplayProfile()); %>
</asp:Content>

