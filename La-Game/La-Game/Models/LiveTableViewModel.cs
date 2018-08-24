using La_Game.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static La_Game.Services.DashboardService;

namespace La_Game.Models
{
    public class LiveTableViewModel
    {
        public List<QuestionIdDto> Questions { get; set; }
        public List<ParticipantQuestionDto> Participants { get; set; }
        public int QuestionListId { get; set; }
    }
}