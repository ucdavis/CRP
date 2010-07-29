<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IQueryable<CRP.Core.Domain.Item>>" %>
<%@ Import Namespace="CRP.Core.Resources"%>
<%@ Import Namespace="CRP.Controllers"%>

   
    <table class="itembrowsetable">
    
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
                        
                        <h3>Last day to register online: <%= Html.Encode(item.Expiration.HasValue ? item.Expiration.Value.ToString("D") : string.Empty) %></h3>                    
                        
                        <% Html.RenderPartial(StaticValues.Partial_TagView, item.Tags); %>

                        <a href='<%= Url.Action("Details", "Item", new {id = item.Id}) %>'>
                            <%= Html.Encode(item.Summary) %>
                            <%--<%= item.Description.Length > 1000 ? Html.HtmlEncode(item.Description.Substring(0, 1000)) : Html.HtmlEncode(item.Description) %>--%>
                        </a>
                        
                        <div><%Item item1 = item;%><%= Html.ActionLink<ItemController>(a => a.Details(item1.Id), "Click here to register.", new{style="color: rgb(0, 0, 255)"}) %></div>
                    
                    </a>
                </td>
            </tr>
        <% } %>
    
    </tbody>
    
    </table>