<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PaymentLog>" %>

<img src="<%= Url.Content("~/Images/red_close.jpg") %>" class="deactivate-check" style="height:20px; width:20px;" />

<%= Html.Hidden("Id", Model.Id) %>
<%= Html.Hidden("Accepted", Model.Accepted, new {@class="accepted-field"}) %>
<p>
    <label for="Name">Payee:</label>
    <%= Html.TextBox("Name", Model.Name) %>
    <%= Html.ValidationMessage("Name", "*")%>
</p>
<p>
    <label for="CheckNumber">CheckNumber:</label>
    <%= Html.TextBox("CheckNumber", Model.CheckNumber.HasValue ? Model.CheckNumber.Value.ToString() : string.Empty) %>
    <%= Html.ValidationMessage("CheckNumber", "*") %>
</p>
<p>
    <label for="Amount">Amount:</label>
    <%= Html.TextBox("Amount", Model.Amount != 0 ? string.Format("{0:0.00}", Model.Amount) : string.Empty, new {@class="amount"}) %>
    <%= Html.ValidationMessage("Amount", "*") %>
</p>
<p>
    <label for="DatePayment">Date Payment:</label>
    <%= Html.TextBox("DatePayment", String.Format("{0:d}", Model.DatePayment), new {@class="date"}) %>
    <%= Html.ValidationMessage("DatePayment", "*")%>
</p>
<p>
    <label for="Notes">Notes:</label>
    <%= Html.TextArea("Notes", Model.Notes) %>
    <%= Html.ValidationMessage("Notes", "*") %>
</p>