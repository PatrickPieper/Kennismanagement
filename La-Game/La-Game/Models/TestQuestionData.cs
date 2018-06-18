using System.Collections.Generic;

namespace La_Game.Models
{
    public class TestQuestionData
    {
        public Question questionData { get; set; }
        public List<AnswerOption> answerOptions { get; set; }
        public int idQuestionList { get; set; }
        public int idParticipant { get; set; }

    }
}