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
                else if ($item.parents().filter("Div#Refunds").length > 0) {
                    link = link + "#Refunds";
                }
                else if ($item.parents().filter("Div#Notifications").length > 0) {
                    link = link + "#Notifications";
                }
                $item.attr("href", link);
            });

            $.each($("Img"), function(index, item) {
                if ($(item).hasClass("CreditIdImage")) {
                    $(item).bt({trigger: 'click', width: '430px'});
                }
            });

            $.each($("Img"), function(index, item) {
                if ($(item).hasClass("ReasonImage")) {
                    $(item).bt({ width: '430px' });
                }
            });
        });
    </script>

</asp:Content>
   

<asp:Content ID="Content4" ContentPlaceHolderID="PageHeader" runat="server">

    <% Html.RenderPartial(StaticValues.Partial_PageHeader, new DisplayProfile()); %>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">    
    <h2>Details: <%=Html.Encode(Model.Item.Name) %></h2>

    <div id="tabs">
    
        <ul>
            <li><a href="#<%= Html.Encode(StaticValues.Tab_Transactions) %>">Transactions</a></li>
            <li><a href="#<%= Html.Encode(StaticValues.Tab_Checks) %>">Checks</a></li>
            <li><a href="#<%= Html.Encode(StaticValues.Tab_Reports) %>">Reports</a></li>
            <li><a href="#<%= Html.Encode(StaticValues.Tab_Refunds) %>">Refunds</a></li>
            <li><a href="#<%= Html.Encode(StaticValues.Tab_Notifications) %>">Notifications</a></li>
        </ul>
    
        <div id="<%= Html.Encode(StaticValues.Tab_Transactions) %>">
            <% Html.Grid(Model.Item.Transactions.Where(a => a.ParentTransaction == null)) 
                   .Transactional()
                   .Name("Transactions")
                   .CellAction(cell =>
                   {
                       switch (cell.Column.Member)
                       {
                           case "Credit":
                               cell.Text = cell.DataItem.Credit ? "Credit Card" : "Check";
                               break;
                       }
                   }) 
                   .Columns(col =>
                                {
                                    col.Template(a =>
                                        {%>                                        
                                            <% using (Html.BeginForm<ItemManagementController>(x => x.ToggleTransactionIsActive(a.Id, Request.QueryString["Transactions-orderBy"], Request.QueryString["Transactions-page"])))
                                               {%>                                     
                                                <%= Html.AntiForgeryToken() %>
                                                <a href="javascript:;" class="FormSubmit"><%= a.IsActive ? "Deactivate" : "Activate" %></a>
                                            
                                            <%} %>
                                            <%});
                                    col.Bound(a => a.TransactionNumber).Title(
                                        "Transaction");
                                    col.Bound(a => a.Quantity);
                                    col.Bound(a => a.Amount).Format("{0:C}");
                                    col.Bound(a => a.DonationTotal).Format("{0:C}").Title("Donation");
                                    col.Bound(a => a.Credit).Title("Payment Type");
                                    col.Bound(a => a.Paid);
                                    col.Bound(a => a.IsActive);
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
                                    col.Template(a =>
                                                {%>
                                                
                                                    <%= Html.ActionLink<PaymentController>(b => b.LinkToTransaction(a.Id, Request.QueryString["Checks-orderBy"], Request.QueryString["Checks-page"]), "Select")%> |
                                                    <%= Html.ActionLink<TransactionController>(b => b.Edit(a.Id, Request.QueryString["Checks-orderBy"], Request.QueryString["Checks-page"]), "Edit")%>
                                                
                                                <%});
                                    col.Bound(a => a.TransactionNumber).Title("Transaction Number");
                                    col.Bound(
                                        a =>
                                        Model.CheckName.Where(z => z.TransactionNumber == a.TransactionNumber).
                                            FirstOrDefault().FullName).Sortable(false).Title("Contact Name");
                                    col.Bound(a => a.Quantity);
                                    col.Bound(a => a.Total).Format("{0:C}").Title("Amount");
                                    col.Bound(a => a.Paid);
                                })
                   .Pageable()
                   .Sortable()
                   .PrefixUrlParameters(true)
                   .Render(); 
                   %>
        </div>
        <div id="<%= Html.Encode(StaticValues.Tab_Reports) %>">
        
            <p style="position:absolute; margin:-2em 0 0 64em;">
                <%= Html.ActionLink<ReportController>(a => a.Create(Model.Item.Id), "Create") %>
            </p>
        
            <% Html.Grid(Model.Reports)
                   .Transactional()
                   .Name("SystemReports")
                   .Columns(col =>
                                {
                                    col.Template(a =>
                                                {%>
                                                    <%= Html.ActionLink<ReportController>(b => b.ViewReport(a.Id, Model.Item.Id), "Select") %>
                                                <%});
                                    col.Bound(a => a.Name);
                                    col.Bound(a => a.Columns.Count).Title("# of Columns");
                                    col.Bound(a => a.User.FullName).Title("Created By");
                                    col.Bound(a => a.SystemReusable).Title("System Report");
                                })
                   .Render(); %>
        </div>
        <div id="<%= Html.Encode(StaticValues.Tab_Refunds) %>">
            <% Html.Grid(Model.Item.Transactions.Where(a => a.ParentTransaction == null)) 
                   .Transactional()
                   .Name("Refunds")
                   .CellAction(cell =>
                   {
                       switch (cell.Column.Member)
                       {
                           //case "Credit":
                           //    cell.Text = cell.DataItem.Credit ? "x" : string.Empty;
                           //    break;
                           case "Paid":
                               cell.Text = cell.DataItem.Paid ? "x" : string.Empty;
                               break;
                           //case "TransactionGuid":
                           //    cell.Text = cell.DataItem.Credit ? cell.DataItem.TransactionGuid.ToString() + Model.Fid : " ";
                           //    break;
                       }
                   }) 
                   .Columns(col =>
                                {
                                    col.Template(a =>
                                                {%>     
                                                    <%if (!a.RefundIssued)
                                                      {%>                                                                                               
                                                        <%=Html.ActionLink<TransactionController>(b => b.Refund(a.Id, Request.QueryString["Refunds-orderBy"], Request.QueryString["Refunds-page"]), "Refund")%>
                                                    <%}%>
                                                    <%else{%>
                                                        <% using (Html.BeginForm<TransactionController>(x => x.RemoveRefund(a.Id, Request.QueryString["Transactions-orderBy"], Request.QueryString["Transactions-page"]), FormMethod.Post, new { @class = "display_inline" }))
                                                           {%>                                     
                                                            <%= Html.AntiForgeryToken()%>
                                                            <a href="javascript:;" class="FormSubmit"><%= "Undo"%></a>                                            
                                                        <%} %>
                                                        | <%=Html.ActionLink<TransactionController>(b => b.DetailsRefund(a.Id, Request.QueryString["Refunds-orderBy"], Request.QueryString["Refunds-page"]), "Details")%>
                                                    <%}%>                                                
                                                <%});
                                    col.Bound(a => a.TransactionNumber).Title("Transaction");
                                    col.Template(a => 
                                        {%> 
                                            <%if (a.Credit){%>
                                                <%= Html.Image("~/images/technorati.ico",new {@class="CreditIdImage", title=Html.Encode(a.TransactionGuid) + Model.Fid})%> 
                                            <%}%>
                                        <%}).Title("Credit Card Id");
                                    //col.Bound(a => a.TransactionGuid).Title("Refund Identifier");
                                    //col.Bound(a => a.Credit).Title("Credit Card");                                  
                                    col.Bound(a => a.Total).Format("{0:C}").Title("Amount");
                                    col.Bound(a => a.TotalPaid).Format("{0:$#,##0.00;($#,##0.00); }");
                                    col.Bound(a => a.Paid);
                                    col.Bound(a => a.RefundAmount).Format("{0:$#,##0.00;($#,##0.00); }").Title("Refunded");
                                })
                   .Pageable()
                   .Sortable()                   
                   .PrefixUrlParameters(true)
                   .Render(); 
                   %>
        </div>
        <div id="<%= Html.Encode(StaticValues.Tab_Notifications) %>">
            <% Html.Grid(Model.Item.Transactions.Where(a => a.ParentTransaction == null  && a.IsActive && (a.Paid || a.Notified))) 
                   .Transactional()
                   .Name("Notifiactions")
                   .CellAction(cell =>
                   {
                       switch (cell.Column.Member)
                       {
                           case "Check":
                               cell.Text = cell.DataItem.Check ? "x" : string.Empty ;
                               break;
                           case "Paid":
                               cell.Text = cell.DataItem.Paid ? "x" : string.Empty;
                               break;
                           case "Notified":
                               cell.Text = cell.DataItem.Notified ? "x" : string.Empty;
                               break;
                           //case "NotifiedDate":
                           //    cell.Text = cell.DataItem.NotifiedDate == null ? string.Empty : (DateTime)cell.DataItem.NotifiedDate;
                           //    break;
                       }
                   }) 
                   .Columns(col =>
                                {
                                    col.Template(a =>
                                                {%>                                                                                                 
                                                    <% using (Html.BeginForm<TransactionController>(x => x.SendNotification(a.Id, Request.QueryString["Notifications-orderBy"], Request.QueryString["Notifications-page"]), FormMethod.Post, new { @class = "display_inline" }))
                                                       {%>                                     
                                                        <%= Html.AntiForgeryToken()%>
                                                        <a href="javascript:;" class="FormSubmit"><%= "Send"%></a>                                            
                                                    <%} %>                                              
                                                <%});
                                    col.Bound(a => a.TransactionNumber).Title("Transaction");
                                    col.Bound(a => a.Notified);
                                    col.Bound(a => a.NotifiedDate);
                                    col.Bound(a => a.Paid);
                                    col.Bound(a => a.TotalPaid).Format("{0:$#,##0.00;($#,##0.00); }");
                                    col.Bound(a => a.Check).Title("Check"); 
                                })
                   .Pageable()
                   .Sortable()                   
                   .PrefixUrlParameters(true)
                   .Render(); 
                   %>
        </div>
    
    </div>
    
    <p>
        <%= Html.ActionLink<ItemManagementController>(a => a.List(null), "Back to List") %>
    </p>

</asp:Content>





