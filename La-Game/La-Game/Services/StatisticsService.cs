using La_Game.Dtos;
using La_Game.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace La_Game.Services
{
    public class StatisticsService
    {
        private static int idQuestionList;
        private LaGameDBContext db = new LaGameDBContext();

        public List<QuestionResultDto> QuestionResults(int idQuestionList)
        {

            List<QuestionResultDto> results = db.QuestionLists
                    .Where(x => x.idQuestionList == idQuestionList)
                    .SelectMany(x => x.QuestionResults)
                    .GroupBy(x => x.attempt)
                    .Select(x => new QuestionResultDto
                    {
                        attempt = x.Key,
                        participantsCount = x.GroupBy(g => g.Participant_idParticipant).Count(),
                        totalTime = x.Sum(y => DbFunctions.DiffSeconds(y.startTime, y.endTime))
                    })
                    .ToList();

            return results;
        }
    }
}