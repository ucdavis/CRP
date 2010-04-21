<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ItemQuestionViewModel>" %>
<%@ Import Namespace="CRP.Controllers.ViewModels"%>
<%@ Import Namespace="CRP.Core.Resources"%>

    <p>
    
        <%= Html.Encode(Model.Question.Name) %>
        <%= Html.Hidden(".QuestionId", Model.Question.Id, new { @class = StaticValues.Class_indexedControl })%>
        <%= Html.Hidden(".QuestionSetId", Model.Question.QuestionSet.Id, new { @class = StaticValues.Class_indexedControl })%>
    
        <!-- Render the controls now -->
        <% switch(Model.Question.QuestionType.Name) { %>
            <% case "Text Box" : %>
                 <%= Html.TextBox(".Answer", Model.Answer, new {@class="indexedControl " + Model.Question.ValidationClasses}) %>
            <% break; %>
            <% case "Text Area" : %>
                <%= Html.TextArea(".Answer", Model.Answer, new { @class = StaticValues.Class_indexedControl })%>
            <% break; %>
            <% case "Boolean" : %>
                <%= Html.Encode(Model.Question.Name) %>
                <%
                    var ans = false;
                    if (!Boolean.TryParse(Model.Answer, out ans)) {
                        ans = false; } %>
                <%= Html.CheckBox(".Answer", ans, new {@class="indexedControl"})%>
            <% break; %>
            <% case "Radio Buttons" : %>               
                <% foreach (var o in Model.Question.Options)
                   { %> 
                    <%= Html.RadioButton(".Answer", o.Name, o.Name == Model.Answer , new { @class = StaticValues.Class_indexedControl + " " + Model.Question.ValidationClasses })%>
                    <%= Html.Encode(o.Name) %>
                <% } %>
            <% break; %>
            <% case "Checkbox List" : %>
                <% var options = !string.IsNullOrEmpty(Model.Answer) ? Model.Answer.Split(',') : new string[1]; %>
                <%= Html.Encode(Model.Answer) %>
                <% foreach (var o in Model.Question.Options)
                   {
                       var cblAns = options.Contains(o.Name);
                       %> 
                    <%= Html.CheckBox(".CblAnswer", cblAns, new { @class = StaticValues.Class_indexedControl + " " + Model.Question.ValidationClasses, @value = o.Name })%>
                    <%= Html.Encode(o.Name) %>
                <% } %>
            <% break; %>
            <% case "Drop Down" : %>
                <%= this.Select(".Answer").Options(Model.Question.Options, x => x.Name, x => x.Name).Class("indexedControl " + Model.Question.ValidationClasses)
                        .Selected(Model.Answer ?? string.Empty)
                        .FirstOption("--Select--") %>
            <% break; %>
            <% case "Date" : %>
                <%= Html.TextBox(".Answer", Model.Answer, new { @class = "dateForm indexedControl" + Model.Question.ValidationClasses })%>
            <% break; %>
        <% }; %>
        
    </p>