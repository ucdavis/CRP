<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.QuestionSetViewModel>" %>
<%@ Import Namespace="CRP.Controllers"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Edit Question Set
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <fieldset>
        <legend>Name</legend>
        
        <p>
            <%= Html.TextBox("QuestionSet.Name") %>
        </p>
    </fieldset>
    
    <fieldset>
        <legend>Questions</legend>
        
        <%= Html.ActionLink<QuestionController>(a => a.Create(Model.QuestionSet.Id), "Create") %>
        
        <%
            Html.Grid(Model.QuestionSet.Questions)
                .Transactional()
                .Name("Questions")
                .PrefixUrlParameters(false)
                .Columns(col =>
                             {
                                 col.Add(x =>
                                             {%>
                                                <%= Html.ActionLink<QuestionController>(a => a.Edit(x.Id), "Edit") %> |
                                                <%= Html.ActionLink<QuestionController>(a => a.Delete(x.Id), "Delete") %>
                                            <%
                                             });
                                 col.Add(x => x.Name).Title("Question");
                                 col.Add(x => x.QuestionType.Name).Title("Type");
                                 col.Add(x => x.Options.Count).Title("# of Options");
                             }
                ); %>
                
    </fieldset>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
