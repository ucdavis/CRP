function RenameControls($container, namePrefix) {
    var name = namePrefix;

    var masterIndex = 0;

    // go through each container passed
    $.each($container, function(cIndex, cItem) {
        // get the paragraph tags that each contain a question
        var spans = $(cItem).find("span");

        // iterate through the paragraphs
        $.each(spans, function(index, item) {
            // construct the new name
            var cName = name + "[" + masterIndex + "]";

            var tControls = $(item).find(".indexedControl");

            // iterate through each control inside each paragraph
            $.each(tControls, function(index2, item2) {
                // pull the last part of the name out
                var charIndex = ($(item2).attr("id")).indexOf("_");
                var nameEnd = ($(item2).attr("id")).substring(charIndex + 1);

                if (item2.tagName == "LABEL") {
                    $(item2).attr("for", cName + "_" + nameEnd);
                }
                else {
                    $(item2).attr("id", cName + "_" + nameEnd);
                    $(item2).attr("name", cName + "." + nameEnd);
                }
            });

            masterIndex++;
        }); // end of paragraph     
    });       // end of container
}