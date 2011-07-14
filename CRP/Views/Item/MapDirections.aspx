<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.BigMapViewModel>" %>
<%@ Import Namespace="CRP.Controllers" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title><%= Html.Encode(Model.Item.Name) + " - Map" %></title>

    <script src="https://www.google.com/jsapi" type="text/javascript"></script>
    <script type="text/javascript">
        google.load("jquery", "1.6.1");
        google.load("jqueryui", "1.8.2");
    </script>

    <script src="https://maps-api-ssl.google.com/maps/api/js?v=3&sensor=false" type="text/javascript"></script>
    <link href="<%= Url.Content("~/Content/jquery.gPositions.css") %>" rel="Stylesheet" type="text/css" />
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/jquery.gPositions.js") %>"></script>
    <script src="<%= Url.Content("~/Scripts/jquery.bt.min.js") %>" type="text/javascript"></script>

    <script type="text/javascript">

        $(function () {
            $("#map").gPositions({ helpIcon: '<%= Url.Content("~/Images/question_blue.png") %>', mode: MapMode.ROUTING, displayDirections: true });
            $(".gp-sidecontainer-title img").attr("title", "");
            $(".gp-sidecontainer-title img").bt("To view Locations/Directions on the map, click on the tabs below"); //, { positions: 'bottom' });
        });

    </script>

</head>
<body>
    <%= Html.ActionLink<ItemController>(a=>a.Map(Model.Item.Id, true), "Full Screen Map Just Pins") %>
    <div id="map" style="margin: 0px 0px 20px;">
    
        <div class="gp-map" style="height: 700px; width: 700px;"></div>

        <% foreach (var a in Model.Item.MapPins) { %>
            <div class="gp-coordinate <%= a.IsPrimary ? "default-location" : string.Empty %>" data-lat="<%= a.Latitude %>" data-lng="<%= a.Longitude %>">
                <dt class="gp-name"><%= Html.Encode(a.Title) %></dt>
                <% if (!string.IsNullOrWhiteSpace(a.Description)) { %>
                    <dd class="gp-description"><%= Html.Encode(a.Description ?? string.Empty) %></dd>
                <% } %>
            </div>
        <% } %>

    </div>


    
</body>
</html>
