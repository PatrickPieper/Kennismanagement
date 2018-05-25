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
)

$('#AddAnswer').click(function (e) {
    e.preventDefault();


    url = $(this).data('url');

    target = $('#Option');
    console.log(url);

    $.get(url, function (data) {
        console.log("click");

        target.append(data);
    });
}

);

