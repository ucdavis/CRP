<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.UserItemDetailViewModel>" %>
<%@ Import Namespace="CRP.Controllers"%>
<%@ Import Namespace="CRP.Core.Resources"%>
<%@ Import Namespace="Telerik.Web.Mvc.UI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Details
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
    <script type="text/javascript">
    
    
        $(function() { $("#tabs").tabs(); });
        $(document).ready(function() {
            $("a.FormSubmit").click(function() { $(this).parents("form").submit(); });
            $.each($("a.t-link"), function(index, item) {
                var $item = $(item);
                var link = $item.attr("href");

                if ($item.parents().filter("Div#Checks").length > 0) {
                    link = link + "#Checks";
                }
                else if ($item.parents().filter("Div#Transactions").length > 0) {
                    link = link + "#Transactions";
                }
                else if ($item.parents().filter("Div#Reports").length > 0) {
                    link = link + "#Reports";
                }
                $item.attr("href", link);
            });
        });
    </script>
); 
</asp:Content>
   

<asp:Content ID="Content4" ContentPlaceHolderID="PageHeader" runat="server">

    <% Html.RenderPartial(StaticValues.Partial_PageHeader, new DisplayProfile()); %>

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
                   .Columns(col =>
                                {
                                    col.Add(a =>
                                        {%>                                        
                                            <% using (Html.BeginForm<ItemManagementController>(x => x.ToggleTransactionIsActive(a.Id, Request.QueryString["Transactions-orderBy"], Request.QueryString["Transactions-page"])))
                                               {%>                                     
                                                <%= Html.AntiForgeryToken() %>
                                                <a href="javascript:;" class="FormSubmit"><%= a.IsActive ? "Deactivate" : "Activate" %></a>
                                            
                                            <%} %>
                                            <%});
                                    col.Add(a => a.TransactionNumber).Title(
                                        "Transaction");
                                    col.Add(a => a.Quantity);
                                    col.Add(a => a.Amount.ToString("C")).Title("Amount");
                                    col.Add(a => a.DonationTotal.ToString("C")).Title("Donation");
                                    col.Add(a => a.Credit ? "Credit Card" : "Check").Title("Payment Type");
                                    col.Add(a => a.Paid);
                                    col.Add(a => a.IsActive);
                                })
                   .Pageable()
                   .Sortable()
                   .PrefixUrlParameters(true)
                   .Render();
                   %>
        </div>
        <div id="<%= Html.Encode(StaticValues.Tab_Checks) %>">
            <% Html.Grid(Model.Item.Transactions.Where(a => a.Check && a.ParentTransaction == null  && a.IsActive)) 
                   .Transactional()
                   .Name("Checks")
                   .Columns(col =>
                                {
                                    col.Add(a =>
                                                {%>
                                                
                                                    <%= Html.ActionLink<PaymentController>(b => b.LinkToTransaction(a.Id, Request.QueryString["Checks-orderBy"], Request.QueryString["Checks-page"]), "Select")%> |
                                                    <%= Html.ActionLink<TransactionController>(b => b.Edit(a.Id, Request.QueryString["Checks-orderBy"], Request.QueryString["Checks-page"]), "Edit")%>
                                                
                                                <%});                      
                                    col.Add(a => a.TransactionNumber).Title("Transaction Number");
                                    col.Add(a => a.Quantity);
                                    col.Add(a => a.Total.ToString("C")).Title("Amount");
                                    col.Add(a => a.Paid);
                                })
                   .Pageable()
                   .Sortable()
                   .PrefixUrlParameters(true)
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
                   .Render(); %>
        </div>
        
    
    </div>
    
    <p>
        <%= Html.ActionLink<ItemManagementController>(a => a.List(), "Back to List") %>
    </p>

</asp:Content>





