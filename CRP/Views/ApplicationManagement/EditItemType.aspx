<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Core.Domain.ItemType>" %>
<%@ Import Namespace="CRP.Controllers"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	EditItemType
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>EditItemType</h2>

    <%= Html.ValidationSummary("Edit was unsuccessful. Please correct the errors and try again.") %>

    <% using (Html.BeginForm()) {%>

        <fieldset>
            <legend>Name</legend>
            <p>
                <%= Html.TextBox("Name", Model.Name) %>
                <%= Html.ValidationMessage("Name", "*") %>
            </p>
            
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
                                     col.Add(a => { %> <%= Html.Encode("Delete") %> <% });
                                     col.Add(a => a.Name).Title("Property Name");
                                     col.Add(a => a.QuestionType.Name).Title("Question Type");
                                 })
                    .Render();
           %>
        </fieldset>
        
        <fieldset>
            <legend>Default Question Sets</legend>
            
            <p>
            //TODO: Add a link for adding/creating question set
            </p>
            
            <% Html.Grid(Model.QuestionSets)
                   .Transactional()
                   .Name("QuestionSets")
                   .PrefixUrlParameters(false)
                   .Columns(col =>
                                {
                                    col.Add(a => a.Name).Title("Question Set Name");
                                    col.Add(a =>
                                                {%> 
                                                
                                                <%= Html.Encode("List of Questions") %> 
                                                <% });
                                        
                                })
                    .Render();
                    %>
            
        </fieldset>

        <p>
            <input type="submit" value="Save" />
        </p>

    <% } %>

    <div>
        <%=Html.ActionLink("Back to List", "Index") %>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

