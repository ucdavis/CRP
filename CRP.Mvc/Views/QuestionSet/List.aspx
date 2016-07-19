<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IQueryable<CRP.Core.Domain.QuestionSet>>" %>
<%@ Import Namespace="CRP.Core.Resources"%>
<%@ Import Namespace="CRP.Controllers"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	List
</asp:Content>

<asp:Content ID="pageHeader" ContentPlaceHolderID="PageHeader" runat="server">
    <% Html.RenderPartial(StaticValues.Partial_PageHeader, new DisplayProfile()); %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <p>
        <%= Html.ActionLink<QuestionSetController>(a => a.Create(null, null, null, null), "Create") %>
    </p>

    <% using (Html.BeginForm()) { %>
                <% Html.Grid(Model)
                   .Transactional()
                   .Name("QuestionSets")
                   .PrefixUrlParameters(false)
                   .CellAction(cell =>
                   {
                       switch (cell.Column.Member)
                       {
                           case "User":
                               cell.Text = cell.DataItem.User != null ? cell.DataItem.User.FullName : string.Empty;
                               break;
                       }
                   }) 
                   .Columns(col =>
                                {
                                    col.Template(x =>
                                                { %>
                                                    <%= Html.ActionLink<QuestionSetController>(a => a.Details(x.Id),"Select")%> |
                                                    <%= Html.ActionLink<QuestionSetController>(a => a.Edit(x.Id), "Edit") %>
                                                <% });
                                    col.Bound(x => x.Name);
                                    col.Bound(x => x.CollegeReusable);
                                    col.Bound(x => x.SystemReusable);
                                    col.Bound(x => x.UserReusable);
                                    col.Bound(x => x.User).Title("User");
                                    col.Bound(x => x.Questions.Count).Title("# of Questions");
                                })
                    .Pageable(x=>x.PageSize(20))
                    .Sortable()
                    .Render(); %>
    <%} %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

