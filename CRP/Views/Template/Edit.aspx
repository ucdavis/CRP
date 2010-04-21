<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Core.Domain.Template>" %>
<%@ Import Namespace="CRP.Controllers"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Edit
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">

    <script src="<%= Url.Content("~/Scripts/tiny_mce/jquery.tinymce.js") %>" type="text/javascript"></script>

    <script type="text/javascript">

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
    
    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Edit</h2>

    <% using (Html.BeginForm()) { %>
    
        <%= Html.AntiForgeryToken() %>
        <%= Html.Hidden("id", Model.Id) %>
        
        <%= Html.TextArea("Text", Model.Text) %>
        
        <%= Html.SubmitButton("Submit", "Save") %>
    
    <% } %>

    <%= Html.ActionLink<HomeController>(a => a.AdminHome(), "Return Home") %>

</asp:Content>


