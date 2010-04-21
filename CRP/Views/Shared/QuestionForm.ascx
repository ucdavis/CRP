<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ItemQuestionViewModel>" %>
<%@ Import Namespace="CRP.Controllers.ViewModels"%>
<%@ Import Namespace="CRP.Core.Resources"%>

    <p>
    
        <%= Html.Encode(Model.Question.Name) %>
    
        <!-- Render the controls now -->
        <% switch(Model.Question.QuestionType.Name) { %>
            <% case "Text Box" : %>
                <%= Html.Hidden(".QuestionId", Model.Question.Id, new { @class = StaticValues.Class_indexedControl })%>
                <%= Html.Hidden(".QuestionSetId", Model.Question.QuestionSet.Id, new { @class = StaticValues.Class_indexedControl })%>
                
                
                <%
                    string value = string.Empty;
                    if (Model.Question.QuestionSet.Name == StaticValues.QuestionSet_ContactInformation
                        && Model.Question.QuestionSet.SystemReusable && Model.Question.QuestionSet.User == null
                        && Model.OpenIDUser != null
                       )
                    {
                        if (Model.Question.Name == StaticValues.Question_FirstName)
                        {
                            value = Model.OpenIDUser.FirstName;
                        }
                        else if (Model.Question.Name == StaticValues.Question_LastName)
                        {
                            value = Model.OpenIDUser.LastName;
                        }
                        else if (Model.Question.Name == StaticValues.Question_StreetAddress)
                        {
                            value = Model.OpenIDUser.StreetAddress;
                        }
                        else if (Model.Question.Name == StaticValues.Question_AddressLine2)
                        {
                            value = Model.OpenIDUser.Address2;
                        }
                        else if (Model.Question.Name == StaticValues.Question_City)
                        {
                            value = Model.OpenIDUser.City;
                        }
                        else if (Model.Question.Name == StaticValues.Question_State)
                        {
                            value = Model.OpenIDUser.State;
                        }
                        else if (Model.Question.Name == StaticValues.Question_Zip)
                        {
                            value = Model.OpenIDUser.Zip;
                        }
                        else if (Model.Question.Name == StaticValues.Question_PhoneNumber)
                        {
                            value = Model.OpenIDUser.PhoneNumber;
                        }
                        else if (Model.Question.Name == StaticValues.Question_Email)
                        {
                            value = Model.OpenIDUser.Email;
                        }
                    }  %>
                <%= Html.TextBox(".Answer", value, new {@class="indexedControl"}) %>
            <% break; %>
            <% case "Text Area" : %>
                <%= Html.Hidden(".QuestionId", Model.Question.Id, new { @class = StaticValues.Class_indexedControl })%>
                <%= Html.Hidden(".QuestionSetId", Model.Question.QuestionSet.Id, new { @class = StaticValues.Class_indexedControl })%>            
                <%= Html.TextArea(".Answer", string.Empty, new { @class = StaticValues.Class_indexedControl })%>
            <% break; %>
            <% case "Boolean" : %>
                <%= Html.Hidden(".QuestionId", Model.Question.Id, new { @class = StaticValues.Class_indexedControl })%>
                <%= Html.Hidden(".QuestionSetId", Model.Question.QuestionSet.Id, new { @class = StaticValues.Class_indexedControl })%>
                <%= Html.CheckBox(".Answer", false, new {@class="indexedControl"})%>
            <% break; %>
            <% case "Radio Buttons" : %>               
                <%= Html.Hidden(".QuestionId", Model.Question.Id, new { @class = StaticValues.Class_indexedControl })%>
                <%= Html.Hidden(".QuestionSetId", Model.Question.QuestionSet.Id, new { @class = StaticValues.Class_indexedControl })%>
            
                <% foreach (var o in Model.Question.Options)
                   { %> 
                    <%= Html.RadioButton(".Answer", o.Name, new { @class = StaticValues.Class_indexedControl })%>
                    <%= Html.Encode(o.Name) %>
                <% } %>
            <% break; %>
            <% case "Checkbox List" : %>
                <%= Html.Hidden(".QuestionId", Model.Question.Id, new { @class = StaticValues.Class_indexedControl })%>
                <%= Html.Hidden(".QuestionSetId", Model.Question.QuestionSet.Id, new { @class = StaticValues.Class_indexedControl })%>
                
                <% foreach (var o in Model.Question.Options)
                   { %> 
                    <%= Html.CheckBox(".Answer", false, new { @class = StaticValues.Class_indexedControl })%>
                    <%= Html.Encode(o.Name) %>
                <% } %>
            <% break; %>
            <% case "Drop Down" : %>
                <%= Html.Hidden(".QuestionId", Model.Question.Id, new { @class = StaticValues.Class_indexedControl })%>
                <%= Html.Hidden(".QuestionSetId", Model.Question.QuestionSet.Id, new { @class = StaticValues.Class_indexedControl })%>
                <%= this.Select(".Answer").Options(Model.Question.Options, x => x.Name, x => x.Name).Class("indexedControl")
                        .FirstOption("--Select--") %>
            <% break; %>
            <% case "Date" : %>
                <%= Html.Hidden(".QuestionId", Model.Question.Id, new { @class = StaticValues.Class_indexedControl })%>
                <%= Html.Hidden(".QuestionSetId", Model.Question.QuestionSet.Id, new { @class = StaticValues.Class_indexedControl })%>
                <%= Html.TextBox(".Answer", null, new {@class = "dateForm indexedControl"}) %>
            <% break; %>
        <% }; %>
        
    </p>