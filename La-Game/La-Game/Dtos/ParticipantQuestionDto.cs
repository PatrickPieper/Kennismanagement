using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace La_Game.Dtos
{
    public class ParticipantQuestionDto
    {
        public ParticipantDto Participant { get; set; }

        public QuestionIdDto CurrentQuestion { get; set; }
    }
}