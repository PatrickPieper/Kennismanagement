using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace La_Game.Models
{
    public class QuestionListStatisticsModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int TotalTime { get; set; }
        public int Attempt { get; set; }
        public int CorrectCount { get; set; }
        public int WrongCount { get; set; }
    }
}