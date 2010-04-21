/*
    CaesMutioptionControl
    Author: Alan Lai, CA&ES Dean's Office, UC Davis
    Date: 11/18/2009
    Requirements:
        jquery-1.3.2
        jqueryui-1.7.2
    Description:
        Modifies a control with multiple options into a more user friendly control with larger target areas.
        Essentially converting the controls to buttons for each checkbox/radio button.
*/

(function($) {
    // attach this new method to jQuery
    $.fn.extend({

        // this is the CaesCheckbox code
        CaesMutioptionControl: function(options) {

            // strings to determine the type of the input being entered
            var STR_Checkbox = "checkbox";
            var STR_Radiobutton = "radio";

            // deal with the default values
            var settings = $.extend({
                width: '100px',
                classes: ["button", "ui-corner-all"]
            }, options);

            // iterate over the current set of matched elements
            return this.each(function(index, item) {
                SetControl(item);
            });

            function SetControl(item) {
                // get the options
                var o = settings;

                var originalControl = $(item);
                // find the label that has the for property equal to the id of our checkbox
                // limit the search to the container holding the list
                var label = originalControl.parent("span").find("label.indexedControl");

                // create the visible control users will see
                var caesControl = $("<div>").html(label.html());
                $.each(o.classes, function(index, item) { caesControl.addClass(item); });
                caesControl.css("width", o.width);

                // add the click event so the actual checkbox will still be checked
                SetAction(caesControl, originalControl, originalControl.attr("type").toLowerCase());

                // hide the original controls
                originalControl.hide();
                label.hide();

                // insert the new control for the user
                caesControl.insertAfter(originalControl);
            }

            function SetAction(newControl, originalControl, type) {
                // behavior for a checkbox
                if (type == STR_Checkbox) {
                    newControl.click(function() {
                        if (originalControl.attr("checked")) {
                            newControl.removeClass("selected");
                            originalControl.attr("checked", false);
                        }
                        else {
                            newControl.addClass("selected");
                            originalControl.attr("checked", true);
                        }
                    });
                }
                // behavior for a radio button list
                else if (type == STR_Radiobutton) {
                    newControl.click(function() {
                        newControl.parent("div").find(".button").removeClass("selected");
                        newControl.addClass("selected");
                        originalControl.attr("checked", true);
                    });
                }
            }

        }
    });
})(jQuery);