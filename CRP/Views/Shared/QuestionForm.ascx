<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CRP.Core.Domain.Question>" %>

    <p>
    
        <%= Html.Encode(Model.Name) %>
    
        <!-- Render the controls now -->
        <% switch(Model.QuestionType.Name) { %>
            <% case "Text Box" : %>
                <input type="text" id='<%= Html.Encode(Model.Id) %>' />
            <% break; %>
            <% case "Text Area" : %>
                <textarea  id='<%= Html.Encode(Model.Id) %>'></textarea>
            <% break; %>
            <% case "Boolean" : %>
                <input type="checkbox" id='<%= Html.Encode(Model.Id) %>' />
            <% break; %>
            <% case "Radio Buttons" : %>
                <% foreach(var o in Model.Options) { %>
                    <input type="radio"  id='<%= Html.Encode(Model.Id) %>' value='<%= Html.Encode(o.Id) %>' /><%= Html.Encode(o.Name) %>
                <% } %>
            <% break; %>
            <% case "Checkbox List" : %>
                <% foreach(var o in Model.Options) { %>
                    <input type="checkbox"  id='<%= Html.Encode(Model.Id) %>' value='<%= Html.Encode(o.Name) %>' />
                <% } %>
            <% break; %>
            <% case "Drop Down" : %>
                <select  id='<%= Html.Encode(Model.Id) %>'>
                <% foreach(var o in Model.Options) { %>
                    <option value='<%= Html.Encode(o.Id) %>'><%= Html.Encode(o.Name) %></option>
                <% } %>     
                </select>
            <% break; %>
            <% case "Date" : %>
                <input type="text" class="dateForm" id='<%= Html.Encode(Model.Id) %>' />
            <% break; %>
        <% }; %>
        
    </p>