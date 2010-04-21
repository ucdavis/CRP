<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<CRP.Core.Domain.Tag>>" %>
<%@ Import Namespace="CRP.Controllers"%>


    <div class="TagContainer">
    
        <strong>Tags:</strong>
    
        <% foreach(var s in Model.Select(a => a.Name).Distinct()) {%>
        
            <%= Html.ActionLink<TagController>(a => a.Index(s), s) %>
        
        <% } %>
    </div>