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
        public ActionResult Index()
        {
            //ViewBag.participant = TempData["participant"];
            //if (index == null)
            //{
            //    ViewBag.index = 0;

            //}
            //else
            //{
            //    index++;
            //    ViewBag.index = index;
            //}


            //if (TempData["questionListData"] != null && TempData["questionData"] != null)
            //{ 
            //    ViewBag.questionListData = TempData["questionListData"];
            //    ViewBag.questionData = TempData["questionData"];
            //}

            //if (TempData["answerOptions"] == null)
            //{
            //    string selectQuery = "SELECT * FROM AnswerOption INNER JOIN Question on AnswerOption.Question_idQuestion=Question.idQuestion;";
            //    List<AnswerOption> answerOptions = db.AnswerOptions.SqlQuery(selectQuery).ToList<AnswerOption>();
            //    ViewBag.answerOptions = answerOptions;
            //}
            //else
            //{
            //    ViewBag.answerOptions = TempData["answerOptions"];
            //}

            //if (studentAnswerId != 0 && TempData["startTime"] != null)
            //{
            //    string endTime = DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss:fff");
            //    DateTime startTime = DateTime.ParseExact((string)TempData["startTime"], "yyyy:MM:dd HH:mm:ss:fff", null);
            //    int questionListId = ViewBag.questionListData[0].idQuestionList;
            //    int questionId = db.AnswerOptions.Find(studentAnswerId).Question_idQuestion;
            //    Participant participant = (Participant)TempData["participant"];
            //    // when I have the id from participant set it here
            //    string questionResultAttempt = "select qr.* from QuestionResult as qr" +
            //                                            " join AnswerOption as ao on qr.AnswerOption_idAnswer = ao.idAnswer" +
            //                                            " where ao.Question_idQuestion =" + questionId +  
            //                                            " and qr.Participant_idParticipant = " + participant.idParticipant +
            //                                            " and qr.QuestionList_idQuestionList = "+ questionListId +
            //                                            " order by qr.attempt";
            //    IEnumerable < QuestionResult > questionResults = db.Database.SqlQuery<QuestionResult>(questionResultAttempt);
            //   int? lastAttempt = questionResults.Count() != 0 ? questionResults.Last().attempt : null;


            //    QuestionResult questionResult = new QuestionResult()
            //    {
            //        QuestionList_idQuestionList = questionListId,
            //        AnswerOption_idAnswer = studentAnswerId,
            //        Participant_idParticipant = participant.idParticipant,
            //        startTime = startTime,
            //        endTime = DateTime.ParseExact(endTime, "yyyy:MM:dd HH:mm:ss:fff", null),
            //        attempt = lastAttempt.HasValue ? lastAttempt.Value+1 : 1
            //    };
            //    db.QuestionResults.Add(questionResult);
            //    db.SaveChanges();

            //}

            //if (ViewBag.questionData.Count <= ViewBag.index)
            //{
            //    TempData["doneMessage"] = "your have completed the test";
            //    return RedirectToAction("Index", "Home");
            //}

            ////to do: when we have a boolean for LikertScale or MultipleChoice return the right view
            //return View("MultipleChoice");
            return View();
        }

        [AllowAnonymous]
        public PartialViewResult TestEntryForm()
        {
            return PartialView("_TestEntryForm");
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public PartialViewResult TestEntryForm(StartListModel model)
        {
            if (ModelState.IsValid)
            {
                // Get the questionlist
                string sqlString = "SELECT * FROM QuestionList WHERE participationCode ='" + model.Participationcode + "'";
                List<QuestionList> questionListData = db.QuestionLists.SqlQuery(sqlString).ToList<QuestionList>();

                // Get the participant
                Participant participant = null;
                sqlString = "SELECT * FROM PARTICIPANT WHERE studentCode='" + model.Studentcode + "'";
                if (db.Participants.SqlQuery(sqlString).ToList<Participant>().Count != 0)
                {
                    participant = db.Participants.SqlQuery(sqlString).First();
                }

                // If both the codes are valid, continue
                if (questionListData.Count != 0 && participant != null)
                {
                    // Get the questions
                    int questionListID = questionListData[0].idQuestionList;
                    sqlString = "SELECT q.* FROM Question AS q JOIN QuestionOrder AS qo on qo.Question_idQuestion = q.idQuestion" +
                    " JOIN QuestionList AS ql on ql.idQuestionList = qo.QuestionList_idQuestionList WHERE ql.participationCode = '" + model.Participationcode + "' ORDER BY qo.[order]";
                    List<Question> questionData = db.Questions.SqlQuery(sqlString).ToList<Question>();

                    // Set the tempdata and redirect to the questionlist
                    TempData["questionListData"] = questionListData;
                    TempData["questionData"] = questionData;
                    TempData["participant"] = participant;
                    return PartialView("_TestForm", model);
                }
                else
                {
                    // One of the codes was incorrect
                    ModelState.AddModelError(String.Empty, "The participationcode and/or studentcode was not valid.");
                }
            }
            else
            {
                // The data in one of the fields was invalid
                ModelState.AddModelError(String.Empty, "Invalid input(s)");
            }

            // Stay on the page with the current data
            return PartialView("_TestEntryForm",model);
        }
        [AllowAnonymous]
        public ActionResult MultipleChoice()
        {
            return View();
           
        }
        [AllowAnonymous]
        public PartialViewResult TestForm(int? questionIndex)
        {
            return PartialView("_TestForm");
        }
        [AllowAnonymous]
        public ActionResult LikertScale(int? index)
        {
            return View();

        }

    }
}