var answerType = $('input[name=answerType]');
answerType.change(function () {
    var selected = $('input[name=answerType]:checked');
    if (selected.val() == 'meerkeuze') {
        $('.createMcAnswer').removeAttr('hidden');
    } else {
        $('.createMcAnswer').hide();
    }
}
)