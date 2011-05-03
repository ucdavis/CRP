<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ItemQuestionViewModel>" %>
<%@ Import Namespace="CRP.Controllers.ViewModels"%>
<%@ Import Namespace="CRP.Core.Resources"%>

    <li>
        <% if (Model.Question.QuestionType.Name != "Boolean"){%>
        <%=Html.Encode(Model.Question.Name)%>
        <%}%>
        <%= Html.Hidden(".QuestionId", Model.Question.Id, new { @class = StaticValues.Class_indexedControl })%>
        <%= Html.Hidden(".QuestionSetId", Model.Question.QuestionSet.Id, new { @class = StaticValues.Class_indexedControl })%>
        <%= Html.Hidden(".QuantityIndex", 0, new { @class = StaticValues.Class_indexedControl })%>
        <% if (Model.Question.QuestionType.Name != "Boolean"){%>
        <br />
        <%}%>
    
        <!-- Render the controls now -->
        <% switch(Model.Question.QuestionType.Name) { %>
            <% case "Text Box" : %>
                 <%--<%= Html.TextBox(".Answer", Model.Answer, new { @class = "indexedControl " + Model.Question.ValidationClasses })%>--%>
                 <input name=".Answer" id="_Answer" class="indexedControl <%= Model.Question.ValidationClasses %>" type="text" <%= Model.Disable ? "disabled='true'" : null %> value="<%= Model.Answer %>" />
            <% break; %>
            <% case "Text Area" : %>
                <%= Html.TextArea(".Answer", Model.Answer, new { @class = StaticValues.Class_indexedControl })%>
            <% break; %>
            <% case "Boolean" : %>
                <%--<%= Html.Encode(Model.Question.Name) %>--%>
                <%
                    var ans = false;
                    if (!Boolean.TryParse(Model.Answer, out ans)) {
                        ans = false; } %>
                <%= Html.CheckBox(".Answer", ans, new {@class="indexedControl"})%> <%= Html.Encode(Model.Question.Name) %>
            <% break; %>
            <% case "Radio Buttons" : %>  
                <%--<%= Html.Encode(Model.Answer) %>--%> 
                <% var option = !string.IsNullOrEmpty(Model.Answer) ? Model.Answer.Trim().ToLower() : string.Empty;%>            
                <% foreach (var o in Model.Question.Options){ %> 
                    <%var isChecked = option == o.Name.Trim().ToLower();%>
                    <%= Html.RadioButton(".Answer", o.Name, isChecked , new { @class = StaticValues.Class_indexedControl + " " + Model.Question.ValidationClasses })%>
                    <%= Html.Encode(o.Name) %>
                <% } %>
            <% break; %>
            <% case "Checkbox List" : %>
                <% var options = !string.IsNullOrEmpty(Model.Answer) ? Model.Answer.Split(',') : new string[1]; %>
                <%--<%= Html.Encode(Model.Answer) %>--%>
                <% foreach (var o in Model.Question.Options){%>
                    <%var cblAns = options.Contains(o.Name); %>
                        
                    <%= Html.CheckBox(".CblAnswer", cblAns, new { @class = StaticValues.Class_indexedControl + " " + Model.Question.ValidationClasses, @value = o.Name })%>
                    <%= Html.Encode(o.Name) %>
                <% } %>
            <% break; %>
            <% case "Drop Down" : %>
                <%= this.Select(".Answer").Options(Model.Question.Options.OrderBy(a => a.Name), x => x.Name, x => x.Name).Class("indexedControl " + Model.Question.ValidationClasses)
                        .Selected(Model.Answer ?? string.Empty)
                        .FirstOption("--Select--") %>
            <% break; %>
            <% case "Date" : %>
                <%= Html.TextBox(".Answer", Model.Answer, new { @class = "dateForm indexedControl " + Model.Question.ValidationClasses })%>
            <% break; %>
        <% }; %>
    <span class="val_img">&nbsp</span>    
    </li>