using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using La_Game.Hubs;
using La_Game.Models;
using Microsoft.AspNet.SignalR;

namespace La_Game.Controllers
{
    /// <summary>
    /// Studenttest Controller.
    /// </summary>
    public class StudentTestController : Controller
    {
        private LaGameDBContext db = new LaGameDBContext();

        // GET: StudentTest
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public PartialViewResult TestEntryForm()
        {
            return PartialView("_TestEntryForm");
        }
        /// <summary>
        /// Partial view which contains a single question
        /// </summary>
        /// <param name="index">A counter to keep track of progress</param>
        /// <returns>Partial view containing a Question</returns>
        [AllowAnonymous]
        public PartialViewResult TestQuestionForm(int? index)
        {
            List<TestQuestionData> testQuestionData = TempData["testQuestionData"] as List<TestQuestionData>;
            TempData.Keep();
            if(index < testQuestionData.Count() || !index.HasValue)
            {
                TestQuestionData questionData = testQuestionData[index ?? 0];
                ViewBag.idQuestionList = questionData.idQuestionList;
                ViewBag.idParticipant = questionData.idParticipant;
                return PartialView("_TestQuestionForm", questionData);
            }
            else
            {
                return PartialView("_TestCompleted");
            }
        }
        /// <summary>
        /// Partial view for entering the test
        /// </summary>
        /// <param name="model">Model containing participation code and student code</param>
        /// <returns></returns>
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
            return PartialView("_TestEntryForm", model);
        }
        /// <summary>
        /// Partial view which contains each question as they're loaded
        /// </summary>
        /// <param name="model">Model containing participation code and student code </param>
        /// <param name="idQuestionList">Questionlist to get</param>
        /// <returns></returns>
        [AllowAnonymous]
        public PartialViewResult TestForm(StartListModel model, int idQuestionList)
        {
            //Get questions for the questionlist
            string sqlStringQuestions = "select q.* from Question as q join QuestionOrder as qo on qo.Question_idQuestion = q.idQuestion " +
                                        " join QuestionList as ql on ql.idQuestionList = qo.QuestionList_idQuestionList " +
                                        " where ql.participationCode = '" + model.Participationcode + "' order by qo.[order] ";
            var questionData = db.Database.SqlQuery<Question>(sqlStringQuestions);

            //Get all matchin answeroptions
            string sqlStringAnswerOptions = "select ao.* from AnswerOption as ao " +
                                            "join Question as q on q.idQuestion = ao.Question_idQuestion " +
                                            "join QuestionList_Question as qlq on qlq.Question_idQuestion = q.idQuestion " +
                                            "where qlq.QuestionList_idQuestionList = " + idQuestionList;
            var answerOptionData = db.Database.SqlQuery<AnswerOption>(sqlStringAnswerOptions);

            int idParticipant = db.Participants.Where(p => p.studentCode == model.Studentcode).First().idParticipant;

            //Get the last attempt value, used to increment attempt count
            string getLastAttempt = "SELECT MAX(Q2.attempt) as lastAttempt FROM AnswerOption as ao" +
                        " join QuestionResult Q2 on ao.idAnswer = Q2.AnswerOption_idAnswer" +
                        " WHERE ao.Question_idQuestion = " + questionData.First().idQuestion +
                        " AND Q2.Participant_idParticipant = " + idParticipant +
                        " AND Q2.QuestionList_idQuestionList = " + idQuestionList;

            int? lastAttempt = db.Database.SqlQuery<int?>(getLastAttempt).Single();
            ViewBag.attempt = lastAttempt;

            //List containing TestQuestionData models, used to combine all of the question data in one place
            List<TestQuestionData> testQuestionData = new List<TestQuestionData>();
            foreach (Question question in questionData)
            {
                testQuestionData.Add(new TestQuestionData
                {
                    questionData = question,
                    answerOptions = answerOptionData.Where(ao => question.idQuestion == ao.Question_idQuestion).ToList(),
                    idQuestionList = idQuestionList,
                    idParticipant = idParticipant
                });
            }

            //Store the questionlist data in the tempdata
            TempData["testQuestionData"] = testQuestionData;
            TempData["attempt"] = lastAttempt + 1;
            TempData.Keep();

            return PartialView("_TestForm");
        }
        /// <summary>
        /// Controller action to submit questionresults
        /// </summary>
        /// <param name="idAnswer">Chosen answer</param>
        /// <param name="idParticipant">Participant who answered</param>
        /// <param name="idQuestionList">Questionlist</param>
        /// <param name="startTime">The time the question was started</param>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public void SubmitQuestionAnswer(int idAnswer, int idParticipant, int idQuestionList, string startTime )
        {
            QuestionResult questionResult = new QuestionResult()
            {
                QuestionList_idQuestionList = idQuestionList,
                AnswerOption_idAnswer = idAnswer,
                Participant_idParticipant = idParticipant,
                startTime = DateTime.ParseExact(startTime, "yyyy:MM:dd HH:mm:ss:fff", null),
                endTime = DateTime.Now,
                attempt = TempData["attempt"] != null ? (int)TempData["attempt"] : 1
            };
            TempData.Keep();
            db.QuestionResults.Add(questionResult);
            db.SaveChanges();

            questionResult = db.QuestionResults.Include(x => x.AnswerOption).Include(x=> x.Participant).FirstOrDefault(x => x.idQuestionResult == questionResult.idQuestionResult);

            int idQuestion = questionResult.AnswerOption.Question_idQuestion;
            string fullName = questionResult.Participant.fullName;

            var hubContext = GlobalHost.ConnectionManager.GetHubContext<LiveHub>();
            hubContext.Clients.Group("Members").SubmitedAnswer(new SubmitedDto { IdAnswer = idAnswer, IdQuestionList = idQuestionList, IdQuestion = idQuestion, IdParticipant = idParticipant, FullName = fullName });
        }

        public class SubmitedDto
        {
            public string FullName { get; set; }
            public int IdParticipant { get; set; }
            public int IdQuestionList { get; set; }
            public int IdAnswer { get; set; }
            public int IdQuestion { get; set; }

            
        }

       

        /// <summary>
        /// Dispose of the database connection.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}