using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace La_Game.Dtos
{
    public class QuestionResultDto
    {
        public int? attempt { get; set; }

        public int? totalTime { get; set; }

        public int participantId { get; internal set; }
        public int participantsCount { get; internal set; }

        public double AvarageTime
        {
            get
            {
                return totalTime != null ? (totalTime.Value / participantsCount) : 0d;
            }
        }
    }
}