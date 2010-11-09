<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CRP.Core.Domain.Item>" %>
<%@ Import Namespace="CRP.Controllers" %>

    <link href="<%= Url.Content("~/Content/ui.BingMaps.css") %>" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="http://ecn.dev.virtualearth.net/mapcontrol/mapcontrol.ashx?v=6.2"></script>
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/ui.BingMaps.js") %>"></script>    
    <script type="text/javascript">
        $(function() {
        $("#map").bingmaps({ enableRouting: false,
                loadAllPins: true,
                displayCurrentLocation: false,
                hideCoordinates: true                
            });
        });        
    </script> 
    

<fieldset>
    
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

    <fieldset>
    <div id = "mapGrid">
        <p>
        <%= Html.ActionLink<MapPinController>(a => a.Create(Model.Id), "Add Map Pin") %>
    </p>
    <% Html.Grid(Model.MapPins.OrderByDescending(a => a.IsPrimary)) 
           .Transactional()
           .Name("MapPinLocations")
           .PrefixUrlParameters(false)
           .Columns(col =>
                        {
                            col.Template(a =>
                            {%>
                                <%= Html.ActionLink<MapPinController>(b=>b.Edit(Model.Id, a.Id), "Edit") %>|
                                <%= Html.ActionLink<MapPinController>(b=>b.RemoveMapPin(Model.Id,a.Id), "Remove") %>
                            <%});
                            col.Bound(a => a.Title);
                            col.Bound(a => a.Description);
                        })
           .Render();
           %>
   </div>
   </fieldset>
</fieldset>    