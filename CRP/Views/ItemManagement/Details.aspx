<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.UserItemDetailViewModel>" %>
<%@ Import Namespace="CRP.Controllers"%>
<%@ Import Namespace="Resources"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Details
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
    <script type="text/javascript">
        $(function() { $("#tabs").tabs(); });
    </script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="PageHeader" runat="server">

    <% Html.RenderPartial(StaticValues.View_PageHeader, new DisplayProfile()); %>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Details</h2>
      
    <div id="tabs">
    
        <ul>
            <li><a href="#<%= Html.Encode(StaticValues.Tab_Transactions) %>">Transactions</a></li>
            <li><a href="#<%= Html.Encode(StaticValues.Tab_Checks) %>">Checks</a></li>
            <li><a href="#<%= Html.Encode(StaticValues.Tab_Reports) %>">Reports</a></li>
        </ul>
    
        <div id="<%= Html.Encode(StaticValues.Tab_Transactions) %>">
            <% Html.Grid(Model.Item.Transactions.Where(a => a.ParentTransaction == null)) 
                   .Transactional()
                   .Name("Transactions")
                   .PrefixUrlParameters(false)
                   .Columns(col =>
                                {
                                    col.Add(a => a.TransactionNumber).Title(
                                        "Transaction Number");
                                    col.Add(a => a.Quantity);
                                    col.Add(a => a.Amount.ToString("C")).Title("Amount");
                                    col.Add(a => a.DonationTotal.ToString("C")).Title("Donation Amount");
                                    col.Add(a => a.Credit ? "Credit Card" : "Check").Title("Payment Type");
                                    col.Add(a => a.Paid);
                                })
                   .Pageable()
                   .Sortable()
                   .Render();
                   %>
        </div>
        <div id="<%= Html.Encode(StaticValues.Tab_Checks) %>">
            <% Html.Grid(Model.Item.Transactions.Where(a => a.Check && a.ParentTransaction == null)) 
                   .Transactional()
                   .Name("Checks")
                   .PrefixUrlParameters(false)
                   .Columns(col =>
                                {
                                    col.Add(a =>
                                                {%>
                                                
                                                    <%= Html.ActionLink<CheckController>(b => b.LinkToTransaction(a.Id), "Select") %>
                                                
                                                <%});
                                    col.Add(a => a.TransactionNumber).Title(
                                        "Transaction Number");
                                    col.Add(a => a.Quantity);
                                    col.Add(a => a.Total.ToString("C")).Title("Amount");
                                    col.Add(a => a.Paid);
                                })
                   .Pageable()
                   .Sortable()
                   .Render();
                   %>
        </div>
        <div id="<%= Html.Encode(StaticValues.Tab_Reports) %>">
        
            <p>
                <%= Html.ActionLink<ReportController>(a => a.Create(Model.Item.Id), "Create") %>
            </p>
        
            <% Html.Grid(Model.Reports)
                   .Transactional()
                   .Name("SystemReports")
                   .Columns(col =>
                                {
                                    col.Add(a =>
                                                {%>
                                                    <%= Html.ActionLink<ReportController>(b => b.ViewReport(a.Id, Model.Item.Id), "Select") %>
                                                <%});
                                    col.Add(a => a.Name);
                                    col.Add(a => a.Columns.Count).Title("# of Columns");
                                    col.Add(a => a.User.FullName).Title("Created By");
                                    col.Add(a => a.SystemReusable).Title("System Report");
                                })
                   .PrefixUrlParameters(false)
                   .Render(); %>
        </div>
        
    
    </div>

    <p>
        <%= Html.ActionLink<ItemManagementController>(a => a.List(), "Back to List") %>
    </p>

</asp:Content>




