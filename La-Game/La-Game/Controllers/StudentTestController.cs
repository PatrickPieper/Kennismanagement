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
            if (index == null)
            {
                ViewBag.index = 0;
                
            }
            else
            {
                index++;
                ViewBag.index = index;
            }
            

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
                Participant participant = (Participant)TempData["participant"];
                // when I have the id from participant set it here
                string questionResultAttempt = "select qr.* from QuestionResult as qr" +
                                                        " join AnswerOption as ao on qr.AnswerOption_idAnswer = ao.idAnswer" +
                                                        " where ao.Question_idQuestion =" + questionId +  
                                                        " and qr.Participant_idParticipant = " + participant.idParticipant +
                                                        " and qr.QuestionList_idQuestionList = "+ questionListId +
                                                        " order by qr.attempt";
                IEnumerable < QuestionResult > questionResults = db.Database.SqlQuery<QuestionResult>(questionResultAttempt);
               int? lastAttempt = questionResults.Count() != 0 ? questionResults.Last().attempt : null;


                QuestionResult questionResult = new QuestionResult()
                {
                    QuestionList_idQuestionList = questionListId,
                    AnswerOption_idAnswer = studentAnswerId,
                    Participant_idParticipant = participant.idParticipant,
                    startTime = startTime,
                    endTime = DateTime.ParseExact(endTime, "yyyy:MM:dd HH:mm:ss:fff", null),
                    attempt = lastAttempt.HasValue ? lastAttempt.Value+1 : 1
                };
                db.QuestionResults.Add(questionResult);
                db.SaveChanges();

            }

            if (ViewBag.questionData.Count <= ViewBag.index)
            {
                TempData["doneMessage"] = "your have completed the test";
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