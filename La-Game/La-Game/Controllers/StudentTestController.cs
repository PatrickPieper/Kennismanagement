﻿using System;
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
        List<TestQuestionData> testQuestionData;

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
        [AllowAnonymous]
        public PartialViewResult TestQuestionForm(int? index)
        {
            List<TestQuestionData> testQuestionData = TempData["testQuestionData"] as List<TestQuestionData>;
            return PartialView("_TestQuestionForm", testQuestionData[index.HasValue ? index.Value : 0]);
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public PartialViewResult TestEntryForm(StartListModel model)
        {
            if (ModelState.IsValid)
            {
                // Get the questionlist
                var questionList = db.QuestionLists.Where(ql => ql.participationCode.Equals(model.Participationcode));

                // Get the participant
                var participant = db.Participants.Where(p => p.studentCode == model.Studentcode);

                // If both the codes are valid, continue
                if (questionList.Count() != 0 && participant.Count() != 0)
                {
                    // Get the questions
                    //int questionListID = questionListData[0].idQuestionList;
                    //sqlString = "SELECT q.* FROM Question AS q JOIN QuestionOrder AS qo on qo.Question_idQuestion = q.idQuestion" +
                    //" JOIN QuestionList AS ql on ql.idQuestionList = qo.QuestionList_idQuestionList WHERE ql.participationCode = '" + model.Participationcode + "' ORDER BY qo.[order]";
                    //List<Question> questionData = db.Questions.SqlQuery(sqlString).ToList<Question>();

                    //// Set the tempdata and redirect to the questionlist
                    //TempData["questionListData"] = questionListData;
                    //TempData["questionData"] = questionData;
                    //TempData["participant"] = participant;
                    return TestForm(model, questionList.First().idQuestionList);
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
        public PartialViewResult TestForm(StartListModel model, int idQuestionList)
        {
            string sqlStringQuestions = "select q.* from Question as q join QuestionOrder as qo on qo.Question_idQuestion = q.idQuestion " +
                                        " join QuestionList as ql on ql.idQuestionList = qo.QuestionList_idQuestionList " +
                                        " where ql.participationCode = '" + model.Participationcode + "' order by qo.[order] ";
            var questionData = db.Database.SqlQuery<Question>(sqlStringQuestions);

            string sqlStringAnswerOptions = "select ao.* from AnswerOption as ao " +
                                            "join Question as q on q.idQuestion = ao.Question_idQuestion " +
                                            "join QuestionList_Question as qlq on qlq.Question_idQuestion = q.idQuestion " +
                                            "where qlq.QuestionList_idQuestionList = " + idQuestionList;
            var answerOptionData = db.Database.SqlQuery<AnswerOption>(sqlStringAnswerOptions);

            testQuestionData = new List<TestQuestionData>();
            foreach(Question question in questionData)
            {
                testQuestionData.Add(new TestQuestionData
                {
                    questionData = question,
                    answerOptions = answerOptionData.Where(ao => question.idQuestion == ao.Question_idQuestion).ToList()
                });
            }

            TempData["testQuestionData"] = testQuestionData;
            TempData.Keep();

            return PartialView("_TestForm");
        }
        [AllowAnonymous]
        public ActionResult LikertScale(int? index)
        {
            return View();

        }

    }
}