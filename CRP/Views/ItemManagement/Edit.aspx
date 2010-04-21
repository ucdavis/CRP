<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.ItemViewModel>" %>
<%@ Import Namespace="CRP.Core.Resources"%>
<%@ Import Namespace="CRP.Controllers"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Edit
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="PageHeader" runat="server">

    <% Html.RenderPartial(StaticValues.Partial_PageHeader, new DisplayProfile()); %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
    <script type="text/javascript">
        var getExtendedPropertyUrl = '<%= Url.Action("GetExtendedProperties", "ItemManagement") %>';
        var saveTemplateUrl = '<%= Url.Action("SaveTemplate", "ItemManagement") %>';
        var id = '<%= Html.Encode(Model.Item.Id) %>';
        var scriptUrl = '<%= Url.Content("~/Scripts/tiny_mce/tiny_mce.js") %>';
        
        $(function() { $("#tabs").tabs(); });
    </script>

    <script src="<%= Url.Content("~/Scripts/ItemEdit.js") %>" type="text/javascript"></script>
    <script src="<%= Url.Content("~/Scripts/tiny_mce/jquery.tinymce.js") %>" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Edit</h2>

    <div id="tabs">
    
        <ul>
            <li><a href="#<%= StaticValues.Tab_Details %>">Item Details</a></li>
            <li><a href="#<%= StaticValues.Tab_Editors %>">Editors</a></li>
            <li><a href="#<%= StaticValues.Tab_Questions %>">Questions</a></li>
            <li><a href="#<%= StaticValues.Tab_Templates %>">Confirmation Template</a></li>
            <li><a href="#<%= StaticValues.Tab_Coupons %>">Coupons</a></li>
        </ul>
    
        <div id="<%= StaticValues.Tab_Details %>">
        
            <% using (Html.BeginForm("Edit", "ItemManagement", FormMethod.Post, new { @enctype = "multipart/form-data" }))
               {%>

            <%
                Html.RenderPartial(StaticValues.Partial_ItemForm);%>

            <% }%>
        
        </div>
        <div id="<%= StaticValues.Tab_Editors %>">
        
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
        <div id="<%= StaticValues.Tab_Questions %>">
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
                                                        
                                                        <% if (!a.QuestionSet.SystemReusable || !a.QuestionSet.CollegeReusable || !a.QuestionSet.UserReusable) { %>
                                                            | 
                                                            <%= Html.ActionLink<QuestionSetController>(b => b.Edit(a.QuestionSet.Id), "Edit") %>
                                                        <% } %>
                                                    <%});
                                        col.Add(a => a.QuestionSet.Name);
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
                                                        
                                                        <% if (!a.QuestionSet.SystemReusable || !a.QuestionSet.CollegeReusable || !a.QuestionSet.UserReusable) { %>
                                                            | 
                                                            <%= Html.ActionLink<QuestionSetController>(b => b.Edit(a.QuestionSet.Id), "Edit") %>
                                                        <% } %>
                                                    <%});
                                        col.Add(a => a.QuestionSet.Name);
                                        col.Add(a => a.QuestionSet.Questions.Count()).Title("# of Questions");
                                    })
                       .Render();
                        %>
                
            </fieldset>
        </div>
        <div id="<%= StaticValues.Tab_Templates %>">
            <% Html.RenderPartial(StaticValues.View_TemplateInstructions);%>
            <p>
                <%= Html.TextArea("BodyText", Model.Item.Template != null ? Model.Item.Template.Text : string.Empty) %>
            </p>
        
        </div>
        <div id="<%= StaticValues.Tab_Coupons %>">
            
            <p>
                <%= Html.ActionLink<CouponController>(a => a.Create(Model.Item.Id), "Generate Coupon") %>
            </p>
            
            <% Html.Grid(Model.Item.Coupons)
                   .Transactional()
                   .Name("Coupons")
                   .PrefixUrlParameters(false)
                   .Columns(col =>
                                {
                                    col.Add(a =>
                                                {%>
                                                    <% if (a.IsActive) { %> 
                                                        <% using (Html.BeginForm<CouponController>(b => b.Deactivate(a.Id), FormMethod.Post)) { %> 
                                                            <%= Html.AntiForgeryToken() %>
                                                            <a href="javascript:;" class="FormSubmit">Deactivate</a>    
                                                        <% } %>
                                                    <% } %>
                                                <%});
                                    col.Add(a => a.Code);
                                    col.Add(a => a.DiscountAmount.ToString("C")).Title("Discount Amount");
                                    col.Add(a => a.Email);
                                    col.Add(a => a.Expiration.HasValue ? a.Expiration.Value.ToString("d"): string.Empty).Title("Expiration");
                                    col.Add(a => a.Used);
                                    col.Add(a => a.Unlimited);
                                    col.Add(a => a.MaxQuantity);
                                    col.Add(a => a.IsActive);
                                })
                   .Render();
                    %>
            
        </div>
    
    </div>


    
    <div>
        <%=Html.ActionLink<ItemManagementController>(a => a.List(), "Back to List") %>
    </div>

</asp:Content>


