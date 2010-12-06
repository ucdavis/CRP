<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<CRP.Core.Domain.Item>" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title><%= Html.Encode(Model.Name) + " - Map" %></title>
    
    <style type="text/css" media="print">
        * {font-family:Arial;}
        .coordinate-container, #MSVE_navAction_container {display:none;}
        .map MSVE_MapContainer {width:100%;}
    </style>

    <script src="https://www.google.com/jsapi" type="text/javascript"></script>
    <script type="text/javascript">
        google.load("jquery", "1.3.2");
        google.load("jqueryui", "1.8.2");
    </script>

    <link href="<%= Url.Content("~/Content/ui.BingMaps.css") %>" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="http://ecn.dev.virtualearth.net/mapcontrol/mapcontrol.ashx?v=6.2"></script>
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/ui.BingMaps.js") %>"></script>
        
    <script type="text/javascript">
        $(function() {
            $("#map").bingmaps({ enableRouting: false, displayCurrentLocation: false, height: "700px", width: "700px" });
        });        
    </script>

</head>
<body>

<h1><%=Html.Encode(Model.Name) %></h1>
    <div>
    
    <div id="map">
	    <div >
	    
	    <dl>
	        <% foreach(var a in Model.MapPins) { %>
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
<% foreach (var ep in Model.ExtendedPropertyAnswers.Where(a => a.Answer != string.Empty)){%>    
    <strong><%= Html.Encode(ep.ExtendedProperty.Name) %>:</strong>
    <%= Html.Encode(ep.ExtendedProperty.QuestionType.Name == "Text Box"
                                ? ep.Answer : Convert.ToDateTime(ep.Answer).ToString("D")) %>      
<% } %>
</p>
</body>
</html>
