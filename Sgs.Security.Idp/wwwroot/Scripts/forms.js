(function () {

    $(function () {

        $(".field-validation-error").prev(".form-text").hide();

        if ($(".validation-summary-errors").length) {
            if ($("form").data("submit-error")) {
                toastr.error($("form").data("submit-error"));
            }
        }

        $('form').submit(function (e) {
            if (!$(this).valid()) {
                $(".field-validation-error").prev(".form-text").hide();

                if ($(this).data("submit-error")) {
                    toastr.error($(this).data("submit-error"));
                }
                else {
                    toastr.error("Submit error");
                }
            }
        });

        //$("form").validate({
        //    submitHandler: function (form) {
        //        $(".field-validation-error").prev(".form-text").hide();
        //    }
        //});

        $('input').focusout(function () {
            var currInput = this;
            if (!$(currInput).valid()) {
                $(".field-validation-error").prev(".form-text").hide();
            }
        });

        function resetForm() {
            $(".field-validation-error").prev(".form-text").show();
            $('.validation-summary-errors li').remove();
            //Removes validation from input-fields
            //$('.input-validation-error').addClass('input-validation-valid');
            $('.input-validation-error').removeClass('input-validation-error');
            //Removes validation message after input-fields
            $('.field-validation-error span').remove();
            $('.field-validation-error').addClass('field-validation-valid');
            $('.field-validation-error').removeClass('field-validation-error');

            //Removes validation summary
            $('.validation-summary-errors').addClass('validation-summary-valid');
            $('.validation-summary-errors').removeClass('validation-summary-errors');

            //var url = '@Url.Action("Index", new { statMsg ="Cancel new user"})';
            //window.location.href = url;
        }

        $("button:reset").on("click", resetForm());

    });

})();