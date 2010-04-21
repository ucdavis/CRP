<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="CRP.Controllers.Helpers"%>
<%@ Import Namespace="CRP.Controllers"%>



<% if (Request.IsAuthenticated) {%>
     <li>
    <% if (!Request.IsOpenId()) { %>
        <%= Html.Encode(Page.User.Identity.Name) %>
    <% } %>
    
    <% if (Request.IsOpenId()) { %> <%= Html.ActionLink<AccountController>(a => a.OpenIdAccount(), "My Account") %> <% } %>
    
     [<%= Html.ActionLink<AccountController>(a => a.LogOut(), "Logout") %>]
     </li>
<% } %>
<% else { %>
    <li><%= Html.ActionLink<AccountController>(a => a.OpenIdAccount(), "My Account") %> </li>
<% } %>
