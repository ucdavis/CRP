<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.ItemDetailViewModel>" %>
<%@ Import Namespace="CRP.Core.Resources"%>
<%@ Import Namespace="CRP.Controllers"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Details
</asp:Content>

<asp:Content ID="pageHeader" ContentPlaceHolderID="PageHeader" runat="server">
    <% Html.RenderPartial(StaticValues.Partial_PageHeader, Model.DisplayProfile ?? new DisplayProfile()); %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h1><%= Html.Encode(Model.Item.Name)%></h1>

    <% Html.RenderPartial(StaticValues.Partial_TagView, Model.Item.Tags); %>

    <fieldset class="details">
        <!-- <legend>Details</legend> -->
        

        
        <ul>
        
        <li class="two_col_float_uneven_left">   
        <p>
            <%= Html.HtmlEncode(Model.Item.Description) %>
        </p>
        
        <!-- Only display link if one has been provided -->
        <% if (!string.IsNullOrEmpty(Model.Item.Link)) { %>
        <p>
            <strong>Link:</strong>
            <a href="<%= Html.Encode(Model.Item.Link)%>"><%= Html.Encode(Model.Item.Link) %></a>
        </p>
        <% } %>

        <!-- Only display the map if there are one or more map pins -->
        <%-- Bing Maps Control, Replaced below with Google Maps
            
        <% if (Model.HasMapPins) { %>
        <%= Html.ActionLink<ItemController>(a=>a.Map(Model.Item.Id, true), "Full Screen Map") %>
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
        <% } %>--%>

        <% if (Model.HasMapPins) { %>
            <%= Html.ActionLink<ItemController>(a=>a.Map(Model.Item.Id, true), "Full Screen Map", new {target="_blank"}) %> &nbsp;&nbsp;&nbsp;
            <%= Html.ActionLink<ItemController>(a => a.MapDirections(Model.Item.Id, true), "Full Screen Map With Directions", new { target = "_blank" })%>

            <div id="map" style="margin: 0px 0px 20px;">
            
                <div class="gp-map" style="height: 450px; width: 450px;"></div>

                <% foreach (var a in Model.Item.MapPins) { %>
                    <div class="gp-coordinate <%= a.IsPrimary ? "gp-default" : string.Empty %>" data-lat="<%= a.Latitude %>" data-lng="<%= a.Longitude %>">
                        <dt class="gp-name"><%= Html.Encode(a.Title) %></dt>
                        <% if (!string.IsNullOrWhiteSpace(a.Description)) { %>
                            <dd class="gp-description"><%= Html.Encode(a.Description ?? string.Empty) %></dd>
                        <% } %>
                    </div>
                <% } %>

            </div>

        <% } %>


        </li>
        
        <li class="two_col_float_uneven_right">
        <ul>
        <li><span class="itemdetailsimg">
            <img src='<%= Url.Action("GetImage", "Item", new {id = Model.Item.Id}) %>' />
        </span>
        </li>
        <li>
                <% if (Model.Item.Expiration >= DateTime.Now.Date) { %>
        <!-- <p>
            <% if (Model.Item.IsAvailableForReg) { %>
                <a href='<%= Url.Action("Checkout", "Transaction", new {id=Model.Item.Id} ) %>'><img src="<%= Url.Content("~/Images/register.png") %>" style="border:0;" /></a>
            <% } %>
        </p> -->
        
        <a href='<%= Url.Action("Checkout", "Transaction", new {id=Model.Item.Id} ) %>' class="reg_btn"><h3>Click Here To Register</h3></a>
        </li>
        <% } %>
        <% foreach (var ep in Model.Item.ExtendedPropertyAnswers.Where(a => a.Answer != string.Empty)){%>    
            <li>
                <strong><%= Html.Encode(ep.ExtendedProperty.Name) %>:</strong>
                <%= Html.Encode(ep.ExtendedProperty.QuestionType.Name == "Text Box"
                                            ? ep.Answer : Convert.ToDateTime(ep.Answer).ToString("D")) %>
            </li>        
        <% } %>
        <li>        
            <strong>Price Per <%=Html.Encode(Model.Item.QuantityName) %>:</strong>
            <%= Html.Encode(String.Format("{0:C}", Model.Item.CostPerItem))%>
        </li>
        <li>
            <strong>Last day to register online:</strong>
            <%= Html.Encode(String.Format("{0:D}", Model.Item.Expiration))%>
        </li>
        </ul>
        </li>
        
        
    </fieldset>
    <p>
        <%= Html.ActionLink<HomeController>(a => a.Index(), "Back to List") %>
    </p>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
    <%--<link href="<%= Url.Content("~/Content/ui.BingMaps.css") %>" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="http://ecn.dev.virtualearth.net/mapcontrol/mapcontrol.ashx?v=6.2"></script>
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/ui.BingMaps.js") %>"></script>--%>
        
    <!-- Only display the map if there are one or more map pins -->
<%--    <% if (Model.HasMapPins) { %>
    <script type="text/javascript">
        $(function() {
            $("#map").bingmaps({ enableRouting: false, displayCurrentLocation: false, height:"450px", width:"450px" });

            $('.coordinate-title').append('  <%=Html.Image("~/images/question_blue.png", new { @id = "MapPinHelp" })%>');
            <%if(Model.HasMapPins) {%>                        
                $("#MapPinHelp").bt("To view Locations on the map, click on the tabs below", {positions: 'top'});
            <% } else {%>
                $("#MapPinHelp").bt("No specific locations have been set for this map", {positions: 'top'});
            <%} %>
        });
    </script>
    <% } %>--%>

    <% if (Model.HasMapPins) { %>

        <script src="https://maps-api-ssl.google.com/maps/api/js?v=3&sensor=false" type="text/javascript"></script>
        <link href="<%= Url.Content("~/Content/jquery.gPositions.css") %>" rel="Stylesheet" type="text/css" />
        <script type="text/javascript" src="<%= Url.Content("~/Scripts/jquery.gPositions.js") %>"></script>

        <script type="text/javascript">
            $(function () {
                $("#map").gPositions({ helpIcon: '<%= Url.Content("~/Images/question_blue.png") %>' });
                $(".gp-sidecontainer-title img").attr("title", "");
                $(".gp-sidecontainer-title img").bt("To view Locations on the map, click on the tabs below", { positions: 'top' });
            });
        </script>
    <% } %>

    <!-- Should a header color have been specified, then throw it in -->
    <% if (Model.DisplayProfile != null && !string.IsNullOrEmpty(Model.DisplayProfile.CustomCss)) { %>
        <style type="text/css">
            <%= Model.DisplayProfile.CustomCss %>
        </style>
    <% } %>
</asp:Content>

