var count = 1;
console.log(count);
var answerType = $('input[name=answerType]');

answerType.change(function () {
    var selected = $('input[name=answerType]:checked');
    if (selected.val() == 'meerkeuze') {
        document.getElementById("Option").style.display = 'inline';
        document.getElementById("AddAnswer").style.display = 'inline';
    } else {
        document.getElementById("Option").style.display = 'none';
        document.getElementById("AddAnswer").style.display = 'none';
    }
}
);

$('#AddAnswer').click(function (e) {
    e.preventDefault();
    count++;
    console.log(count);

    url = $(this).data('url');

    target = $('#Option');
    console.log(url);

    $.get(url, function (data) {
        target.append(data);
    });
}
);

$(document).on('click', '.deleteAnswer', function (e) {
    e.preventDefault();
    count--;
    console.log(count);
    deleteAnswer($(this));
}
);

function deleteAnswer(button) {
    wrapper = button.closest('.answerWrapper');
    wrapper.remove();
    optionDiv = $("#Option");
    optionDiv.load(optionDiv);
}

