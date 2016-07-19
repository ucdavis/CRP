jQuery.validator.addMethod("zipUS", function(zip_code, element) {
    zip_code = zip_code.replace(/\s+/g, "");
    return this.optional(element) || zip_code.match(/^\d{5}(-\d{4})?$/);
}, "Please specify a valid zip code");