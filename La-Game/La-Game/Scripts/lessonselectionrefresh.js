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
        url: '/Statistics/CompareLessonSelection',
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
}
);
$('#ddlessons').change(function (e) {
    //Store the currently selected indexes
    if ($('#ddlanguages').prop("selectedIndex") !== 0)
    {
        $("#btnselectionadd").prop('disabled', false);
    }
    if ($('#ddquestionLists').prop("selectedIndex") !== 0)
    {
        $("#btnselectionadd").prop('disabled', false);
    }
}
);

//Add chart data for the selection in the dropdowns
function addSelectionToChart()
{
    updateChart($('#ddlanguages').val(), $('#ddlessons').val());
    $("#ddlanguages").prop("selectedIndex", 0);
    $("#ddlessons").prop("selectedIndex", 0);
    $("#btnselectionadd").prop('disabled', true);
    $("#btnclearchartdata").prop('disabled', false);
}
//Clear the chart data
function clearChartData()
{
    clearChart();
    $("#ddlanguages").prop("selectedIndex", 0);
    $("#ddlessons").prop("selectedIndex", 0);
    $("#btnselectionadd").prop('disabled', true);
    $("#btnclearchartdata").prop('disabled', true);
}

//Update chart with new data
function updateChart(idLanguage, idLesson)
{
    //Get the two datasets as jsonresult from controller
    var tData = $.getValues('/Statistics/BarChartDataCompareLessons?idLanguage=' + idLanguage + '&idLesson=' + idLesson);
    //If the chart doesn't have datasets in its array, add both full datasets
    if (myBarChart.data.datasets.length === 0) {
        myBarChart.data.datasets.push(tData[0]);
        myBarChart.data.datasets.push(tData[1]);
        myBarChart.data.labels.push('Lesson ' + myBarChart.data.datasets[0].data.length);
    }
    //If it already has datasets, add the data in the returned datasets instead
    else
    {
        myBarChart.data.datasets[0].data.push(tData[0].data);
        myBarChart.data.datasets[1].data.push(tData[1].data);
        myBarChart.data.labels.push('Lesson ' + myBarChart.data.datasets[0].data.length);
    }
    myBarChart.update();
}
//Update chart with empty arrays
function clearChart()
{
    myBarChart.data.datasets = [];
    myBarChart.data.labels = [];
    myBarChart.update();
}