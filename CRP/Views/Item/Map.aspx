<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<CRP.Core.Domain.Item>" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title><%= Html.Encode(Model.Name) + " - Map" %></title>
    
    <style type="text/css" media="screen">@import url(http://caes.ucdavis.edu/portal_css/Plone%20Default/base.css);</style>
    <style type="text/css" media="print">@import url(http://caes.ucdavis.edu/portal_css/Plone%20Default/print.css);</style>

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
            $("#map").bingmaps({ enableRouting: false, displayCurrentLocation: false, height: "450px", width: "450px" });
        });        
    </script>

</head>
<body>
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
</body>
</html>
