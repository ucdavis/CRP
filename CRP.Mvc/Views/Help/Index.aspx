<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.HelpTopicViewModel>" %>
<%@ Import Namespace="CRP.Controllers" %>
<%@ Import Namespace="CRP.Core.Resources" %>

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
                   .CellAction(cell =>
                    {
                        switch (cell.Column.Member)
                        {
                            case "AvailableToPublic":
                                cell.Text = cell.DataItem.AvailableToPublic ? "x" : string.Empty;
                                break;
                            case "IsActive":
                                cell.Text = cell.DataItem.IsActive ? "x" : string.Empty;
                                break;
                        }
                    })                                   
                   .Columns(col =>
                                {
                                    col.Template(x =>
                                                { %>
                                                    <% if (x.IsVideo){%>
                                                        <%= Html.ActionLink<HelpController>(a => a.WatchVideo(x.Id),"Watch")%>
                                                    <%}else{%>
                                                        <%= Html.ActionLink<HelpController>(a => a.Details(x.Id),"View")%>
                                                    <%}%>                                    
                                                    <% if (Model.IsUserAdmin){%>|
                                                    <%=Html.ActionLink<HelpController>(a => a.Edit(x.Id), "Edit")%> 
                                                    <%}%>
                                                <% });                                    
                                    col.Bound(x => x.IsActive).Visible(Model.IsUserAdmin).Title("Active");
                                    col.Bound(x => x.AvailableToPublic).Visible(Model.IsUserAdmin).Title("Public");
                                    col.Bound(x => x.NumberOfReads).Visible(Model.IsUserAdmin).Title("Reads");
                                    col.Bound(x => x.Question).Title("Frequently Asked Question");
                                })                  
                    .Pageable(x=>x.PageSize(20))
                    .Sortable()
                    .Render(); %>
    <%} %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="PageHeader" runat="server">
<% Html.RenderPartial(StaticValues.Partial_PageHeader, new DisplayProfile()); %>
</asp:Content>

