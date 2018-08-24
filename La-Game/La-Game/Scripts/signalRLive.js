var connection = $.hubConnection();
var hub = connection.createHubProxy("LiveHub");
hub.on("SubmitedAnswer", function (dto) {
        
    if (questionListId && questionListId === dto.IdQuestionList) {

        let index = questions.indexOf(dto.IdQuestion);
        index++;

        if (index === questions.length) {
            var element = document.getElementById("Participant_" + dto.IdParticipant);
            if (element) {
                element.parentNode.removeChild(element)
            }
        } else {
            var element = document.getElementById("Value_" + dto.IdParticipant);
            
            if (!element) {

                var table = document.getElementById("participants_body");

                var row = document.createElement('tr');
                row.id = "Participant_" + dto.IdParticipant;
                var tdName = document.createElement('td');
                tdName.innerHTML = dto.FullName;

                var tdValue = document.createElement('td');
                tdValue.id = "Value_" + dto.IdParticipant;
                tdValue.innerHTML = 1;

                row.appendChild(tdName);
                row.appendChild(tdValue);

                table.appendChild(row);
            } else {
                element.innerHTML = index;
            }
        }
    }

});

connection.start();