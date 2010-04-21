<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CRP.Controllers.ViewModels.ItemTypeViewModel>" %>
<%@ Import Namespace="CRP.Core.Domain"%>
<%@ Import Namespace="CRP.Controllers"%>

    <script type="text/javascript">

        $(document).ready(function() {
            $("img#addExtendedProperty").click(function() { AddExtendedProperty(); });
        });
    
        function AddExtendedProperty() {
            var div = $("<div>");
            div.append($("<label>").html("Property Name:"));
            div.append($("<input>").attr("id", "ExtendedPropertyName").attr("name", "ExtendedPropertyName"));
            div.append($("span#QuestionTypeBase").clone().css("display", "block").attr("id", "QuestionType"));
            div.find("select").attr("id", "QuestionType").attr("name", "QuestionType");

            $("div#ExtendedProperties").append(div);
        }
    </script>

    <%= Html.ClientSideValidation<ItemType>("") %>

    <%= Html.ValidationSummary("Create was unsuccessful. Please correct the errors and try again.") %>

    <% using (Html.BeginForm()) {%>

        <%= Html.AntiForgeryToken() %>

        <fieldset>
            <legend>Name</legend>
            <p>
                <%= Html.TextBox("Name") %>
                <%= Html.ValidationMessage("Name", "*") %>
            </p>
        </fieldset>
        <fieldset>
            <legend>Extended Properties</legend>
            
            <%  if (Model.ItemType != null)
                {
                    foreach (var prop in Model.ItemType.ExtendedProperties)
                    {%>
               
               <label>Property Name:</label>
               <input id="ExtendedPropertyName" value='<%= Html.Encode(prop.Name) %>' />
               <label>Question Type:</label>
               <%= this.Select("QuestionType").Options(Model.QuestionTypes, x => x.Id, x => x.Name).Selected(prop.QuestionType.Id).Label("Question Type: ")%>               
               
            <% }
                } %>
            
            <div id="ExtendedProperties"></div>
            
            <img id="addExtendedProperty" src="../../Images/plus.png" style="width:24px; height:24px" />
        </fieldset>
        
            <span id="QuestionTypeBase" style="display:none;">
            <%= this.Select("QuestionTypeBase").Options(Model.QuestionTypes, x=>x.Id, x=>x.Name).FirstOption("Select a Question Type").Label("Question Type: ") %>
            </span>
            
            <p>
                <input type="submit" value="Create" />
            </p>

    <% } %>

    <div>
        <%= Html.ActionLink<ApplicationManagementController>(a => a.ListItemTypes(), "Back to List") %>
    </div>


