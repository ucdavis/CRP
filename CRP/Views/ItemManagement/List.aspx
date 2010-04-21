<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IQueryable<CRP.Core.Domain.Item>>" %>
<%@ Import Namespace="CRP.Controllers"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	List
</asp:Content>

<asp:Content ID="pageHeader" ContentPlaceHolderID="PageHeader" runat="server">
    <% Html.RenderPartial("~/Views/Shared/PageHeader.ascx", new DisplayProfile()); %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <p>
        <%= Html.ActionLink<ItemManagementController>(a => a.Create(), "Create New") %>
    </p>

    <% Html.Grid(Model)
           .Transactional()
           .Name("Items")
           .PrefixUrlParameters(false)
           .Columns(col =>
                        {
                            col.Add(a =>
                                        {%>
                                            <%= Html.ActionLink<ItemManagementController>(b => b.Edit(a.Id), "Edit") %>
                                        <%});
                            col.Add(a => a.Name);
                            col.Add(a => a.CostPerItem).Format("{0:C}");
                            col.Add(a => a.Quantity);
                            col.Add(a =>
                                        {%>
                                            <%= Html.Encode(a.Transactions.Sum(b => b.Quantity)) %>
                                        <%}).Title("# Sold");
                            col.Add(a => a.Expiration).Format("{0:d}");
                            col.Add(a => a.DateCreated);
                            col.Add(a => a.Available);
                        })
            .Sortable()
            .Pageable()
            .Render(); %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

