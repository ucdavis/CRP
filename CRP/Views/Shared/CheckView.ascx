<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Check>" %>

<%= Html.Hidden("Id", Model.Id) %>

<p>
    <label for="Payee">Payee:</label>
    <%= Html.TextBox("Payee", Model.Payee) %>
    <%= Html.ValidationMessage("Payee", "*") %>
</p>
<p>
    <label for="CheckNumber">CheckNumber:</label>
    <%= Html.TextBox("CheckNumber", Model.CheckNumber != 0 ? Model.CheckNumber.ToString() : string.Empty) %>
    <%= Html.ValidationMessage("CheckNumber", "*") %>
</p>
<p>
    <label for="Amount">Amount:</label>
    <%= Html.TextBox("Amount", Model.Amount != 0 ? string.Format("{0:0.00}", Model.Amount) : string.Empty, new {@class="amount"}) %>
    <%= Html.ValidationMessage("Amount", "*") %>
</p>
<p>
    <label for="DateReceived">DateReceived:</label>
    <%= Html.TextBox("DateReceived", String.Format("{0:d}", Model.DateReceived), new {@class="date"}) %>
    <%= Html.ValidationMessage("DateReceived", "*") %>
</p>
<p>
    <label for="Notes">Notes:</label>
    <%= Html.TextArea("Notes", Model.Notes) %>
    <%= Html.ValidationMessage("Notes", "*") %>
</p>