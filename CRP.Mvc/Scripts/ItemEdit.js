$(function() {
  $("input#Item_Expiration").datepicker();

  $("select#Item_ItemTypeId").change(function(event) {
    var url = window.getExtendedPropertyUrl;
    $.getJSON(url + "/" + $(this).val(), {}, function(result) {
      // remove old questions;
      $("div#ExtendedProperties")
        .children()
        .remove();

      if (!result.length) {
        return;
      }

      // for each question, add the input, etc
      $.each(result, function(index, item) {
        var name = item.Name.replace(/ /g, "");

        // create container
        var container = $("<div class='form-group'>");

        // create the label
        var label = $("<label>")
          .attr("for", "item." + name)
          .html(item.Name);

        // must have a title, or bt breaks all of this.
        var textBox = $("<input>")
          .attr("type", "text")
          .attr("name", "ExtendedProperties[" + index + "].value")
          .attr("class", "required form-control");

        // create hidden field to store the extended property id
        var hidden = $("<input>")
          .attr("type", "hidden")
          .attr("name", "ExtendedProperties[" + index + "].propertyId")
          .val(item.Id);

        // add date values
        if (item.QuestionType.Name === "Date") {
          textBox
            .attr("title", "mm/dd/yyyy")
            .attr("placeholder", "mm/dd/yyyy")
            .attr("data-date-format", "mm/dd/yyyy")
            .datepicker();
        }

        // build and append tree
        container
          .append(label)
          .append(textBox)
          .append(hidden);

        $("div#ExtendedProperties").append(container);
      });
    });
  });

  $("img#tagAddButton").click(function(event) {
    var input = $("<input>")
      .attr("id", "tags")
      .attr("name", "tags")
      .val($("input#tagInput").val());
    input.attr("type", "text");
    input.css("cursor", "pointer");
    input.click(function(event) {
      $(this).remove();
    });
    $("div#tagContainer").append(input);

    // blank the input
    $("input#tagInput").val("");
  });

  $("input#tags").click(function(event) {
    $(this).remove();
  });

  $("a.FormSubmit").click(function(event) {
    //$(this).parents("form#RemoveForm").submit();

    $(this)
      .parent()
      .submit();
  });

  $("input#Item_QuantityName").change(function(event) {
    var quantityName = $(this).val();
    $("#CostPerItemLabel").text("Cost Per " + quantityName + ":");
    $("#QuantityLabel").text("Number of " + quantityName + "(s) Available:");
  });
});
