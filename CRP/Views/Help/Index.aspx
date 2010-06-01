<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.HelpTopicViewModel>" %>
<%@ Import Namespace="CRP.Controllers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Help Topics</h2>
    <% if (Model.IsUserAuthorized){%>
    <p>
        <%=Html.ActionLink("Create New", "Create")%>
    </p>
    <%}%>
    <% using (Html.BeginForm()) { %>
                <% Html.Grid(Model.HelpTopics)
                   .Transactional()
                   .Name("Help Topics")
                   .PrefixUrlParameters(false)
                   .Columns(col =>
                                {
                                    col.Add(x =>
                                                { %>
                                                    <%= Html.ActionLink<HelpController>(a => a.Details(x.Id),"Select")%> 
                                                    <% if (Model.IsUserAdmin){%>|
                                                    <%=Html.ActionLink<HelpController>(a => a.Edit(x.Id), "Edit")%> 
                                                    <%}%>
                                                <% });
                                    <% if (Model.IsUserAdmin){%>
                                    col.Add(x => x.IsActive);
                                    col.Add(x => x.NumberOfReads);
                                    <%}%>
                                    col.Add(x => x.Name);
                                    col.Add(x => x.ShortDescription);
                                })
                    .Pageable(x=>x.PageSize(20))
                    .Sortable()
                    .Render(); %>
    <%} %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="PageHeader" runat="server">
</asp:Content>

