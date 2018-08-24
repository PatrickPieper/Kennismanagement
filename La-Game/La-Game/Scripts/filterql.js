//If language dropdown selection changes
$('#questionListDropdown').change(function (e) {
    //Store the currently selected index
    var selectedLanguageIndex = $(this).prop("selectedIndex");
    //Replace the filter partial with a new one, with the dropdown values
    $.ajax({
        url: '/Dashboard/Participants',
        type: 'GET',
        dataType: 'html',
        async: false,
        data: { idQuestionlist: $(this).val() },
        success: function (data) {
            $('#filterql').html(data);
        }
    });
    //Set index back after replacing filter partial
    // $("#ddlanguages").prop("selectedIndex", selectedLanguageIndex);
});