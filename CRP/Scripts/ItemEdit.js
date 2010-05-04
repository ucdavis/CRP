$(document).ready(function() {
    $("input#Item_Expiration").datepicker();

    $("select#Item_ItemType").change(function(event) {
        var url = getExtendedPropertyUrl;
        $.getJSON(url + '/' + $(this).val(), {},
                    function(result) {

                        var length = result.length;

                        if (length > 0) {
                            $.each(result, function(index, item) {

                                var name = item.Name.replace(/ /g, "");

                                // create the label
                                var label = $("<label>").attr("for", "item." + name).html(item.Name);
                                var textBox = $("<input>").attr("id", "ExtendedProperties[" + index + "]_value")
                                                          .attr("name", "ExtendedProperties[" + index + "].value")
                                                          .attr("type", "text");
                                // create hidden field to store the extended property id
                                var hidden = $("<input>").attr("type", "hidden")
                                                         .attr("id", "ExtendedProperties[" + index + "]_propertyId")
                                                         .attr("name", "ExtendedProperties[" + index + "].propertyId")
                                                         .val(item.Id);

                                if (item.QuestionType.Name == "Date") {
                                    //textBox.datepicker().watermark("mm/dd/yyyy", { className: "watermark" });
                                    textBox.datepicker().bt('mm/dd/yyyy format');                                    
                                }

                                var p = $("<p>").append(label).append(textBox).append(hidden);
                                $("div#ExtendedProperties").append(p);
                            });
                        }
                    });
    });

    $("img#tagAddButton").click(function(event) {
        var input = $("<input>").attr("id", "tags").attr("name", "tags").val($("input#tagInput").val());
        input.attr("type", "text");
        input.css("cursor", "pointer");
        input.click(function(event) { $(this).remove(); });
        $("div#tagContainer").append(input);

        // blank the input
        $("input#tagInput").val("");
    });

    $("input#tags").click(function(event) {
        $(this).remove();
    });

    $("a.FormSubmit").click(function(event) {
        //$(this).parents("form#RemoveForm").submit();

        $(this).parent().submit();
    });

    $("textarea#BodyText").tinymce({
        script_url: scriptUrl,
        // General options
        theme: "advanced",
        plugins: "safari,style,save,searchreplace,print,contextmenu,paste",

        // Theme options
        theme_advanced_buttons1: "save,print,|,bold,italic,underline,|,styleselect,formatselect,fontselect,fontsizeselect",
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
        media_external_list_url: "js/media_list.js",

        save_onsavecallback: function() {
            var textbox = $(this);
            var token = $($("input:hidden[name='__RequestVerificationToken']")[0]).val();

            $.post(saveTemplateUrl, { id: id, text: textbox.val(), __RequestVerificationToken: token }
                , function(result) { if (result) { alert("template saved."); } else { alert("template was unable to save."); } });
        }
    });

    $("textarea#Item_Description").tinymce({
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

    $("textarea#Item_CheckPaymentInstructions").tinymce({
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
        theme_advanced_resizing: false,

        // dimensions stuff
        height: "225",

        // Example content CSS (should be your site CSS)
        //content_css: "css/Main.css",

        // Drop lists for link/image/media/template dialogs
        template_external_list_url: "js/template_list.js",
        external_link_list_url: "js/link_list.js",
        external_image_list_url: "js/image_list.js",
        media_external_list_url: "js/media_list.js"
    });
});