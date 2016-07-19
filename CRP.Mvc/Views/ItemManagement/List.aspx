<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IQueryable<CRP.Core.Domain.Item>>" %>
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
        <%= Html.ActionLink<ItemManagementController>(a => a.Create(), "Create New") %> |
        <%= Html.ActionLink<HelpController>(a => a.CreateItem(), "Watch Demo") %>
    </p>
    <p>
        <% using (Html.BeginForm("List","ItemManagement", FormMethod.Get)){%>
            <%--<%= Html.AntiForgeryToken() %>--%>
            <label for="transactionNumber">Transaction Number: </label>
            <%= Html.TextBox("transactionNumber")%>
            <input type="submit" value="Filter" />
        <%}%>
    </p>

    <% Html.Grid(Model)
           .Transactional()
           .Name("Items")
           .PrefixUrlParameters(false)
           .Columns(col =>
                        {
                            col.Template(a =>
                                        {%>
                                            <%= Html.ActionLink<ItemManagementController>(b => b.Details(a.Id), "Details") %> | 
                                            <%= Html.ActionLink<ItemManagementController>(b => b.Edit(a.Id), "Edit") %> |
                                            <%= Html.ActionLink<ItemManagementController>(b => b.Map(a.Id), "Map") %> |
                                            <%= Html.ActionLink<ItemManagementController>(b => b.Copy(a.Id), "Copy") %>
                                        <%});
                            col.Bound(a => a.Name);
                            col.Bound(a => a.CostPerItem).Format("{0:C}");
                            col.Bound(a => a.Quantity);
                            col.Template(a =>
                                        {%>
                                            <%= Html.Encode(a.Transactions.Where(b=>!b.Donation && b.IsActive).Sum(b => b.Quantity)) %>
                                        <%}).Title("# Sold");
                            col.Bound(a => a.Expiration).Format("{0:d}");
                            col.Bound(a => a.DateCreated);
                            col.Bound(a => a.Available);     
                            col.Template(a => 
                                        {%>
                                            <%= Html.ActionLink<TransactionController>(b => b.Checkout(a.Id, null, null, null, null), "Register") %>
                                        <%});                      
                        })
            .Sortable()
            .Pageable()
            .Render(); %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

