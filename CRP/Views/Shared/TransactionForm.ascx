<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ItemTransactionViewModel>" %>
<%@ Import Namespace="CRP.Controllers.ViewModels"%>
<%@ Import Namespace="CRP.Core.Resources"%>

<div class="two_col_float">
<h2 class="two_col_float_for_h2">Transaction Level</h2>

<div id="TransactionContainer">

<% foreach(var qs in Model.Item.QuestionSets.Where(a => a.TransactionLevel).OrderBy(a => a.Order)) {%>
    <fieldset id='<%= Html.Encode(qs.Id) %>'>
        <legend><%= Html.Encode(qs.QuestionSet.Name) %></legend>
        
        <ul>
        <% foreach (var q in qs.QuestionSet.Questions) {
                var answer = Model.Answers.Where(a => a.Transaction && a.QuestionSetId == q.QuestionSet.Id && a.QuestionId == q.Id).FirstOrDefault();
               %>
        
            <% Html.RenderPartial(StaticValues.Partial_QuestionForm, new ItemQuestionViewModel(q, Model.OpenIDUser, answer != null ? answer.Answer : string.Empty)); %>
        
        <% } %></ul>
        
    </fieldset>
<% } %>

</div>
</div>

<div class="two_col_float two_col_float_right">
<h2>Quantity Level</h2>
<p>
    *The following questions must be answered for each of the <%= !String.IsNullOrEmpty(Model.Item.QuantityName) ? Html.Encode(Model.Item.QuantityName) : Html.Encode(ScreenText.STR_QuantityName)%>
</p>


    <% for (var i = 0; i < Model.Quantity; i++ ) { %>
        <% foreach (var qs in Model.Item.QuestionSets.Where(a => a.QuantityLevel).OrderBy(a => a.Order))
           { %> 
<div class="QuantityContainer">
                <fieldset>
                <legend><%= Html.Encode(qs.QuestionSet.Name)%> <span class="quantityIndex"><%= Html.Encode(i + 1) %></span> </legend>
                <ul>
                <% foreach (var q in qs.QuestionSet.Questions) {
                    var answer = Model.Answers.Where(a => !a.Transaction && a.QuestionSetId == q.QuestionSet.Id && a.QuestionId == q.Id && a.QuantityIndex == i).FirstOrDefault();
                   %>
                
                    <% Html.RenderPartial(StaticValues.Partial_QuestionForm, new ItemQuestionViewModel(q, Model.OpenIDUser, answer != null ? answer.Answer : string.Empty)); %>
                
                <% } %>
            </ul>    
            </fieldset>
</div>

        <% } %> <!-- End of foreach loop -->
    <% } %> <!-- End of for loop -->
</div>
<div style="clear:both;">&nbsp</div>