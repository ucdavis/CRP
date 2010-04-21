<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.QuestionSetViewModel>" %>
<%@ Import Namespace="CRP.Core.Domain"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	CreateQuestionSet
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">

    <style type="text/css">
        .Question
        {
            border: solid 1px gray;
            padding: 10px;
        }
        .Options
        {
            border: dotted 1px gray;
            padding: 10px;
        }
        
    </style>

    <script type="text/javascript">

        $(document).ready(function() {
            $("img#addQuestions").click(function() { AddQuestion(); });
            $("img.addOption").live("click", function(item) {
                var $item = $(item.target);
                var index = $item.parents("div.Question").attr("index");
                var count = $item.parents("div.Options").children("input").length;

                $item.before($("<input>").attr("id", "questions[" + index + "]_Options[" + count + "]_Name").attr("name", "questions[" + index + "].Options[" + count + "].Name"));
            });
            $("select").live("change", function(item) {
                // parse out the information
                var $item = $(item.target);
                var $selected = $item.find("option:selected");
                var value = $selected[0].value;
                var index = $item.parents("div.Question").attr("index");

                // convert csv to array of type with options
                var hasOptions = $("div#QuestionTypeHasOptions").html().split(",");

                // type needs options
                if ($.inArray(value, hasOptions) >= 0) {
                    $item.siblings("div.Options").prepend($("<input>").attr("id", "questions[" + index + "]_Options[0]_Name").attr("name", "questions[" + index + "].Options[0].Name"));
                    $item.siblings("div.Options").css("display", "block");
                }
            });
        });

        function AddQuestion() {

            var count = $("div#Questions").children().length;
            var div = $("<div>").addClass("Question").attr("index", count);
            div.append($("<label>").html("Property Name:"));
            div.append($("<input>").attr("id", "Questions[" + count + "]_Name").attr("name", "Questions[" + count + "].Name"));
            div.append($("span#QuestionTypeBase").find("select").clone().attr("id", "Questions[" + count + "]_QuestionType").attr("name", "Questions[" + count + "].QuestionType"));
            
            div.append($("<div>").addClass("Options").css("display", "none").append($("<img>").addClass("addOption").attr("src", "../../Images/plus.png").css("height", "24px").css("width", "24px")));

            $("div#Questions").append(div);
        }
    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Create Question Set</h2>
    
    <div id="QuestionTypeHasOptions" style="display:none;"><%= String.Join(",", Model.QuestionTypes.Where(a => a.HasOptions).Select(a => a.Id.ToString()).ToArray()) %></div>

    <%= Html.ClientSideValidation<QuestionSet>("QuestionSet") %>

    <%= Html.ValidationSummary("Create was unsuccessful. Please correct the errors and try again.") %>

    <% using (Html.BeginForm()) {%>

        <%= Html.AntiForgeryToken() %>

        <fieldset>
            <legend>Name</legend>
            <p>
                <label for="Name">Name:</label>
                <%= Html.TextBox("QuestionSet.Name") %>
                <%= Html.ValidationMessage("Name", "*") %>
            </p>
        </fieldset>

        <fieldset>
            <legend>Questions</legend>
            
            <div id="Questions">
                <!-- Add in the existing questions -->
                <% for (var i = 0; i < Model.QuestionSet.Questions.Count; i++ )
                       { %> 
                        <div class="Question" index='<%= Html.Encode(i) %>'>
                            <label>Property Name:</label>
                            <%= Html.TextBox("Questions[" + i.ToString() + "].Name") %>
                            <%= this.Select("Questions[" + i.ToString() + "].QuestionType").Options(Model.QuestionTypes, x=>x.Id, x=>x.Name).FirstOption("Select a Question Type").Label("Question Type: ") %>                        
                            
                            <!-- Add in the existing options if any -->
                            <div class="Options" style='<%= Model.QuestionSet.Questions.ToList()[i].Question.QuestionType.HasOptions ? "block" : "none" %>'>
                                <% for(var j = 0; j < Model.QuestionSet.Questions.ToList()[i].Question.Options.Count; j++ )
                                   { %>
        
                                    <%= Html.TextBox("questions[" + i.ToString() + "].Options[" + j.ToString() + "].Name") %>
        
                                <% } %>
                                <img id="addOption" src="../../Images/plus.png" style="width:24px; height:24px;" />        
                            </div>
                        </div>
                <% } %>
            </div>
            
            <img id="addQuestions" src="../../Images/plus.png" style="width:24px; height:24px" />
        </fieldset>

            <span id="QuestionTypeBase" style="display:none;">
                <%= this.Select("QuestionTypeBase").Options(Model.QuestionTypes, x=>x.Id, x=>x.Name).FirstOption("Select a Question Type").Label("Question Type: ") %>
            </span>

        <p>
            <input type="submit" value="Create" />
        </p>

    <% } %>

    <div>
        <%=Html.ActionLink("Back to List", "Index") %>
    </div>

</asp:Content>



