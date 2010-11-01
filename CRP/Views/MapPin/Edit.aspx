<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.MapPinViewModel>" %>
<%@ Import Namespace="CRP.Core.Resources" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Edit
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Edit</h2>

    <%= Html.ValidationSummary("Edit was unsuccessful. Please correct the errors and try again.") %>

    <% using (Html.BeginForm()) {%>
        <%= Html.AntiForgeryToken() %>
    
        <%= Html.ClientSideValidation<MapPin>("") %>
        <fieldset>
            <legend>Fields</legend>
            <ul>
            <li>
                <label for="Latitude">Latitude:</label>
                <%= Html.TextBox("Latitude", Model != null && Model.MapPin != null ? Model.MapPin.Latitude : string.Empty)%>
                <%= Html.ValidationMessage("MapPin.Latitude")%>
            </li>
            <li>
                <label for="Longitude">Longitude:</label>
                <%= Html.TextBox("Longitude", Model != null && Model.MapPin != null ? Model.MapPin.Longitude : string.Empty)%>
                <%= Html.ValidationMessage("MapPin.Longitude")%>
            </li>
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

    <div>
       <%= Url.EditItemLink(Model.Item.Id, StaticValues.Tab_MapPins) %>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="PageHeader" runat="server">
    <% Html.RenderPartial(StaticValues.Partial_PageHeader, new DisplayProfile()); %>
</asp:Content>

