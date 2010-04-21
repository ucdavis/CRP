<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.QuestionSetViewModel>" %>
<%@ Import Namespace="CRP.Core.Domain"%>
<%@ Import Namespace="CRP.Controllers"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Edit Question Set
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function() {
            // assign the submit event to each of the delete links
            $("a.DeleteQuestion").click(function(event) { $(this).parents("form[name='DeleteQuestionForm']").submit(); });
        });
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <!-- //TODO: Create a link back based on the conditions of the question set -->

    <%= 
        Model.ItemType != null ?
            Html.ActionLink<ApplicationManagementController>(a => a.EditItemType(Model.ItemType.Id), "Back to Item Type") : (
                Model.Item != null ? 
                    Html.Encode("//TODO: Add a link back to the item") : 
                    Html.ActionLink<QuestionSetController>(a => a.List(), "Back to List")
        ) %>
            
    <fieldset>
        <legend>Properties</legend>
        <% using(Html.BeginForm()) {%>
        
            <%= Html.AntiForgeryToken() %>
        
            <%= Html.ClientSideValidation<QuestionSet>("QuestionSet") %>
        
            <p>
                <label for="Name">Name:</label>
                <%= Html.TextBox("QuestionSet.Name") %>
            </p>
            <p>
                <label for="SystemReusable">System Reusable:</label>
                <%= Html.CheckBox("QuestionSet.SystemReusable") %>
            </p>
            <p>
                <label for="CollegeReusable">College Reusable:</label>
                <%= Html.CheckBox("QuestionSet.CollegeReusable")%>
                
                <%= Model.QuestionSet.CollegeReusable ? Html.Encode(Model.QuestionSet.School.LongDescription) : Html.Encode(string.Empty) %>
            </p>    
            <p>
                <label for="UserReusable">User Reusable:</label>
                <%= Html.CheckBox("QuestionSet.UserReusable")%>
            </p>
                    
            <p>
                <button type="submit">Save</button>
            </p>
        <%} %>
    </fieldset>
    
    <fieldset>
        <legend>Questions</legend>
        
        <p>
        <%= Html.ActionLink<QuestionController>(a => a.Create(Model.QuestionSet.Id), "Create") %>
        </p>
        
        <%
            Html.Grid(Model.QuestionSet.Questions)
                .Transactional()
                .Name("Questions")
                .PrefixUrlParameters(false)
                .Columns(col =>
                             {
                                 col.Add(x =>
                                             {%>
                                                <% using(Html.BeginForm<QuestionController>(a => a.Delete(x.Id), FormMethod.Post, new { name="DeleteQuestionForm"})) { %>
                                                    <%= Html.AntiForgeryToken() %>
                                                    <a class="DeleteQuestion" href="javascript:;" >Delete</a>
                                                <%} %>
                                            <%
                                             });
                                 col.Add(x => x.Name).Title("Question");
                                 col.Add(x => x.QuestionType.Name).Title("Type");
                                 col.Add(x => x.Options.Count).Title("# of Options");
                                 col.Add(x => x.Required);
                             }
                )
                .Render(); %>
                
    </fieldset>

</asp:Content>


