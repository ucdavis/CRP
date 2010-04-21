<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<CRP.Core.Domain.QuestionSet>>" %>
<%@ Import Namespace="CRP.Controllers"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	List
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <p>
        <%= Html.ActionLink<QuestionSetController>(a => a.Create(null), "Create") %>
    </p>

    <% using (Html.BeginForm()) { %>
                <% Html.Grid(Model)
                   .Transactional()
                   .Name("QuestionSets")
                   .PrefixUrlParameters(false)
                   .Columns(col =>
                                {
                                    col.Add(x =>
                                                { %>
                                                    <%= Html.ActionLink<QuestionSetController>(a => a.Details(x.Id),"Select")%> |
                                                    <%= Html.ActionLink<QuestionSetController>(a => a.Edit(x.Id), "Edit") %>
                                                <% });
                                    col.Add(x => x.Name);
                                    col.Add(x => x.CollegeReusable);
                                    col.Add(x => x.SystemReusable);
                                    col.Add(x => x.UserReusable);
                                    col.Add(x => x.User.FullName).Title("User");
                                    col.Add(x => x.Questions.Count).Title("# of Questions");
                                })
                    .Pageable(x=>x.PageSize(20))
                    .Sortable()
                    .Render(); %>
    <%} %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

