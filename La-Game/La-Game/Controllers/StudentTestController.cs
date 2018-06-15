using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using La_Game.Models;

namespace La_Game.Controllers
{
    public class StudentTestController : Controller
    {
        private LaGameDBContext db = new LaGameDBContext();
        
        // GET: StudentTest
        [AllowAnonymous]
        public ActionResult Index(int? index, int studentAnswerId = 0)
        {
            ViewBag.participant = TempData["participant"];
            Participant participant = (Participant)TempData["participant"];
            if (index == null)
            {
                ViewBag.index = 0;
                var data = TempData["attempt"];
                if (TempData["attempt"] == null && TempData["questionData"] != null)
                {
                    List<Question> questions = (List<Question>)TempData["questionData"];
                    Question firstQuestion = questions.First();
                    var test = (List<QuestionList>)TempData["questionListData"];
                    QuestionList qlist = test.Single();

                    string getLastAttempt = "SELECT MAX(Q2.attempt) as lastAttempt FROM AnswerOption as ao" +
                        " join QuestionResult Q2 on ao.idAnswer = Q2.AnswerOption_idAnswer" + 
                        " WHERE ao.Question_idQuestion = " + firstQuestion.idQuestion + 
                        " AND Q2.Participant_idParticipant = " + participant.idParticipant + 
                        " AND Q2.QuestionList_idQuestionList = " + qlist.idQuestionList;

                    int? lastAttempt = db.Database.SqlQuery<int?>(getLastAttempt).Single();
                    ViewBag.attempt = lastAttempt;
                }
            }
            else
            {
                index++;
                ViewBag.index = index;
            }

            ViewBag.attempt = TempData["attempt"];

            if (TempData["questionListData"] != null && TempData["questionData"] != null)
            { 
                ViewBag.questionListData = TempData["questionListData"];
                ViewBag.questionData = TempData["questionData"];
            }

            if (TempData["answerOptions"] == null)
            {
                string selectQuery = "SELECT * FROM AnswerOption INNER JOIN Question on AnswerOption.Question_idQuestion=Question.idQuestion;";
                List<AnswerOption> answerOptions = db.AnswerOptions.SqlQuery(selectQuery).ToList<AnswerOption>();
                ViewBag.answerOptions = answerOptions;
            }
            else
            {
                ViewBag.answerOptions = TempData["answerOptions"];
            }

            if (studentAnswerId != 0 && TempData["startTime"] != null)
            {
                string endTime = DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss:fff");
                DateTime startTime = DateTime.ParseExact((string)TempData["startTime"], "yyyy:MM:dd HH:mm:ss:fff", null);
                int questionListId = ViewBag.questionListData[0].idQuestionList;
                int questionId = db.AnswerOptions.Find(studentAnswerId).Question_idQuestion;

                QuestionResult questionResult = new QuestionResult()
                {
                    QuestionList_idQuestionList = questionListId,
                    AnswerOption_idAnswer = studentAnswerId,
                    Participant_idParticipant = participant.idParticipant,
                    startTime = startTime,
                    endTime = DateTime.ParseExact(endTime, "yyyy:MM:dd HH:mm:ss:fff", null),
                    attempt = TempData["attempt"] != null ? (int)TempData["attempt"] + 1 : 1
                };
                db.QuestionResults.Add(questionResult);
                db.SaveChanges();

            }

            if (ViewBag.questionData.Count <= ViewBag.index)
            {
                TempData["doneMessage"] = "You have completed the test";
                return RedirectToAction("Index", "Home");
            }

            //to do: when we have a boolean for LikertScale or MultipleChoice return the right view
            return View("MultipleChoice");
        }
        
        [AllowAnonymous]
        public ActionResult MultipleChoice()
        {
            return View();
           
        }

        [AllowAnonymous]
        public ActionResult LikertScale(int? index)
        {
            return View();

        }

    }
}