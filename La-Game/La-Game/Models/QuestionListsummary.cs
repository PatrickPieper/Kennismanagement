using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace La_Game.Models
{
    public class QuestionListsummary
    {
        public int questionListId { get; set; }
        public string questionListName { get; set; }
        public int QuestionCount { get; set; }
        public int HighestAttempt { get; set; }
    }
}