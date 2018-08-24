using La_Game.Dtos;
using La_Game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace La_Game.Services
{
    public class DashboardService
    {
        private LaGameDBContext db = new LaGameDBContext();

        public LiveViewModel CreateLiveViewModel()
        {
            LiveViewModel vm = new LiveViewModel();

            vm.QuestionList = db.QuestionLists.Where(x => x.isActive == 1 && x.QuestionResults.Count() > 0).Select(x => new SelectListItem { Text = x.questionListName, Value = x.idQuestionList.ToString()}).ToList();

            return vm;
        }

        public LiveTableViewModel CreateLiveTableViewModel(int questionListId)
        {
            LiveTableViewModel result = db.QuestionLists
                .Where(x => x.idQuestionList == questionListId)
                .Select(x => new LiveTableViewModel
                {
                    Questions = x.QuestionList_Question.Select(n => new QuestionIdDto() { Id = n.Question.idQuestion }).ToList(),
                   
                    Participants = x.QuestionResults.GroupBy(m => m.Participant).Select(c => new ParticipantQuestionDto
                    {
                        Participant = new ParticipantDto() { Id = c.Key.idParticipant, FirstName = c.Key.firstName, LastName = c.Key.lastName},
                        CurrentQuestion = c.OrderByDescending(o => o.startTime).Select(o => new QuestionIdDto
                        {
                            Id = o.AnswerOption.Question_idQuestion
                        }).FirstOrDefault()
                    }).ToList()
                }).First();

            result.QuestionListId = questionListId;

            var lastQuestionid = result.Questions.Last().Id;
            foreach (var participant in result.Participants.ToList())
            {
                if (participant.CurrentQuestion.Id == lastQuestionid)
                {
                    result.Participants.Remove(participant);
                }
            }

            return result;
        }

    
    }


}