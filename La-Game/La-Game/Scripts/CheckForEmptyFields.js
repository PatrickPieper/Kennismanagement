﻿$(document).ready(function () {
    $('#btnSubmit').click(function (e) {
        var isValid = true;
        var answerType = $('input[name=answerType]');
        var selected = $('input[name=answerType]:checked');
        if (selected.val() === "multiplechoice") {

            $('input[type="text"]').each(function ()
            
            {
                if ($.trim($(this).val()) == '') {
                    isValid = false;


                    alertbox = $('#alertBox');
                    alertbox.addClass("alert-danger");
                    alertbox.empty();
                    alertbox.html("<p>Not all fields are filled in!</p>");
                    alertbox.show();
                    alertbox.hide(5000);


                    $(this).css({
                        "border": "1px solid red",
                    });
                }
                else {
                    $(this).css({
                        "border": "",
                    });
                }
            });

            check()
        }

        else if (selected.val() === "likert") {
            if (!document.getElementById("questionText").value) {
                isValid = false;
                $("#questionText").css("border", "1px solid red");
                alertbox = $('#alertBox');
                alertbox.addClass("alert-danger");
                alertbox.empty();
                alertbox.html("<p>You forgot to fill in a Question!</p>"); alertbox.show();
            }
        }

        if (isValid == false)
            e.preventDefault();
    });
});



function check() {
    var count = 0;
    selects = document.getElementsByName("correctAnswer");
    alertbox = $('#alertBox');
    for (i = 0; i < selects.length; i++) {
        if (selects[i].value == 1) {
            count++;
        }
        if (count < 1) {
            alertbox.addClass("alert-danger");
            alertbox.html("<p>You have not chosen a correct answer!</p>");
            alertbox.show();
            window.scrollTo(0, 0);
            selects[i].value = 0;
            event.preventDefault()
        }
        else if (count > 1) {
            alertbox.hide();
        }
    }

};