using System.Collections.Generic;

namespace La_Game.Models
{
    public class CommonWrongQuestionResult
    {
        public int idQuestion { get; set; }
        public int wrongCount { get; set; }
        public string questionText { get; set; }
    }
}