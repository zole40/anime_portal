(function ($) {
    var defaultOptions = {
        errorClass: 'has-error has-feedback',
        validClass: 'has-success has-feedback',

        highlight: function (element, errorClass, validClass) {
            $(element).closest('.form-group')
            .addClass(errorClass)
            .removeClass(validClass);


        },
        unhighlight: function (element, errorClass, validClass) {
            $(element).closest('.form-group')
            .removeClass(errorClass)
            .addClass(validClass);
        }
    };
    $.validator.setDefaults(defaultOptions);
    $.validator.unobtrusive.options = {
        errorClass: defaultOptions.errorClass,
        validClass: defaultOptions.validClass,
    };
})(jQuery);