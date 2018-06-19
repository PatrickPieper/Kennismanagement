var count = 1;
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
            $("#btntestentry").text('Loading..');
            $("#btntestentry").prop('disabled', true);
            $.ajax({
                url: _form.attr("action"),
                type: 'POST',
                dataType: 'html',
                data: _form.serialize(),
                success: function (data) {
                    $('#partialContainer').html(data);
                    $('#loginlink').hide();
                    $('#indexlink').removeAttr('href');
                }
            });
        }

    });
});
function replaceQuestionPartial() {
    $.ajax({
        url: "/StudentTest/TestQuestionForm",
        type: 'GET',
        dataType: 'html',
        data: { index: count },
        success: function (data) {
            $('#testQuestionPartialContainer').html(data);
        }
    });
    count++;
}
$(document).on('click', '#debugnext', function () { replaceQuestionPartial();});
$(document).on('click', '.answerbutton', function (e) {
    var form = $('#__AjaxAntiForgeryForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();
    $(".answerbutton").prop('disabled', true);
    $.ajax({
        url: "StudentTest/SubmitQuestionAnswer",
        type: 'POST',
        dataType: 'html',
        data: { __RequestVerificationToken: token, idAnswer: $(this).val(), idParticipant: $('#idParticipant').val(), idQuestionList: $('#idQuestionList').val(), startTime : $('#startTime').val() },
        success: function (data) {
            replaceQuestionPartial();
        }
    });
});