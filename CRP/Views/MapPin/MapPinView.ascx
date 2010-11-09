<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CRP.Controllers.ViewModels.MapPinViewModel>" %>

    <link href="<%= Url.Content("~/Content/ui.BingMaps.css") %>" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="http://ecn.dev.virtualearth.net/mapcontrol/mapcontrol.ashx?v=6.2"></script>
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/ui.BingMaps.js") %>"></script>    
    <script type="text/javascript">
        $(function() {
        $("#map").bingmaps({ enableRouting: false
            , displayCurrentLocation: true
            , crosshairLocation: '<%= Url.Content("~/Images/crosshair.gif") %>'
            , displayLongitudeControl : $("#Longitude")
            , displayLatitudeControl: $("#Latitude")
            , defaultLat: <%= Model.MapPin.Id > 0 ? Model.MapPin.Latitude : (38.539438402158495).ToString() %>
            , defaultLng: <%= Model.MapPin.Id > 0 ? Model.MapPin.Longitude : (-121.75701141357422).ToString() %>
            , defaultZoom : <%= Model.MapPin.Id > 0 ? 16 : 14 %>
            , displaySearch: true
            });
        });        
    </script> 
    
    
    
    <%= Html.ValidationSummary("Create was unsuccessful. Please correct the errors and try again.") %>

    <% using (Html.BeginForm()) {%>
        <%= Html.AntiForgeryToken() %>
    
        <%= Html.ClientSideValidation<MapPin>("") %>
        <fieldset>
            <legend>Fields</legend>
            
            <div id="map">
            </div>
            <%--<input id="Latitude" type="hidden" name="Latitude" value="<%= Model != null && Model.MapPin != null ? Model.MapPin.Latitude : string.Empty %>" />
            <input id="Longitude" type="hidden" name="Longitude" value="<%= Model != null && Model.MapPin != null ? Model.MapPin.Longitude : string.Empty %>" />--%>
            <%= Html.Hidden("Latitude", Model != null && Model.MapPin != null ? Model.MapPin.Latitude : string.Empty) %>
            <%= Html.Hidden("Longitude", Model != null && Model.MapPin != null ? Model.MapPin.Longitude : string.Empty)%>
            
           <ul>
            <li>
                <label for="Title">Title:</label>
                <%= Html.TextBox("Title", Model != null && Model.MapPin != null ? Model.MapPin.Title : string.Empty) %>
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