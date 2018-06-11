//If language dropdown selection changes
$('#ddlanguages').change(function (e) {
    //Store the currently selected index
    var selectedLanguageIndex = $(this).prop("selectedIndex");
    var idLesson = -1;
    var idQuestionList = -1;
    //Check if lessons is set
    if ($('#ddlessons').prop("selectedIndex") !== -1) {
        idLesson: $('#ddlessons').val();
    }
    if ($('#ddquestionList').prop("selectedIndex") !== -1) {
        idQuestionList: $('#ddquestionList').val();
    }
    //Replace the filter partial with a new one, with the dropdown values
    $.ajax({
        url: '/Statistics/CommonWrongAnswerFilter',
        type: 'GET',
        dataType: 'html',
        async: false,
        data: { idLanguage: $(this).val(), idLesson: idLesson },
        success: function (data) {
            $('#filter').html(data);
        }
    });
    //Set index back after replacing filter partial
    $("#ddlanguages").prop("selectedIndex", selectedLanguageIndex);
    //Update chart with new filters
    updateChart($(this).val(), idLesson, idQuestionList);
}
);
//If lessons dropdown selection changes
$('#ddlessons').change(function (e) {
    //Store the currently selected indexes
    var selectedLessonIndex = $(this).prop("selectedIndex");
    var selectedLanguageIndex = $("#ddlanguages").prop("selectedIndex");
    var idLesson = -1;
    var idQuestionList = -1;
    if ($('#ddquestionLists').prop("selectedIndex") !== -1) {
        idQuestionList: $('#ddquestionList').val();
    }
    //Replace the filter partial with a new one, with the dropdown values
    $.ajax({
        url: '/Statistics/CommonWrongAnswerFilter',
        type: 'GET',
        dataType: 'html',
        async: false,
        data: { idLanguage: $('#ddlanguages').val(), idLesson: $(this).val() },
        success: function (data) {
            $('#filter').html(data);
        }
    });
    //Set indexes back after replacing filter partial
    $("#ddlanguages").prop("selectedIndex", selectedLanguageIndex);
    $("#ddlessons").prop("selectedIndex", selectedLessonIndex);
    //Update chart with new filters
    updateChart($('#ddlanguages').val(), $(this).val(), idQuestionList);
}
);
$('#ddquestionLists').change(function (e) {
    //Store the currently selected indexes
    var selectedLessonIndex = $(this).prop("selectedIndex");
    var selectedLanguageIndex = $("#ddlanguages").prop("selectedIndex");
    var idLesson = -1;
    var idQuestionList = -1;
    if ($('#ddquestionList').prop("selectedIndex") !== -1) {
        idQuestionList: $('#ddquestionList').val();
    }
    //Update chart with new filters
    updateChart($('#ddlanguages').val(), $('#ddlessons').val(), $(this).val());
}
);
