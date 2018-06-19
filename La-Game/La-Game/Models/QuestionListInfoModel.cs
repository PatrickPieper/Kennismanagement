using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace La_Game.Models
{
    public class QuestionListInfoModel
    {
        public int IdQuestionList { get; set; }
        public int MaxAttempt { get; set; }
        public int QuestionCount { get; set; }
    }
}