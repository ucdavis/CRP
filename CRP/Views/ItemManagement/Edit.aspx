<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.ItemViewModel>" %>
<%@ Import Namespace="CRP.Controllers"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Edit
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
    <script type="text/javascript">
        var getExtendedPropertyUrl = '<%= Url.Action("GetExtendedProperties", "ItemManagement") %>';
        var saveTemplateUrl = '<%= Url.Action("SaveTemplate", "ItemManagement") %>';
        var id = '<%= Html.Encode(Model.Item.Id) %>';
        
        $(function() { $("#tabs").tabs(); });
    </script>

    <script src="../../Scripts/ItemEdit.js" type="text/javascript"></script>
    <script src="../../Scripts/tiny_mce/jquery.tinymce.js" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Edit</h2>

    <div id="tabs">
    
        <ul>
            <li><a href="#tabs-1">Item Details</a></li>
            <li><a href="#tabs-2">Editors</a></li>
            <li><a href="#tabs-3">Questions</a></li>
            <li><a href="#tabs-4">Confirmation Template</a></li>
        </ul>
    
        <div id="tabs-1">
        
            <% using (Html.BeginForm("Edit", "ItemManagement", FormMethod.Post, new { @enctype = "multipart/form-data" }))
               {%>

            <%
                Html.RenderPartial("~/Views/Shared/ItemForm.ascx");%>

            <% }%>
        
        </div>
        <div id="tabs-2">
        
            <% using(Html.BeginForm("AddEditor", "ItemManagement", FormMethod.Post)){%>
                <%= Html.AntiForgeryToken() %>
                <%= Html.Hidden("id") %>
                Add User: <%= this.Select("userId").Options(Model.Users.OrderBy(a => a.LastName), x=>x.Id, x=>x.SortableName).FirstOption("--Select a User--") %>
                <input type="submit" value="Add User" />
            <%} %>
            <% Html.Grid(Model.Item.Editors)
                   .Transactional()
                   .Name("Editors")
                   .PrefixUrlParameters(false)
                   .Columns(col =>
                                {
                                    col.Add(a =>
                                                {%>
                                                
                                                    <% using (Html.BeginForm("RemoveEditor", "ItemManagement", FormMethod.Post, new {id="RemoveForm"})) {%>
                                                        
                                                        <%= Html.AntiForgeryToken() %>
                                                        <%= Html.Hidden("id") %>
                                                        <%= Html.Hidden("editorId", a.Id) %>
                                                    
                                                        <a href="javascript:;" class="FormSubmit">Delete</a>
                                                    
                                                    <%} %>
                                                
                                                <%});
                                    col.Add(a => a.User.FullName);
                                    col.Add(a => a.Owner);
                                })
                   .Render(); %>
        </div>
        <div id="tabs-3">
            <fieldset>
                <legend>Transaction</legend>
                
                <p>
                    <%= Html.ActionLink<QuestionSetController>(a => a.LinkToItem(Model.Item.Id, true, false), "Add Question Set") %>
                </p>
                
                <% Html.Grid(Model.Item.QuestionSets.Where(a => a.TransactionLevel).OrderBy(a => a.Order))
                       .Transactional()
                       .Name("TransactionLevelQuestionSets")
                       .PrefixUrlParameters(false)
                       .Columns(col =>
                                    {
                                        col.Add(a =>
                                                    {%>
                                                        <% using(Html.BeginForm<QuestionSetController>(b => b.UnlinkFromItem(a.Id))) {%>
                                                            <%= Html.AntiForgeryToken() %>
                                                            <a href="javascript:;" class="FormSubmit">Delete</a>
                                                        
                                                        <%} %>
                                                    <%});
                                        col.Add(a => a.QuestionSet.Name);
                                        col.Add(a => a.Required);
                                        col.Add(a => a.QuestionSet.Questions.Count()).Title("# of Questions");
                                    })
                       .Render();
                        %>
                
            </fieldset>
            
            <fieldset>
                <legend>Quantity</legend>
                
                <p>
                    <%= Html.ActionLink<QuestionSetController>(a => a.LinkToItem(Model.Item.Id, false, true), "Add Question Set") %>
                </p>
                
                <% Html.Grid(Model.Item.QuestionSets.Where(a => a.QuantityLevel).OrderBy(a => a.Order))
                       .Transactional()
                       .Name("QuantityLevelQuestionSets")
                       .PrefixUrlParameters(false)
                       .Columns(col =>
                                    {
                                        col.Add(a =>
                                                    {%>
                                                        <% using(Html.BeginForm<QuestionSetController>(b => b.UnlinkFromItem(a.Id))) {%>
                                                            <%= Html.AntiForgeryToken() %>
                                                            <a href="javascript:;" class="FormSubmit">Delete</a>
                                                        <%} %>  
                                                    <%});
                                        col.Add(a => a.QuestionSet.Name);
                                        col.Add(a => a.Required);
                                        col.Add(a => a.QuestionSet.Questions.Count()).Title("# of Questions");
                                    })
                       .Render();
                        %>
                
            </fieldset>
        </div>
        <div id="tabs-4">
        
            <p>
                <%= Html.TextArea("BodyText", Model.Item.Template != null ? Model.Item.Template.Text : string.Empty) %>
            </p>
        
        </div>
    
    </div>


    
    <div>
        <%=Html.ActionLink<ItemManagementController>(a => a.List(), "Back to List") %>
    </div>

</asp:Content>


