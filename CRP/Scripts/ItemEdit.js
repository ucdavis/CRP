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
                                    textBox.datepicker();
                                }

                                var p = $("<p>").append(label).append(textBox).append(hidden);
                                $("div#ExtendedProperties").append(p);
                            });
                        }
                    });
    });

    $("img#tagAddButton").click(function(event) {
        var input = $("<input>").attr("id", "tags").attr("name", "tags").val($("input#tagInput").val());
        $("div#tagContainer").append(input);

        // blank the input
        $("input#tagInput").val("");
    });

    $("input#tags").click(function(event) {
        $(this).remove();
    });

    $("a.FormSubmit").click(function(event) {
        $(this).parents("form#RemoveForm").submit();
    });
});