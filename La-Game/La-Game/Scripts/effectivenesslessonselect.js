//If language dropdown selection changes
$('#ddlanguages').change(function (e) {
    //Update chart with dropdown value
    updateChart($(this).val());
}
);
