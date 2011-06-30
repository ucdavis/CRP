<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.BigMapViewModel>" %>
<%@ Import Namespace="CRP.Controllers" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title><%= Html.Encode(Model.Item.Name) + " - Map" %></title>
    
    <%--<style type="text/css" media="print">
        * {font-family:Arial;}
        .coordinate-container, #MSVE_navAction_container, .pinToggle {display:none;}
        .map MSVE_MapContainer {width:100%;}
    </style>--%>

    <script src="https://www.google.com/jsapi" type="text/javascript"></script>
    <script type="text/javascript">
        google.load("jquery", "1.6.1");
        google.load("jqueryui", "1.8.2");
    </script>

    <%--<link href="<%= Url.Content("~/Content/ui.BingMaps.css") %>" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="http://ecn.dev.virtualearth.net/mapcontrol/mapcontrol.ashx?v=6.2"></script>
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/ui.BingMaps.js") %>"></script>
    <script src="<%= Url.Content("~/Scripts/jquery.bt.min.js") %>" type="text/javascript"></script>
    --%>
        
    <%--<script type="text/javascript">

        var icon = "<div style='background-color: #235087; border: 2px solid #FFFFFF; font-size: 12px; font-weight: bold; opacity: 0.7; padding: 0.5em 0; text-align: center; width: 100px;'>title</div>";
        var pins = '<%=Model.UsePins %>'.toLowerCase();
        var xpin = false;
        if (pins == 'true') {
            xpin = true;
        }
        
        $(function() {
            $("#map").bingmaps({ enableRouting: false, displayCurrentLocation: false, displaySearch: false
                               , loadAllPins: true, usePushPins: xpin, customShape: icon, allowShapeDragging: !xpin
                               , height: "700px", width: "700px"});
        });        
    </script>
    <script type="text/javascript">
        $(document).ready(function() {
            $('.coordinate-title').append('  <%=Html.Image("~/images/question_blue.png", new { @id = "MapPinHelp" })%>');
            <%if(Model.HasMapPins) {%>                        
                $("#MapPinHelp").bt("To view Locations on the map, click on the tabs below", {positions: 'top'});
            <% } else {%>
                $("#MapPinHelp").bt("No specific locations have been set for this map", {positions: 'top'});
            <%} %>
        });
    </script>--%>

    <script src="https://maps-api-ssl.google.com/maps/api/js?v=3&sensor=false" type="text/javascript"></script>
    <link href="<%= Url.Content("~/Content/jquery.gPositions.css") %>" rel="Stylesheet" type="text/css" />
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/jquery.gPositions.js") %>"></script>
    <script src="<%= Url.Content("~/Scripts/jquery.bt.min.js") %>" type="text/javascript"></script>

    <script type="text/javascript">

        $(function () {
            $("#map").gPositions({ helpIcon: '<%= Url.Content("~/Images/question_blue.png") %>' });
            $(".gp-sidecontainer-title img").bt("To view Locations on the map, click on the tabs below"); //, { positions: 'bottom' });
        });

    </script>

</head>
<body>

    <div id="map" style="margin: 0px 0px 20px;">
    
        <div class="gp-map" style="height: 700px; width: 700px;"></div>

        <% foreach (var a in Model.Item.MapPins) { %>
            <div class="gp-coordinate <%= a.IsPrimary ? "default-location" : string.Empty %>" data-lat="<%= a.Latitude %>" data-lng="<%= a.Longitude %>">
                <dt><%= Html.Encode(a.Title) %></dt>
                <% if (!string.IsNullOrWhiteSpace(a.Description)) { %>
                    <dd><%= Html.Encode(a.Description ?? string.Empty) %></dd>
                <% } %>
            </div>
        <% } %>

    </div>


<%--<h1><%=Html.Encode(Model.Item.Name) %></h1>
    <div class="pinToggle"><%= Html.ActionLink<ItemController>(a=>a.Map(Model.Item.Id, !Model.UsePins), "Toggle Pins-Labels") %></div>
    <div>
    
    <div id="map">
	    <div >
	    
	    <dl>
	        <% foreach(var a in Model.Item.MapPins) { %>
	            <div class="<%= a.IsPrimary ? "default-location" : string.Empty %>" lat="<%= a.Latitude %>" lng="<%= a.Longitude %>">
	                <dt><%= Html.Encode(a.Title) %></dt>
	                <% if (!string.IsNullOrEmpty(a.Description)) { %>
	                    <dd><%= Html.Encode(a.Description ?? string.Empty) %></dd>
	                <% } %>
	            </div>
	        <% } %>
	    </dl>
	    </div>
    </div>
        
    
    </div>
<p>
<% foreach (var ep in Model.Item.ExtendedPropertyAnswers.Where(a => a.Answer != string.Empty)){%>    
    <strong><%= Html.Encode(ep.ExtendedProperty.Name) %>:</strong>
    <%= Html.Encode(ep.ExtendedProperty.QuestionType.Name == "Text Box"
                                ? ep.Answer : Convert.ToDateTime(ep.Answer).ToString("D")) %>
                                <br />      
<% } %>
</p>--%>
</body>
</html>
