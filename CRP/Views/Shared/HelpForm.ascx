<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CRP.Core.Domain.HelpTopic>" %>

    <script src='<%= Url.Content("~/Scripts/tiny_mce/jquery.tinymce.js") %>' type="text/javascript"></script>
    <script src="<%= Url.Content("~/Scripts/jquery.enableTinyMce.js") %>" type="text/javascript"></script>

    <script type="text/javascript">
        $(document).ready(function() {
            $("#Answer").enableTinyMce({ script_location: '<%= Url.Content("~/Scripts/tiny_mce/tiny_mce.js") %>', overRideHeigth: '225', overRideWidth: '925' });
        });
   </script>
<%--	<script type="text/javascript">
	    $(document).ready(function() {
	        var scriptUrl = '<%= Url.Content("~/Scripts/tiny_mce/tiny_mce.js") %>';
	        $("textarea#Answer").tinymce({
	            script_url: scriptUrl,
	            // General options
	            theme: "advanced",
	            plugins: "safari,style,save,searchreplace,print,contextmenu,paste",

	            // Theme options
	            theme_advanced_buttons1: "print,|,bold,italic,underline,|,styleselect,formatselect,fontselect,fontsizeselect",
	            theme_advanced_buttons2: "cut,copy,paste,pastetext,pasteword,|,search,replace,|,undo,redo",
	            theme_advanced_buttons3: "",
	            theme_advanced_toolbar_location: "top",
	            theme_advanced_toolbar_align: "left",
	            theme_advanced_statusbar_location: "bottom",
	            theme_advanced_resizing: true,

	            // dimensions stuff
	            height: "225",
	            width: "925",

	            // Example content CSS (should be your site CSS)
	            //content_css: "css/Main.css",

	            // Drop lists for link/image/media/template dialogs
	            template_external_list_url: "js/template_list.js",
	            external_link_list_url: "js/link_list.js",
	            external_image_list_url: "js/image_list.js",
	            media_external_list_url: "js/media_list.js"
	        });
	        $("input[name*=VideoName]").bt('Only name, no extension or path. <br>Video must already exist on hosted server.');
	    });
	</script>--%>

    <%= Html.ValidationSummary("Save was unsuccessful. Please correct the errors and try again.") %>

    
        <%= Html.AntiForgeryToken() %>
        <%= Html.ClientSideValidation<HelpTopic>("") %>
        
        <fieldset>
            <legend>Fields</legend>
            <ul>
            <li>
                <label for="Question">Frequently Asked Question:</label>
                <%= Html.TextBox("Question")%>
                <%= Html.ValidationMessage("Question", "*")%>
            </li>
            <li>
                <label for="IsActive">Is Active: </label>
                <%= Html.CheckBox("IsActive", Model != null ? Model.IsActive : false)%>
                <%= Html.ValidationMessage("IsActive")%>
            </li>
            <li>
                <label for="AvailableToPublic">Available To Public:</label>
                <%= Html.CheckBox("AvailableToPublic", Model != null ? Model.AvailableToPublic : false) %>
                <%= Html.ValidationMessage("AvailableToPublic") %>
            </li>
            <li>
                <label for="IsVideo">Video:</label>
                <%= Html.CheckBox("IsVideo", Model != null ? Model.IsVideo : false)%>
                <%= Html.ValidationMessage("IsVideo")%>
            </li>
            <li>
                <label for="VideoName">Name of video:</label>
                <%= Html.TextBox("VideoName")%>
                <%= Html.ValidationMessage("VideoName")%>
            </li>
            <li>
                <label for="NumberOfReads">Number Of Reads:</label>
                <%= Html.TextBox("NumberOfReads")%>
                <%= Html.ValidationMessage("NumberOfReads")%>
            </li>
            <li>
                <label for="Answer">Answer:</label>
                <%= Html.TextArea("Answer")%>
                <%= Html.ValidationMessage("Answer", "*")%>
            </li>
            </ul>
            <p>
                <input type="submit" value="Save" />
            </p>
        </fieldset>



    <div>
        <%=Html.ActionLink("Back to List", "Index") %>
    </div>