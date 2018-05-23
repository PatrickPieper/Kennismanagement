var answerType = $('input[name=answerType]');

answerType.change(function () {
    var selected = $('input[name=answerType]:checked');
    if (selected.val() == 'meerkeuze') {
        document.getElementById("Option").style.display = 'inline';
    } else {
        document.getElementById("Option").style.display = 'none';
    }
}
)