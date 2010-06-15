<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<CRP.Core.Domain.Transaction>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	AdminLookup
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>AdminLookup</h2>

    <table>
        <tr>
            <th></th>
            <th>
                TransactionDate
            </th>
            <th>
                IsActive
            </th>
            <th>
                Credit
            </th>
            <th>
                Check
            </th>
            <th>
                Refunded
            </th>
            <th>
                Amount
            </th>
            <th>
                Donation
            </th>
            <th>
                Quantity
            </th>
            <th>
                TransactionNumber
            </th>
            <th>
                CreatedBy
            </th>
            <th>
                CorrectionReason
            </th>
            <th>
                TransactionGuid
            </th>
            <th>
                DonationTotal
            </th>
            <th>
                UncorrectedDonationTotal
            </th>
            <th>
                CorrectionTotal
            </th>
            <th>
                RefundAmount
            </th>
            <th>
                RefundIssued
            </th>
            <th>
                AmountTotal
            </th>
            <th>
                Total
            </th>
            <th>
                TotalPaid
            </th>
            <th>
                TotalPaidByCheck
            </th>
            <th>
                TotalPaidByCredit
            </th>
            <th>
                Paid
            </th>
            <th>
                Id
            </th>
        </tr>

    <% foreach (var item in Model) { %>
    
        <tr>
            <td>
                <%= Html.ActionLink("Edit", "Edit", new { /* id=item.PrimaryKey */ }) %> |
                <%= Html.ActionLink("Details", "Details", new { /* id=item.PrimaryKey */ })%>
            </td>
            <td>
                <%= Html.Encode(String.Format("{0:g}", item.TransactionDate)) %>
            </td>
            <td>
                <%= Html.Encode(item.IsActive) %>
            </td>
            <td>
                <%= Html.Encode(item.Credit) %>
            </td>
            <td>
                <%= Html.Encode(item.Check) %>
            </td>
            <td>
                <%= Html.Encode(item.Refunded) %>
            </td>
            <td>
                <%= Html.Encode(String.Format("{0:F}", item.Amount)) %>
            </td>
            <td>
                <%= Html.Encode(item.Donation) %>
            </td>
            <td>
                <%= Html.Encode(item.Quantity) %>
            </td>
            <td>
                <%= Html.Encode(item.TransactionNumber) %>
            </td>
            <td>
                <%= Html.Encode(item.CreatedBy) %>
            </td>
            <td>
                <%= Html.Encode(item.CorrectionReason) %>
            </td>
            <td>
                <%= Html.Encode(item.TransactionGuid) %>
            </td>
            <td>
                <%= Html.Encode(String.Format("{0:F}", item.DonationTotal)) %>
            </td>
            <td>
                <%= Html.Encode(String.Format("{0:F}", item.UncorrectedDonationTotal)) %>
            </td>
            <td>
                <%= Html.Encode(String.Format("{0:F}", item.CorrectionTotal)) %>
            </td>
            <td>
                <%= Html.Encode(String.Format("{0:F}", item.RefundAmount)) %>
            </td>
            <td>
                <%= Html.Encode(item.RefundIssued) %>
            </td>
            <td>
                <%= Html.Encode(String.Format("{0:F}", item.AmountTotal)) %>
            </td>
            <td>
                <%= Html.Encode(String.Format("{0:F}", item.Total)) %>
            </td>
            <td>
                <%= Html.Encode(String.Format("{0:F}", item.TotalPaid)) %>
            </td>
            <td>
                <%= Html.Encode(String.Format("{0:F}", item.TotalPaidByCheck)) %>
            </td>
            <td>
                <%= Html.Encode(String.Format("{0:F}", item.TotalPaidByCredit)) %>
            </td>
            <td>
                <%= Html.Encode(item.Paid) %>
            </td>
            <td>
                <%= Html.Encode(item.Id) %>
            </td>
        </tr>
    
    <% } %>

    </table>

    <p>
        <%= Html.ActionLink("Create New", "Create") %>
    </p>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="PageHeader" runat="server">
</asp:Content>

