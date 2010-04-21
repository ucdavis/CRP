<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Core.Domain.OpenIdUser>" %>
<%@ Import Namespace="CRP.Controllers"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	OpenIdAccount
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>OpenIdAccount</h2>

    <%= Html.ValidationSummary("Edit was unsuccessful. Please correct the errors and try again.") %>

    <% using (Html.BeginForm()) {%>
<%= Html.AntiForgeryToken() %>
        <fieldset>
            <legend>Fields</legend>
            <p>
                <label for="Email">Email:</label>
                <%= Html.TextBox("Email", Model.Email) %>
                <%= Html.ValidationMessage("Email", "*") %>
            </p>
            <p>
                <label for="FirstName">FirstName:</label>
                <%= Html.TextBox("FirstName", Model.FirstName) %>
                <%= Html.ValidationMessage("FirstName", "*") %>
            </p>
            <p>
                <label for="LastName">LastName:</label>
                <%= Html.TextBox("LastName", Model.LastName) %>
                <%= Html.ValidationMessage("LastName", "*") %>
            </p>
            <p>
                <label for="StreetAddress">StreetAddress:</label>
                <%= Html.TextBox("StreetAddress", Model.StreetAddress) %>
                <%= Html.ValidationMessage("StreetAddress", "*") %>
            </p>
            <p>
                <label for="Address2">Address2:</label>
                <%= Html.TextBox("Address2", Model.Address2) %>
                <%= Html.ValidationMessage("Address2", "*") %>
            </p>
            <p>
                <label for="City">City:</label>
                <%= Html.TextBox("City", Model.City) %>
                <%= Html.ValidationMessage("City", "*") %>
            </p>
            <p>
                <label for="State">State:</label>
                <%= Html.TextBox("State", Model.State) %>
                <%= Html.ValidationMessage("State", "*") %>
            </p>
            <p>
                <label for="Zip">Zip:</label>
                <%= Html.TextBox("Zip", Model.Zip) %>
                <%= Html.ValidationMessage("Zip", "*") %>
            </p>
            <p>
                <label for="PhoneNumber">PhoneNumber:</label>
                <%= Html.TextBox("PhoneNumber", Model.PhoneNumber) %>
                <%= Html.ValidationMessage("PhoneNumber", "*") %>
            </p>
            <p>
                <input type="submit" value="Save" />
            </p>
        </fieldset>

    <% } %>

    <div>
        <%= Html.ActionLink<HomeController>(a => a.Index(), "Back Home") %>
    </div>
    
    <fieldset>
        <legend>Transactions</legend>
        
        <% Html.Grid(Model.Transactions.Where(a => a.ParentTransaction == null))
               .Transactional()
               .PrefixUrlParameters(false)
               .Name("Transactions")
               .Columns(col =>
                            {
                                col.Add(a => a.TransactionNumber);
                                col.Add(a => a.TransactionDate);
                                col.Add(a => a.Quantity);
                                col.Add(a => a.AmountTotal.ToString("C")).Title("Amount");
                                col.Add(a => a.DonationTotal.ToString("C")).Title("Donation");
                                col.Add(a => a.Total.ToString("C")).Title("Total");
                            })
               .Render(); %>
    </fieldset>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="PageHeader" runat="server">
</asp:Content>

