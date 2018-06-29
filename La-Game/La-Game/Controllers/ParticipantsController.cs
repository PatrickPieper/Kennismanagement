using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using La_Game.Models;

namespace La_Game.Controllers
{
    /// <summary>
    /// Participant Controller
    /// </summary>
    public class ParticipantsController : Controller
    {
        private LaGameDBContext db = new LaGameDBContext();

        // GET: Participants
        public ActionResult Index()
        {
            return View(db.Participants.ToList());
        }

        // GET: Participants/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Participant participant = db.Participants.Find(id);
            if (participant == null)
            {
                return HttpNotFound();
            }
            return View(participant);
        }

        // GET: Participants/Create
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// POST: Participants/Create
        /// After pressing the button check if the data is valid then add it to database.
        /// </summary>
        /// <param name="participant"> The data that has to be added. </param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idParticipant,firstName,lastName,birthDate,studentCode")] Participant participant)
        {
            // Check if the data is valid
            if (ModelState.IsValid)
            {
                // Check the data
                if (string.IsNullOrEmpty(participant.firstName) || string.IsNullOrEmpty(participant.lastName) || string.IsNullOrEmpty(participant.birthDate.ToString()))
                {
                    // One or more fields were empty
                    ModelState.AddModelError(string.Empty, "You need to fill all the fields.");
                    return View(participant);
                }

                // Get a unique studentId
                int studentId;
                Random rng = new Random();
                while (true)
                {
                    studentId = rng.Next(100000, 1000000);
                    string sqlstring = "SELECT * FROM Participant WHERE StudentCode=" + studentId;
                    List<Participant> students = db.Participants.SqlQuery(sqlstring).ToList();

                    if (students.Count == 0)
                    {
                        break;
                    }
                }

                // Set the code then add the participant to the database
                participant.studentCode = studentId;
                db.Participants.Add(participant);
                db.SaveChanges();

                // Redirect to list  
                return RedirectToAction("Index", "Participants");
            }

            // If not valid, stay on the page with the current data
            return View(participant);
        }

        // GET: Participants/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Participant participant = db.Participants.Find(id);
            if (participant == null)
            {
                return HttpNotFound();
            }
            return View(participant);
        }

        // POST: Participants/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idParticipant,firstName,lastName,birthDate,studentCode")] Participant participant)
        {
            if (ModelState.IsValid)
            {
                // Check the data
                if (string.IsNullOrEmpty(participant.firstName) || string.IsNullOrEmpty(participant.lastName) || string.IsNullOrEmpty(participant.birthDate.ToString()))
                {
                    // One or more fields were empty
                    ModelState.AddModelError(string.Empty, "You need to fill all the fields.");
                    return View(participant);
                }

                db.Entry(participant).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(participant);
        }

        // GET: Participants/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Participant participant = db.Participants.Find(id);
            if (participant == null)
            {
                return HttpNotFound();
            }
            return View(participant);
        }

        // POST: Participants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Participant participant = db.Participants.Find(id);
            db.Participants.Remove(participant);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Display all questionlists made by the participant, the number of questions in them and the total amount of attempts for the participant.
        /// </summary>
        /// <param name="id">Id of the participant we want to see the lists from</param>
        /// <returns>returns a view with all data needed for the page</returns>
        public ActionResult Results(int id)
        {
            Participant participant = db.Participants.Find(id);
            string getQuestionListSummary = "select DISTINCT(ql.idQuestionList) as 'questionListId', ql.questionListDescription, ql.questionListName," +
        " (SELECT max(qr.attempt) FROM QuestionResult as qr WHERE qr.QuestionList_idQuestionList = ql.idQuestionList and qr.Participant_idParticipant = " + participant.idParticipant + ") as 'HighestAttempt'," +
        " (SELECT COUNT(*) FROM QuestionList_Question as qlq WHERE qlq.QuestionList_idQuestionList = ql.idQuestionList) as 'QuestionCount' from QuestionList as ql" +
        " JOIN QuestionResult Result on ql.idQuestionList = Result.QuestionList_idQuestionList AND Result.Participant_idParticipant =" + participant.idParticipant;

            List<QuestionListsummary> questionListsummaries = db.Database.SqlQuery<QuestionListsummary>(getQuestionListSummary).ToList();

            ViewBag.questionslist = questionListsummaries;


            return View(participant);
        }

        /// <summary>
        /// GET: /participants/questionlistresult/[participantId]/[questionlistId]
        ///  Shows all data for a specific participant and questionlist
        /// </summary>
        /// <param name="participantId">id of the participant to use in retrieving data</param>
        /// <param name="questionlistId">id of the questionlist to get data from</param>
        /// <returns>Returns a view with viewbags containing all data needed to display data on page</returns>
        public ActionResult QuestionlistResult(int participantId, int questionlistId)
        {
            //Get Participant model by id
            Participant participant = db.Participants.Find(participantId);

            //Get QuestionList model by id
            QuestionList qlist = db.QuestionLists.Find(questionlistId);
            ViewBag.questionListName = qlist.questionListName;

            //Query for finding the amount of questions in questionlist
            string getNumOfQuestions = "SELECT COUNT(Question_idQuestion) as numQuestions FROM QuestionList_Question WHERE QuestionList_idQuestionList =" + qlist.idQuestionList;
            int numOfQuestions = db.Database.SqlQuery<int>(getNumOfQuestions).Single();

            //Set the number of questions in a viewbag to use in the view
            ViewBag.numOfQuestions = numOfQuestions;

            //Query for finding questions from specific QuestionList 
            string getQuestions = "SELECT * FROM Question JOIN QuestionList_Question Q2 on Question.idQuestion = Q2.Question_idQuestion WHERE Q2.QuestionList_idQuestionList = " + qlist.idQuestionList;
            List<Question> questionslist = db.Database.SqlQuery<Question>(getQuestions).ToList();
            Dictionary<int, Question> list = new Dictionary<int, Question>();
            ViewBag.questions = questionslist;


            List<QuestionListResult> results = new List<QuestionListResult>();
            StringBuilder sqlQueryString = new StringBuilder();

            sqlQueryString.Append("select q.idQuestion, q.questionText,ao.answerText,ao.correctAnswer, qr.attempt, datediff(ms, qr.startTime, qr.endTime) as totalTime from QuestionResult as qr" +
                " join AnswerOption as ao on qr.AnswerOption_idAnswer = ao.idAnswer" +
                " join Question as q on q.idQuestion = ao.Question_idQuestion" +
                " where qr.QuestionList_idQuestionList = " + questionlistId +
                " and qr.Participant_idParticipant = " + participantId);
            results = db.Database.SqlQuery<QuestionListResult>(sqlQueryString.ToString()).OrderBy(qr => qr.idQuestion).OrderBy(qr => qr.attempt).ToList();

            var questions = results.Select(r => r.idQuestion).Distinct().ToList();
            List<Dictionary<int, QuestionListResult>> sortedList = new List<Dictionary<int, QuestionListResult>>();

            //Create a list with all unique attempts
            List<int> attempts = results.Select(r => r.attempt).Distinct().ToList();

            List<int> correctAnswers = new List<int>();

            //Get all correct answer for each attempt made by a participant
            foreach (int attempt in attempts)
            {
                int correctPerAttempt = results.Where(r => r.attempt == attempt && r.correctAnswer == 1).Count();
                correctAnswers.Add(correctPerAttempt);
            }
            ViewBag.correctAnswers = correctAnswers;


            ViewBag.attempts = attempts;
            if (!attempts.Count().Equals(0))
            {
                ViewBag.attemptCount = attempts.Last();
            }
            else
            {
                ViewBag.attemptCount = 0;
            }

            foreach (var question in questions)
            {
                List<QuestionListResult> questionListResults = results.Where(qr => qr.idQuestion.Equals(question)).ToList();
                Dictionary<int, QuestionListResult> dict = new Dictionary<int, QuestionListResult>();

                // Check if the question has a result for each attempt, if so add it to an dictionary
                foreach (int attempt in attempts)
                {
                    if (!questionListResults.Where(qr => qr.attempt == attempt).Count().Equals(0))
                    {
                        QuestionListResult attemptQuestion = questionListResults.Where(qr => qr.attempt == attempt).Single();
                        if (attemptQuestion != null)
                        {
                            dict.Add(attempt, attemptQuestion);
                        }
                    }
                }

                sortedList.Add(dict);
            }
            ViewBag.results = sortedList;


            return View(participant);
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
