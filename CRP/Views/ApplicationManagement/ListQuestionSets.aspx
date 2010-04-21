<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IQueryable<CRP.Core.Domain.QuestionSet>>" %>
<%@ Import Namespace="CRP.Controllers"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	ListQuestionSets
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>ListQuestionSets</h2>

    <p>
        <%= Html.ActionLink<ApplicationManagementController>(a => a.CreateQuestionSet(), "Create") %>
    </p>
    
    <% Html.Grid(Model)
           .Transactional()
           .Name("QuestionSets")
           .PrefixUrlParameters(false)
           .Columns(col =>
                        {
                            col.Add(a =>
                                        { %>
                                           <%= Html.Encode("Select") %> 
                                        <% }
                                );
                            col.Add(a => a.Name);
                            col.Add(a => a.Questions.Count).Title("# of Questions");
                            col.Add(a => a.User);
                        })
            .Pageable(x => x.PageSize(20))
            .Sortable()
            .Render(); %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
