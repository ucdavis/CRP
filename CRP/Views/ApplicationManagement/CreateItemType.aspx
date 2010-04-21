<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.ItemTypeViewModel>" %>
<%@ Import Namespace="CRP.Core.Domain"%>
<%@ Import Namespace="CRP.Controllers"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	CreateItemType
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">

    <script type="text/javascript">

        $(document).ready(function() {
            $("img#addExtendedProperty").click(function() { AddExtendedProperty(); });
        });

        function AddExtendedProperty() {
        
            var count = $("div#ExtendedProperties").children().length;
            var div = $("<div>");
            div.append($("<label>").html("Property Name:"));
            div.append($("<input>").attr("id", "ExtendedProperties[" + count + "]_Name").attr("name", "ExtendedProperties[" + count + "].Name"));
            div.append($("span#QuestionTypeBase").clone().css("display", "block").attr("id", "ExtendedProperties[" + count + "]_QuestionType"));
            div.find("select").attr("id", "ExtendedProperties[" + count + "]_QuestionType").attr("name", "ExtendedProperties[" + count + "].QuestionType");

            $("div#ExtendedProperties").append(div);
        }
    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>CreateItemType</h2>

        <%= Html.ClientSideValidation<ItemType>("") %>

    <%= Html.ValidationSummary("Create was unsuccessful. Please correct the errors and try again.") %>

    <% using (Html.BeginForm()) {%>

        <%= Html.AntiForgeryToken() %>

        <fieldset>
            <legend>Name</legend>
            <p>
                <%= Html.TextBox("ItemType.Name") %>
                <%= Html.ValidationMessage("ItemType.Name", "*") %>
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

</asp:Content>


