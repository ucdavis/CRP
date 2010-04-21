<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Core.Domain.ItemType>" %>
<%@ Import Namespace="CRP.Core.Resources"%>

<%@ Import Namespace="CRP.Controllers"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	EditItemType
</asp:Content>

<asp:Content ID="pageHeader" ContentPlaceHolderID="PageHeader" runat="server">
    <% Html.RenderPartial(StaticValues.Partial_PageHeader, new DisplayProfile()); %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>EditItemType</h2>

    <%= Html.ValidationSummary("Edit was unsuccessful. Please correct the errors and try again.") %>

    <%--Don't really need a form but it's throwing a wierd error--%>
    <% using(Html.BeginForm()) { %>

        <fieldset>
            <legend>Name</legend>
            
            <% using (Html.BeginForm()) {%>
            
                <%= Html.AntiForgeryToken() %>
                <%= Html.ClientSideValidation<ItemType>("") %>
            
            <p>
                <%= Html.TextBox("Name", Model.Name) %>
                <%= Html.ValidationMessage("Name", "*") %>
            </p>
            
            <p>
                <input type="submit" value="Save" />
            </p>

        <% } %>
            
        </fieldset>
        <fieldset>
            <legend>Extended Properties</legend>
            <p>
            <%= Html.ActionLink<ExtendedPropertyController>(a => a.Create(Model.Id), "Create") %>
            </p>
            
            <%  Html.Grid(Model.ExtendedProperties)
                    .Transactional()
                    .Name("ExtendedProperties")
                    .PrefixUrlParameters(false)
                    .Columns(col =>
                                 {
                                     col.Add(a => { %> 
                                        
                                            <% using (Html.BeginForm<ExtendedPropertyController>(b => b.Delete(a.Id), FormMethod.Post)) { %>
                                                <%= Html.AntiForgeryToken() %>
                                                <a href="javascript:;" class="FormSubmit">Delete</a>
                                            
                                            <% } %>
                                        
                                        <% });
                                     col.Add(a => a.Name).Title("Property Name");
                                     col.Add(a => a.QuestionType.Name).Title("Question Type");
                                 })
                    .Render();
           %>
        </fieldset> 
       
        <fieldset>
            <legend>Transaction Question Sets</legend>
            
            <p>
                <%= Html.ActionLink<QuestionSetController>(a => a.LinkToItemType(Model.Id, true, false), "Add Question Set") %>
            </p>
            
            

            <% Html.Grid(Model.QuestionSets.Where(a => a.TransactionLevel))
                   .Transactional()
                   .Name("TransactionQuestionSets")
                   .PrefixUrlParameters(false)
                   .Columns(col =>
                                {
                                    col.Add(a =>
                                                {%>
                                                <%= Html.ActionLink<QuestionSetController>(b => b.Edit(a.QuestionSet.Id), "Edit")%>
                                                <% });
                                    col.Add(a => a.QuestionSet.Name).Title("Question Set Name");
                                    col.Add(a =>
                                                {%> 
                                                
                                                <%= Html.Encode("List of Questions") %> 
                                                <% });
                                        
                                })
                    .Render();
                    %>       
        </fieldset>

        <fieldset>
            <legend>Quantity Question Sets</legend>
            
            <p>
                <%= Html.ActionLink<QuestionSetController>(a => a.LinkToItemType(Model.Id, false, true), "Add Question Set") %>
            </p>
            
            <% Html.Grid(Model.QuestionSets.Where(a => a.QuantityLevel))
                   .Transactional()
                   .Name("QuantityQuestionSets")
                   .PrefixUrlParameters(false)
                   .Columns(col =>
                                {
                                    col.Add(a =>
                                                {%>
                                                <%= Html.ActionLink<QuestionSetController>(b => b.Edit(a.QuestionSet.Id), "Edit") %>
                                                <% });
                                    col.Add(a => a.QuestionSet.Name).Title("Question Set Name");
                                    col.Add(a =>
                                                {%> 
                                                
                                                <%= Html.Encode("List of Questions") %> 
                                                <% });
                                        
                                })
                    .Render();
                    %>
            
        </fieldset>
    <% } %>        
        
    <div>
        <%= Html.ActionLink<ApplicationManagementController>(a => a.ListItemTypes(), "Back to List") %>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function() {
            $("a.FormSubmit").click(function() { $(this).parents("form").submit(); });
        });
    </script>
</asp:Content>

