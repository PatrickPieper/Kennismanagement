using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace La_Game.Models
{
    public class QuestionResultDto
    {
        public Nullable<int> attempt { get; set; }

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