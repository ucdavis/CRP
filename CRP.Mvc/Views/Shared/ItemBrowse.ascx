<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<CRP.Core.Domain.Item>>" %>
<%@ Import Namespace="CRP.Core.Resources"%>
<%@ Import Namespace="CRP.Controllers"%>

   
    <table class="itembrowsetable">
    
        <thead>

        </thead>
    
    <tbody>
    
        <% if (Model == null || Model.Count() <= 0){%>
            <h1>There are currently no events to browse</h1>
        <%}%>
    
        <%  // for loop to go through all the items passed
            foreach(var item in Model) { %>
            <tr>
                <td>
                    <a href='<%= Url.Action("Details", "Item", new {id = item.Id}) %>'>
                    
                        <h1><%= Html.Encode(item.Name) %></h1>
                        
                        <img src='<%= Url.Action("GetImage", "Item", new {id = item.Id}) %>' />
                        <% foreach (var ep in item.ExtendedPropertyAnswers.Where(a => a.Answer != string.Empty)){%>    
                            <strong>
                                <%= Html.Encode(ep.ExtendedProperty.Name) %>:
                            </strong> 
                                <%= Html.Encode(ep.ExtendedProperty.QuestionType.Name == "Text Box"
                                                            ? ep.Answer : Convert.ToDateTime(ep.Answer).ToString("D")) %> <br/>
                                                            <%--<%= Html.Encode(ep.ExtendedProperty.QuestionType.Name == "Text Box"
                                                            ? ep.Answer : String.Format("{0:D}", ep.Answer)) %> <br/>--%>
                                   
                        <% } %>    
                        <strong>        
                            Price Per <%=Html.Encode(item.QuantityName) %>:
                        </strong>  
                            <%= Html.Encode(String.Format("{0:C}", item.CostPerItem))%>
                        <br/>                  
                        <strong>Last day to register online:</strong> <%= Html.Encode(item.Expiration.HasValue ? item.Expiration.Value.ToString("D") : string.Empty) %>                    
                        <br/>
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