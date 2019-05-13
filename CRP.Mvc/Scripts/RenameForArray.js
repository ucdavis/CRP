function RenameControls($container, namePrefix, containerType) {
    var name = namePrefix;

    var masterIndex = 0;

    // go through each container passed
    $container.each(function (cIndex, cItem) {
        var spans = $(cItem).find(containerType);

        // iterate through the paragraphs
        spans.each(function (index, item) {
            // construct the new name
            var cName = name + "[" + masterIndex + "]";

            var tControls = $(item).find(".indexedControl");

            // iterate through each control inside each paragraph
            $.each(tControls, function (index2, item2) {
                // pull the last part of the name out
                var charIndex = ($(item2).attr("id")).indexOf("_");
                var nameEnd = ($(item2).attr("id")).substring(charIndex + 1);
                
                if (item2.tagName === "LABEL") {
                    $(item2).attr("for", cName + "_" + nameEnd);
                }
                else {
                    $(item2).attr("id", cName + "_" + nameEnd);
                    $(item2).attr("name", cName + "." + nameEnd);
                }
            });

            masterIndex++;
        }); // end of paragraph     
    });          // end of container
}