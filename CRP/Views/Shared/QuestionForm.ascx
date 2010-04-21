<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Question>" %>

    <p>
    
        <%= Html.Encode(Model.Name) %>
    
        <!-- Render the controls now -->
        <% switch(Model.QuestionType.Name) { %>
            <% case "Text Box" : %>
                <%= Html.Hidden(".QuestionId", Model.Id, new { @class = "indexedControl" })%>
                <%= Html.Hidden(".QuestionSetId", Model.QuestionSet.Id, new { @class = "indexedControl" })%>
                <%= Html.TextBox(".Answer", string.Empty, new {@class="indexedControl"}) %>
            <% break; %>
            <% case "Text Area" : %>
                <%= Html.Hidden(".QuestionId", Model.Id, new { @class = "indexedControl" })%>
                <%= Html.Hidden(".QuestionSetId", Model.QuestionSet.Id, new { @class = "indexedControl" })%>            
                <%= Html.TextArea(".Answer", string.Empty, new { @class = "indexedControl" })%>
            <% break; %>
            <% case "Boolean" : %>
                <%= Html.Hidden(".QuestionId", Model.Id, new { @class = "indexedControl" })%>
                <%= Html.Hidden(".QuestionSetId", Model.QuestionSet.Id, new { @class = "indexedControl" })%>
                <%= Html.CheckBox(".Answer", false, new {@class="indexedControl"})%>
            <% break; %>
            <% case "Radio Buttons" : %>               
                <%= Html.Hidden(".QuestionId", Model.Id, new { @class = "indexedControl" })%>
                <%= Html.Hidden(".QuestionSetId", Model.QuestionSet.Id, new { @class = "indexedControl" })%>
            
                <% foreach(var o in Model.Options) { %> 
                    <%= Html.RadioButton(".Answer", o.Name, new { @class = "indexedControl" })%>
                    <%= Html.Encode(o.Name) %>
                <% } %>
            <% break; %>
            <% case "Checkbox List" : %>
                <%= Html.Hidden(".QuestionId", Model.Id, new { @class = "indexedControl" })%>
                <%= Html.Hidden(".QuestionSetId", Model.QuestionSet.Id, new { @class = "indexedControl" })%>
                
                <% foreach(var o in Model.Options) { %> 
                    <%= Html.CheckBox(".Answer", false, new { @class = "indexedControl" })%>
                    <%= Html.Encode(o.Name) %>
                <% } %>
            <% break; %>
            <% case "Drop Down" : %>
                <%= Html.Hidden(".QuestionId", Model.Id, new { @class = "indexedControl" })%>
                <%= Html.Hidden(".QuestionSetId", Model.QuestionSet.Id, new { @class = "indexedControl" })%>
                <%= this.Select(".Answer").Options(Model.Options, x => x.Name, x => x.Name).Class("indexedControl")
                        .FirstOption("--Select--") %>
            <% break; %>
            <% case "Date" : %>
                <%= Html.Hidden(".QuestionId", Model.Id, new { @class = "indexedControl" })%>
                <%= Html.Hidden(".QuestionSetId", Model.QuestionSet.Id, new { @class = "indexedControl" })%>
                <%= Html.TextBox(".Answer", null, new {@class = "dateForm indexedControl"}) %>
            <% break; %>
        <% }; %>
        
    </p>