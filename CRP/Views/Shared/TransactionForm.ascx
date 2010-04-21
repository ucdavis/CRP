<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CRP.Core.Domain.Item>" %>
<%@ Import Namespace="CRP.Core.Resources"%>

<h2>Transaction Level</h2>

<div id="TransactionContainer">

<% foreach(var qs in Model.QuestionSets.Where(a => a.TransactionLevel).OrderBy(a => a.Order)) {%>
    <fieldset id='<%= Html.Encode(qs.Id) %>'>
        <legend><%= Html.Encode(qs.QuestionSet.Name) %></legend>
        
        <% foreach (var q in qs.QuestionSet.Questions) {%>
        
            <% Html.RenderPartial("~/Views/Shared/QuestionForm.ascx", q); %>
        
        <% } %>
        
    </fieldset>
<% } %>

</div>


<h2>Quantity Level</h2>
<p>
    *The following questions must be answered for each of the <%= !String.IsNullOrEmpty(Model.QuantityName) ? Html.Encode(Model.QuantityName) : Html.Encode(ScreenText.STR_QuantityName) %>
</p>

<div class="QuantityContainer">
    <% foreach(var qs in Model.QuestionSets.Where(a => a.QuantityLevel).OrderBy(a => a.Order)) { %> 

            <fieldset>
            <legend><%= Html.Encode(qs.QuestionSet.Name) %> <span class="quantityIndex">1</span> </legend>
            
            <% foreach (var q in qs.QuestionSet.Questions) {%>
            
                <% Html.RenderPartial("~/Views/Shared/QuestionForm.ascx", q); %>
            
            <% } %>
            
        </fieldset>

    <% } %>
</div>