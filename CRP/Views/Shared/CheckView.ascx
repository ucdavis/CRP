<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PaymentLog>" %>

<img src="<%= Url.Content("~/Images/red_close.jpg") %>" class="deactivate-check" style="height:20px; width:20px;" />

<%= Html.Hidden("Id", Model.Id) %>
<%= Html.Hidden("Accepted", Model.Accepted, new {@class="accepted-field"}) %>
<ul>
<% if (Model.DisplayCheckInvalidMessage) {%>
<li>*There is a problem with this check</li>
<%} %>
<li>
    <label for="Name">Payee:</label><br />
    <%= Html.TextBox("Name", Model.Name) %>
    <%= Html.ValidationMessage("Name", "*")%>
</li>
<li>
    <label for="CheckNumber">CheckNumber:</label><br />
    <%= Html.TextBox("CheckNumber", Model.CheckNumber.HasValue ? Model.CheckNumber.Value.ToString() : string.Empty) %>
    <%= Html.ValidationMessage("CheckNumber", "*") %>
</li>
<li>
    <label for="Amount">Amount:</label><br />
    <%= Html.TextBox("Amount", Model.Amount != 0 ? string.Format("{0:0.00}", Model.Amount) : string.Empty, new {@class="amount"}) %>
    <%= Html.ValidationMessage("Amount", "*") %>
</li>
<li>
    <label for="DatePayment">Date Payment:</label><br />
    <%= Html.TextBox("DatePayment", String.Format("{0:d}", Model.DatePayment), new {@class="date"}) %>
    <%= Html.ValidationMessage("DatePayment", "*")%>
</li>
<li>
    <label for="Notes">Notes:</label><br />
    <%= Html.TextArea("Notes", Model.Notes) %>
    <%= Html.ValidationMessage("Notes", "*") %>
</li>
</ul>