<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.QuestionViewModel>" %>
<%@ Import Namespace="Resources"%>
<%@ Import Namespace="CRP.Controllers"%>


<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Create
</asp:Content>

<asp:Content ID="pageHeader" ContentPlaceHolderID="PageHeader" runat="server">
    <% Html.RenderPartial(StaticValues.View_PageHeader, new DisplayProfile()); %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Create</h2>

    <%= Html.ClientSideValidation<Question>("") %>

    <%= Html.ValidationSummary("Create was unsuccessful. Please correct the errors and try again.") %>

    <% using (Html.BeginForm()) {%>

        <%= Html.AntiForgeryToken() %>

        <span style="display:none;" id="TypesWithOptions"><%= string.Join(",", Model.QuestionTypes.Where(a => a.HasOptions).Select(a => a.Id.ToString()).ToArray()) %></span>

        <fieldset>
            <legend>Fields</legend>
            
            <p>
                <label for="Name">Name:</label>
                <%= Html.TextBox("Name") %>
            </p>
            
            <p>
                <label for="Required">Required:</label>
                <%= Html.CheckBox("Required") %>
            </p>
            
            <p>
                <%= this.Select("QuestionType").Options(Model.QuestionTypes, x=>x.Id, x=>x.Name).FirstOption("--Select a Type--").Label("Question Type:") %>
            </p>
            
            <p id="Option" style="display:none;">
                <span id="OptionsContainer">
                </span>
                <img id="AddOptions" src="../../Images/plus.png" style="width:24px; height:24px;" />
            </p>
            
            <p>
                <input type="submit" value="Create" />
            </p>
        </fieldset>

    <% } %>

    <div>
        <%= Html.ActionLink<QuestionSetController>(a => a.Edit(Model.QuestionSet.Id), "Back to Question Set") %>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">

    <script type="text/javascript">
        $(document).ready(function() {
            // attach event handlers
            $("img#AddOptions").click(function() { AddOptionInput(); });
            $("select#QuestionType").change(function(event) { QuestionTypeChange(this); });
        });
        
        function AddOptionInput() {
            var index = $("span#OptionsContainer").children().length;

            var input = $("<input>");
            var name = "questionOptions[" + index + "]";
            input.attr("id", name);
            input.attr("name", name);

            $("span#OptionsContainer").append(input);
        }

        function QuestionTypeChange(obj) {
            var selectedId = $(obj).find("option:selected").val();
            var typesWithOptions = $("span#TypesWithOptions").html().split(",");

            if ($.inArray(selectedId, typesWithOptions) >= 0) {
                // it's in the array, it has options
                $("p#Option").show();
                $("span#OptionsContainer").empty();
            }
            else {
                $("p#Option").hide();
            }
        }
    </script>

</asp:Content>

