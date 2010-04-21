<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Question>" %>

    <p>
    
        <%= Html.Encode(Model.Name) %>
    
        <!-- Render the controls now -->
        <% switch(Model.QuestionType.Name) { %>
            <% case "Text Box" : %>
                <%= Html.Hidden(".QuestionId", Model.Id) %>
                <%= Html.Hidden(".QuestionSetId", Model.QuestionSet.Id) %>
                <%= Html.TextBox(".Answer") %>
            <% break; %>
            <% case "Text Area" : %>
                <%= Html.Hidden(".QuestionId", Model.Id) %>
                <%= Html.Hidden(".QuestionSetId", Model.QuestionSet.Id) %>            
                <%= Html.TextArea(".Answer")%>
            <% break; %>
            <% case "Boolean" : %>
                <%= Html.Hidden(".QuestionId", Model.Id) %>
                <%= Html.Hidden(".QuestionSetId", Model.QuestionSet.Id) %>
                <%= Html.CheckBox(".Answer")%>
            <% break; %>
            <% case "Radio Buttons" : %>               
                <%= Html.Hidden(".QuestionId", Model.Id) %>
                <%= Html.Hidden(".QuestionSetId", Model.QuestionSet.Id) %>
            
                <% foreach(var o in Model.Options) { %> 
                    <%= Html.RadioButton(".Answer", o.Id) %>
                    <%= Html.Encode(o.Name) %>
                <% } %>
            <% break; %>
            <% case "Checkbox List" : %>
                <%= Html.Hidden(".QuestionId", Model.Id) %>
                <%= Html.Hidden(".QuestionSetId", Model.QuestionSet.Id) %>
                
                <% foreach(var o in Model.Options) { %> 
                    <%= Html.CheckBox(".Answer", o.Id) %>
                    <%= Html.Encode(o.Name) %>
                <% } %>
            <% break; %>
            <% case "Drop Down" : %>
                <%= Html.Hidden(".QuestionId", Model.Id) %>
                <%= Html.Hidden(".QuestionSetId", Model.QuestionSet.Id) %>
                <%= this.Select(".Answer").Options(Model.Options, x=>x.Id, x=>x.Name)
                        .FirstOption("--Select--")%>
            <% break; %>
            <% case "Date" : %>
                    <%= Html.Hidden(".QuestionId", Model.Id) %>
                <%= Html.Hidden(".QuestionSetId", Model.QuestionSet.Id) %>
                <%= Html.TextBox(".Answer", null, new {@class = "dateForm"}) %>
            <% break; %>
        <% }; %>
        
    </p>