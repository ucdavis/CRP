<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IQueryable<CRP.Core.Domain.ItemType>>" %>
<%@ Import Namespace="CRP.Controllers"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	ListItemTypes
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Item Types</h2>

    <p>
        <%= Html.ActionLink<ApplicationManagementController>(a => a.CreateItemType(), "Create") %>
    </p>

    <% Html.Grid(Model)
           .Transactional()
           .Name("Items")
           .PrefixUrlParameters(false)
           .Columns(col =>
                        {
                            col.Add(x =>
                                        {%>
                                            <%= Html.ActionLink<ApplicationManagementController>(a => a.EditItemType(x.Id), "Edit") %>|
                                        <%});
                            col.Add(x => x.Name);
                            col.Add(x => x.IsActive);
                        })
            .Pageable()
            .Sortable()
            .Render(); %>


</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

