$('#ddlanguages').change(function (e) {
    var selectedLanguageIndex = $(this).prop("selectedIndex");
    //var selectedLessonIndex = $('ddlessons').prop("selectedIndex");
    //var selectedQuestListIndex = $('ddquestionList').prop("selectedIndex");
    $.ajax({
        url: '/Statistics/CommonWrongAnswerFilter', //@Url.Action("FunName","ControllerName")
        type: 'GET',
        dataType: 'html',
        async: false,
        data: { idLanguage: $(this).val()},
        success: function (data) { //Make the function to return the partial view you want which would be fetched in the data
            $('#filter').html(data);
        }
    });
    $("#ddlanguages").prop("selectedIndex", selectedLanguageIndex);
    $("#ddlessons").prop("selectedIndex", -1);
    $("#ddquestionLists").prop("selectedIndex", -1);
}
);