<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Question>" %>

    <p>
    
        <%= Html.Encode(Model.Name) %>
    
        <!-- Render the controls now -->
        <% switch(Model.QuestionType.Name) { %>
            <% case "Text Box" : %>
                <%= Html.Hidden(".QuestionId", Model.Id, new { @class = "QA" })%>
                <%= Html.Hidden(".QuestionSetId", Model.QuestionSet.Id, new { @class = "QA" })%>
                <%= Html.TextBox(".Answer", string.Empty, new {@class="QA"}) %>
            <% break; %>
            <% case "Text Area" : %>
                <%= Html.Hidden(".QuestionId", Model.Id, new { @class = "QA" })%>
                <%= Html.Hidden(".QuestionSetId", Model.QuestionSet.Id, new { @class = "QA" })%>            
                <%= Html.TextArea(".Answer", string.Empty, new { @class = "QA" })%>
            <% break; %>
            <% case "Boolean" : %>
                <%= Html.Hidden(".QuestionId", Model.Id, new { @class = "QA" })%>
                <%= Html.Hidden(".QuestionSetId", Model.QuestionSet.Id, new { @class = "QA" })%>
                <%= Html.CheckBox(".Answer", false, new {@class="QA"})%>
            <% break; %>
            <% case "Radio Buttons" : %>               
                <%= Html.Hidden(".QuestionId", Model.Id, new { @class = "QA" })%>
                <%= Html.Hidden(".QuestionSetId", Model.QuestionSet.Id, new { @class = "QA" })%>
            
                <% foreach(var o in Model.Options) { %> 
                    <%= Html.RadioButton(".Answer", o.Name, new { @class = "QA" })%>
                    <%= Html.Encode(o.Name) %>
                <% } %>
            <% break; %>
            <% case "Checkbox List" : %>
                <%= Html.Hidden(".QuestionId", Model.Id, new { @class = "QA" })%>
                <%= Html.Hidden(".QuestionSetId", Model.QuestionSet.Id, new { @class = "QA" })%>
                
                <% foreach(var o in Model.Options) { %> 
                    <%= Html.CheckBox(".Answer", false, new { @class = "QA" })%>
                    <%= Html.Encode(o.Name) %>
                <% } %>
            <% break; %>
            <% case "Drop Down" : %>
                <%= Html.Hidden(".QuestionId", Model.Id, new { @class = "QA" })%>
                <%= Html.Hidden(".QuestionSetId", Model.QuestionSet.Id, new { @class = "QA" })%>
                <%= this.Select(".Answer").Options(Model.Options, x => x.Name, x => x.Name).Class("QA")
                        .FirstOption("--Select--") %>
            <% break; %>
            <% case "Date" : %>
                <%= Html.Hidden(".QuestionId", Model.Id, new { @class = "QA" })%>
                <%= Html.Hidden(".QuestionSetId", Model.QuestionSet.Id, new { @class = "QA" })%>
                <%= Html.TextBox(".Answer", null, new {@class = "dateForm QA"}) %>
            <% break; %>
        <% }; %>
        
    </p>