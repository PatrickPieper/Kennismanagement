using System;
using System.Collections.Generic;

namespace La_Game.Models
{
    public class ParticipantLessonWrongCorrect
    {
        public int IdParticipant { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Lesson_idLesson { get; set; }
        public string LessonName { get; set; }
        public int CorrectCount { get; set; }
        public int WrongCount { get; set; }
        public double CorrectPercentage { get; set; }

        public void UpdateCorrectPercentage()
        {
            if(WrongCount == 0 && CorrectCount != 0)
            {
                CorrectPercentage = 100;
                return;
            }
            if(WrongCount == 0 && CorrectCount == 0)
            {
                CorrectPercentage = 0;
                return;
            }
            CorrectPercentage = Math.Round((double)CorrectCount / ((double)CorrectCount + (double)WrongCount) * 100.0,2);
        }
    }
}