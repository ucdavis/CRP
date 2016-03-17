<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CRP.Core.Domain.DisplayProfile>" %>


    <a href="<%= Url.Action("Index", "Home") %>">

    <% if (Model.Logo == null) { %> 
        <img class="brand_bar_caes_img" src="<%= Url.Content("~/Images/caes-logo.jpg") %>" alt="The College of Agricultural and Environmental Sciences" />
    <% } else { %>  
        <img src='<%= Url.Action("GetLogo", "DisplayProfile", new {id = Model.Id}) %>' class="brand_bar_caes_img" alt="The College of Agricultural and Environmental Sciences" />
    <% } %>
    
    </a>

<%--    <% if (String.IsNullOrEmpty(Model.Name)) { %>
        <!-- Display the page's default template -->    
        <h1 style="display:inline;">Conference Registration and Payments</h1>
    <% } else { %>
        <h1 style="display:inline;">
            <%= Html.Encode(Model.Name) %>
        </h1>    
    <% } %>--%>

