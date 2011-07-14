<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CRP.Controllers.ViewModels.MapPinViewModel>" %>
   
    <script src="https://maps-api-ssl.google.com/maps/api/js?v=3&sensor=false" type="text/javascript"></script>
    <link href="<%= Url.Content("~/Content/jquery.gPositions.css") %>" rel="Stylesheet" type="text/css" />
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/jquery.gPositions.js") %>"></script>

    <script type="text/javascript">
        $(function () {
            $("#map").gPositions({ mode: MapMode.SELECTINGPOINT, showLocations: false });
        });
    </script>
    
    
    <%= Html.ValidationSummary("Create was unsuccessful. Please correct the errors and try again.") %>

    <% using (Html.BeginForm()) {%>
        <%= Html.AntiForgeryToken() %>
    
        <%= Html.ClientSideValidation<MapPin>("") %>
        <fieldset>
            <legend>Fields</legend>
            
            <div id="map">
                <div class="gp-map" style="height: 300px; width: 500px;"></div>

                <% if (Model != null && Model.MapPin != null && !string.IsNullOrWhiteSpace(Model.MapPin.Latitude) )
                   { %>
                    <div class="gp-coordinate gp-default" data-lat="<%= Model.MapPin.Latitude %>" data-lng="<%= Model.MapPin.Longitude %>">
                        <dt class="gp-name">n/a</dt>
                    </div>
                <% } %>

                
            </div>
                        
           <ul style="float:left; margin-top: 1em;">
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
            <li>
                <input type="submit" value="Save" />
            </li>
            </ul>
        </fieldset>

    <% } %>