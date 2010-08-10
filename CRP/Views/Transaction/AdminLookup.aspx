<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<CRP.Core.Domain.Transaction>>" %>
<%@ Import Namespace="CRP.Core.Resources" %>
<%@ Import Namespace="CRP.Controllers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	AdminLookup
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>AdminLookup</h2>

    <p>
        <% using (Html.BeginForm("AdminLookup","Transaction", FormMethod.Get)){%>
            <label for="email">Email: </label>
            <%= Html.TextBox("email")%>
            <input type="submit" value="Find" />
        <%}%>
    </p>

    <fieldset>
        <legend>Transactions</legend>
        
        <% Html.Grid(Model)
               .Transactional()
               .PrefixUrlParameters(false)
               .Name("Transactions")
               .CellAction(cell =>
               {
                   switch (cell.Column.Member)
                   {
                       case "IsActive":
                           cell.Text = cell.DataItem.IsActive ? "x" : string.Empty;
                           break;
                   }
               })
               .Columns(col =>
                            {
                                col.Template(a =>
                                            { %>
                                              <%= Html.ActionLink<ItemManagementController>(b => b.Details(a.Item.Id), "Details") %> 
                                            <% });
                                col.Bound((a => a.Item.Name)).Title("Item Name");
                                col.Bound(a => a.TransactionNumber).Title("Transaction");
                                col.Bound(a => a.IsActive).Title("Active");
                                col.Bound(a => a.TransactionDate);    
                                col.Bound(a => a.AmountTotal).Format("{0:C}").Title("Amount");
                                col.Bound(a => a.DonationTotal).Format("{0:C}").Title("Donation");
                                col.Bound(a => a.Total).Format("{0:C}").Title("Total");
                                col.Bound(a => a.TotalPaid).Format("{0:C}").Title("Paid");
                            })
                .Sortable()
                .Pageable()
                .Render(); %>
    </fieldset>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="PageHeader" runat="server">
    <% Html.RenderPartial(StaticValues.Partial_PageHeader, new DisplayProfile()); %>
</asp:Content>

