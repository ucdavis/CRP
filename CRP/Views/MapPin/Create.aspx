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
            
            <div id="map">
            </div>
            
            <ul>
            <li>
                <label for="Latitude">Latitude:</label>
                <%= Html.Hidden("Latitude") %>
                <%= Html.ValidationMessage("MapPin.Latitude")%>
            </li>
            <li>
                <label for="Longitude">Longitude:</label>
                <%= Html.Hidden("Longitude") %>
                <%= Html.ValidationMessage("MapPin.Longitude")%>
            </li>
            <li>
                <label for="Title">Title:</label>
                <%= Html.TextBox("Title") %>
                <%= Html.ValidationMessage("MapPinTitle") %>
            </li>
            <li>
                <label for="Description">Description:</label>
                <%= Html.TextArea("Description") %>
                <%= Html.ValidationMessage("MapPinDescription")%>
            </li>
            </ul>
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

    <script type="text/javascript" src="<%= Url.Content("~/Scripts/jquery.bingmaps.js") %>"></script>
    <script type="text/javascript" src="http://ecn.dev.virtualearth.net/mapcontrol/mapcontrol.ashx?v=6.2"></script>
    
    <script type="text/javascript">
        $(document).ready(function() {
        $("#map").bingMaps({ enableRouting: false
            , displayCurrentLocation: true
            , crosshairLocation: '<%= Url.Content("~/Images/crosshair.gif") %>'
            , displayLongitudeControl : "Latitude"
            , displayLatitudeControl: "Longitude"  
            });
        });        
    </script>
    
    <link href="<%= Url.Content("~/Content/jquerymap.css") %>" rel="stylesheet" type="text/css" />

</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="PageHeader" runat="server">
    <% Html.RenderPartial(StaticValues.Partial_PageHeader, new DisplayProfile()); %>
</asp:Content>

