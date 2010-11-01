<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.MapPinViewModel>" %>
<%@ Import Namespace="CRP.Core.Resources" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Create
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Create</h2>

    <%= Html.ValidationSummary("Create was unsuccessful. Please correct the errors and try again.") %>

    <% using (Html.BeginForm()) {%>
        <%= Html.AntiForgeryToken() %>
    
        <%= Html.ClientSideValidation<MapPin>("") %>
        <fieldset>
            <legend>Fields</legend>
            <p>
                <label for="Latitude">Latitude:</label>
                <%= Html.TextBox("Latitude") %>
                <%= Html.ValidationMessage("MapPin.Latitude")%>
            </p>
            <p>
                <label for="Longitude">Longitude:</label>
                <%= Html.TextBox("Longitude") %>
                <%= Html.ValidationMessage("MapPin.Longitude")%>
            </p>
            <p>
                <label for="Title">Title:</label>
                <%= Html.TextBox("Title") %>
                <%= Html.ValidationMessage("MapPinTitle") %>
            </p>
            <p>
                <label for="Description">Description:</label>
                <%= Html.TextArea("Description") %>
                <%= Html.ValidationMessage("MapPinDescription")%>
            </p>

            <p>
                <input type="submit" value="Create" />
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

