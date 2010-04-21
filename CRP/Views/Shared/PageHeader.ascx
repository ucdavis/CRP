<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CRP.Core.Domain.DisplayProfile>" %>

    <% if (Model.Logo == null) { %> 
        <img src="../../Images/logo.gif" style="display:inline;" />
    <% } else { %>  
        <img src='<%= Url.Action("GetLogo", "DisplayProfile", new {id = Model.Id}) %>' style="display:inline;" />
    <% } %>

    <% if (String.IsNullOrEmpty(Model.Name)) { %>
        <!-- Display the page's default template -->    
        <h1 style="display:inline;">Conference Registration and Payments</h1>
    <% } else { %>
        <h1 style="display:inline;">
            <%= Html.Encode(Model.Name) %>
        </h1>    
    <% } %>

