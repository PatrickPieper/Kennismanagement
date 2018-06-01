var count = 2;
console.log(count);
var answerType = $('input[name=answerType]');

answerType.change(function () {
    var selected = $('input[name=answerType]:checked');
    if (selected.val() === 'meerkeuze') {
        document.getElementById("Option").style.display = 'inline';
        document.getElementById("AddAnswer").style.display = 'inline';
        document.getElementById("test").style.display = 'none';
    } else if (selected.val() === "likert") {
        document.getElementById("test").style.display = 'inline';
        document.getElementById("Option").style.display = 'none';
        document.getElementById("AddAnswer").style.display = 'none';
    }
}
);

$('#AddAnswer').click(function (e) {
    e.preventDefault();

    console.log(count);

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

