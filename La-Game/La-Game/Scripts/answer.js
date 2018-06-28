var count = 2;
var answerType = $('input[name=answerType]');

// Checks which answer is selected and show/hide divs depending on which answer is selected.
answerType.change(function () {
    var selected = $('input[name=answerType]:checked');
    if (selected.val() === "multiplechoice") {
        $("#Option").css("display", "inline");
        $("#AddAnswer").css("display", "inline");
        $("#test").css("display", "none");
        $("#HP").attr("required", false);
    } else if (selected.val() === "likert") {
        $("#test").css("display","inline");
        $("#Option").css("display", "none");
        $("#AddAnswer").css("display", "none");
        $("#HP").attr("required", true);
    }
}
);
// Adds a new Partialview
// 
$('#AddAnswer').click(function (e) {
    e.preventDefault();

    
    if (count < 6) {
        url = $(this).data('url');

        target = $('#Option');

        $.get(url, function (data) {
            target.append(data);
        });

        count++;
    } else {
        alertbox = $('#alertBox');
        alertbox.addClass("alert-danger");
        alertbox.html("<p>The maximum amount of answers is 6!</p>");
        alertbox.show();
        window.scrollTo(0,0);
    }
    
}
);

// Delete PartialView
// 
$(document).on('click', '.deleteAnswer', function (e) {
    e.preventDefault();
    if (count > 2) {
        count--;
        deleteAnswer($(this));
        
    } else {
        alertbox = $('#alertBox');
        alertbox.addClass("alert-danger");
        alertbox.empty();
        alertbox.html("<p>Multiple choice questions need at least 2 answers!</p>");
        alertbox.show();
        window.scrollTo(0, 0);
    }
    
}
);

function deleteAnswer(button) {
    wrapper = button.closest('.answerWrapper');
    wrapper.remove();
    optionDiv = $("#Option");
    optionDiv.load(optionDiv);
}


// Shows a previes of the uploaded Image.
function readURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            document.getElementById("impPrev").style.display = 'inline';
            $('#impPrev').attr('src', e.target.result);
        }
        reader.readAsDataURL(input.files[0]);
    }
}
$("#fileImage").change(function () {
    readURL(this);
});

// If there is no image uploaded hide the image view.
document.addEventListener("DOMContentLoaded", function (event) {
    document.querySelectorAll('img').forEach(function (img) {
        img.onerror = function () { this.style.display = 'none'; };
    })
});

// If there is no audio uploaded hide the player view
document.addEventListener("DOMContentLoaded", function (event) {
    document.querySelectorAll('audio').forEach(function (audio) {
        audio.onerror = function () { this.style.display = 'none'; };
    })
});

