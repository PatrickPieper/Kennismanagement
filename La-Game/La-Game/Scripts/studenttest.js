$(function () {

    $("#btntestentry").click(function (e) {

        e.preventDefault();
        var _this = $(this);
        var _form = _this.closest("form");

        var isvalid = _form.valid();  // Tells whether the form is valid

        if (isvalid) {
            //$.post(_form.attr("action"), _form.serialize(), function (data) {
            //    //check the result and do whatever you want
            //})
            $.ajax({
                url: _form.attr("action"),
                type: 'POST',
                dataType: 'html',
                data: _form.serialize(),
                success: function (data) {
                    $('#partialContainer').html(data);
                }
            });
        }

    });

});