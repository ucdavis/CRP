<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IQueryable<CRP.Core.Domain.Item>>" %>
<%@ Import Namespace="CRP.Controllers"%>


<%--<% foreach (var item in Model)
        {%>

        <div class=" ui-widget-content ui-corner-all" style="padding: 5px; margin-top: 10px; margin-bottom:10px;">
            <div class="ui-widget-header ui-corner-all" style="padding:10px;">
                
                <%= Html.ActionLink<ItemController>(a => a.Details(item.Id), item.Name)%>
                
                <span style="float:right;">
                    <% Html.RenderPartial("~/Views/Shared/TagView.ascx", item.Tags); %>
                </span>
                
            </div>
            <div >
            <p>
                <img src='<%= Url.Action("GetImage", "Item", new {id = item.Id}) %>' />
            </p>
            <p>
                <%= item.Description%>
            </p>
            
            </div>
        </div>

    <% }%>--%>
    
   
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
                        
                        <h3><%= Html.Encode(item.Expiration) %></h3>                    
                        
                        <% Html.RenderPartial("~/Views/Shared/TagView.ascx", item.Tags); %>
                        
                        <p>
                            <%= item.Description %>
                        </p>
                    
                        <%= Html.TextBox(item.Name) %>
                    
                    </a>
                </td>
            </tr>
        <% } %>
    
    </tbody>
    
    </table>