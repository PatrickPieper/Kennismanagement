using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace La_Game.Models
{
    public class QuestionListResult
    {
        public int idQuestion { get; set; }
        public string answerText{ get; set; }
        public string questionText { get; set; }
        public short correctAnswer { get; set; }
        public int attempt { get; set; }
        public int totalTime { get; set; }
    }
}