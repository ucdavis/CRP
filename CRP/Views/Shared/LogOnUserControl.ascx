<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="CRP.Controllers.Helpers"%>
<%@ Import Namespace="CRP.Controllers"%>

<% if (Request.IsAuthenticated) {%>
     
    <% if (!Request.IsOpenId()) { %>
        <%= Html.Encode(Page.User.Identity.Name) %>
    <% } %>
     <%= Html.ActionLink<AccountController>(a => a.LogOut(), "Logout") %>
     
<% } else { %>
    <%= Html.ActionLink<AccountController>(a => a.LogOn(HttpContext.Current.Request.RawUrl, true), "Login") %>
<% } %>
