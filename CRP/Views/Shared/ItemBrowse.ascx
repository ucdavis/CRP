<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IQueryable<CRP.Core.Domain.Item>>" %>
<%@ Import Namespace="CRP.Controllers"%>

   
    <table>
    
        <thead>

        </thead>
    
    <tbody>
    
        <%  // for loop to go through all the items passed
            foreach(var item in Model) { %>
            <tr>
                <td>
                    <a href='<%= Url.Action("Details", "Item", new {id = item.Id}) %>'>
                    
                        <h1><%= Html.Encode(item.Name) %></h1>
                        
                        <img src='<%= Url.Action("GetImage", "Item", new {id = item.Id}) %>' />
                        
                        <h3><%= Html.Encode(item.Expiration.HasValue ? item.Expiration.Value.ToString("d") : string.Empty) %></h3>                    
                        
                        <% Html.RenderPartial("~/Views/Shared/TagView.ascx", item.Tags); %>
                        
                        <p>
                            <%= item.Description %>
                        </p>
                    
                    </a>
                </td>
            </tr>
        <% } %>
    
    </tbody>
    
    </table>