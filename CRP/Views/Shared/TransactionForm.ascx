<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CRP.Core.Domain.Item>" %>

<% foreach(var qs in Model.QuestionSets.Where(a => a.TransactionLevel).OrderBy(a => a.Order)) {%>
    <fieldset>
        <legend><%= Html.Encode(qs.QuestionSet.Name) %></legend>
        
        <% foreach (var q in qs.QuestionSet.Questions) {%>
        
            <p>
            
                <%= Html.Encode(q.Name) %>
            
                <!-- Render the controls now -->
                <% switch(q.QuestionType.Name) { %>
                    <% case "Text Box" : %>
                        <input type="text" />
                    <% break; %>
                    <% case "Text Area" : %>
                        <textarea></textarea>
                    <% break; %>
                    <% case "Boolean" : %>
                        <input type="checkbox" />
                    <% break; %>
                    <% case "Radio Buttons" : %>
                        <% foreach(var o in q.Options) { %>
                            <input type="radio" value='<%= Html.Encode(o.Id) %>' /><%= Html.Encode(o.Name) %>
                        <% } %>
                    <% break; %>
                    <% case "Checkbox List" : %>
                        <% foreach(var o in q.Options) { %>
                            <input type="checkbox" value='<%= Html.Encode(o.Name) %>' />
                        <% } %>
                    <% break; %>
                    <% case "Drop Down" : %>
                        <select>
                        <% foreach(var o in q.Options) { %>
                            <option value='<%= Html.Encode(o.Id) %>'><%= Html.Encode(o.Name) %></option>
                        <% } %>     
                        </select>
                    <% break; %>
                    <% case "Date" : %>
                        <input type="text" />
                    <% break; %>
                <% }; %>
                
            </p>
        
        <% } %>
        
    </fieldset>
<% } %>
