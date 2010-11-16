<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.ConfirmationTemplateViewModel>" %>
<%@ Import Namespace="CRP.Core.Resources"%>
<%@ Import Namespace="CRP.Controllers"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Edit
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">

    <script src="<%= Url.Content("~/Scripts/tiny_mce/jquery.tinymce.js") %>" type="text/javascript"></script>

<%--    <script type="text/javascript">

        $(document).ready(function() {
            $("textarea#Text").tinymce({
                script_url: '../../Scripts/tiny_mce/tiny_mce.js',
                // General options
                theme: "advanced",
                plugins: "safari,style,save,searchreplace,print,contextmenu,paste",

                // Theme options
                theme_advanced_buttons1: "print,|,bold,italic,underline,|,styleselect,formatselect,fontselect,fontsizeselect",
                theme_advanced_buttons2: "cut,copy,paste,pastetext,pasteword,|,search,replace,|,undo,redo,|,bullist,numlist",
                theme_advanced_buttons3: "",
                theme_advanced_toolbar_location: "top",
                theme_advanced_toolbar_align: "left",
                theme_advanced_statusbar_location: "bottom",
                theme_advanced_resizing: false,

                // dimensions stuff
                height: "400",

                // Example content CSS (should be your site CSS)
                //content_css: "css/Main.css",

                // Drop lists for link/image/media/template dialogs
                template_external_list_url: "js/template_list.js",
                external_link_list_url: "js/link_list.js",
                external_image_list_url: "js/image_list.js",
                media_external_list_url: "js/media_list.js"

            });
        });
    
    </script>--%>
    
    <script src="<%= Url.Content("~/Scripts/tiny_mce/jquery.tinymce.js") %>" type="text/javascript"></script>
    <script src="<%= Url.Content("~/Scripts/jquery.enableTinyMce.js") %>" type="text/javascript"></script>   

    <script type="text/javascript">
        $(document).ready(function() {
        $("#PaidText").enableTinyMce({ script_location: '<%= Url.Content("~/Scripts/tiny_mce/tiny_mce.js") %>', overrideWidth: '500', overrideHeight: '250' ,overrideShowPreview: 'preview,', overridePlugin_preview_pageurl: '<%= Url.Content("~/Static/Preview.html") %>'});
            $(".add-token").click(function(event) {
                var pasteValue = $(this).attr("name");
                tinyMCE.execInstanceCommand("PaidText", "mceInsertContent", false, pasteValue);
            });
            $("#UnpaidText").enableTinyMce({ script_location: '<%= Url.Content("~/Scripts/tiny_mce/tiny_mce.js") %>', overrideWidth: '500', overrideHeight: '250', overrideShowPreview: 'preview,', overridePlugin_preview_pageurl: '<%= Url.Content("~/Static/Preview.html") %>' });
            $(".add-token-unpaid").click(function(event) {
                var pasteValue = $(this).attr("name");
                tinyMCE.execInstanceCommand("UnpaidText", "mceInsertContent", false, pasteValue);
            });
        });
        

        
   </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Edit System Confirmation Template</h2>
     <%= Html.ValidationSummary() %>

    <% using (Html.BeginForm()) { %>
    
        <%= Html.AntiForgeryToken() %>
        <%= Html.Hidden("id", Model.Template.Id) %>
        
        <% Html.RenderPartial(StaticValues.View_TemplateInstructions);%>
        <h3>Paid Template</h3>
        <%= Html.TextArea("PaidText", Model.PaidText) %><br/>
        <h3>Unpaid Template</h3>
        <%= Html.TextArea("UnpaidText", Model.UnpaidText) %>
        
        <%= Html.SubmitButton("Submit", "Save") %>
    
    <% } %>

    <%= Html.ActionLink<HomeController>(a => a.AdminHome(), "Return Home") %>

</asp:Content>


