using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using La_Game.Models;

namespace La_Game.Controllers
{
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

        public ActionResult Results(int id)
        {
            Participant participant = db.Participants.Find(id);
            List<List<KeyValuePair<String, object>>> questionlists = new List<List<KeyValuePair<String, object>>>();
            List<int> listIds = this.GetLists(participant.idParticipant);
            foreach (int questionlistId in listIds)
            {
                String qListName = db.QuestionLists.Where(q => q.idQuestionList.Equals(questionlistId)).Select(q => q.questionListName).Single();
                List<KeyValuePair<String, object>> questionList = new List<KeyValuePair<String, object>>();
                questionList.Add(new KeyValuePair<string, object>("ID", questionlistId));
                questionList.Add(new KeyValuePair<string, object>("Name", qListName));
                List<String> questions = new List<String>();
                List<int> answerIds = GetAnswerIds(participant.idParticipant, questionlistId);
                List<int> question_ids = GetQuestionIds(questionlistId);
                int numOfQuestions = question_ids.Count();
                questionList.Add(new KeyValuePair<string, object>("countQuestions", numOfQuestions));

                int correctAnswerCount = 0;
                List<AnswerOption> answers = new List<AnswerOption>();
                foreach (var answerid in answerIds)
                {
                    short? correct = db.AnswerOptions.Where(a => a.idAnswer.Equals(answerid)).Select(a => a.correctAnswer).Single();
                    if (correct == 1)
                    {
                        correctAnswerCount++;
                    }
                }



                foreach (int question_id in question_ids)
                {
                    IEnumerable<AnswerOption> correctAnswer = db.AnswerOptions.Where(ao => ao.Question_idQuestion.Equals(question_id) && ao.correctAnswer == 1);
                    IEnumerable<String> question = db.Questions.Where(q => q.idQuestion.Equals(question_id)).Select(q => q.questionText);
                    IEnumerable<Question> list_questions = db.Questions.Where(q => q.idQuestion.Equals(question_id));

                    questions.Add(question.Single().ToString());
                }
                questionList.Add(new KeyValuePair<string, object>("correctAnswerCount", correctAnswerCount));
                questionList.Add(new KeyValuePair<string, object>("Questions", questions));
                questionlists.Add(questionList);
            }
            //ViewBag.answers = answers;
            ViewBag.questionslist = questionlists;


            return View(participant);
        }
        
        public List<int> GetLists(int participantId)
        {
            List<int> listIds = db.QuestionResults.Where(q => q.Participant_idParticipant.Equals(participantId)).Select(q => q.QuestionList_idQuestionList).Distinct().ToList();
            return listIds;
        }

        public List<int> GetQuestionIds(int questionListId)
        {
            List<int> questionIds = db.QuestionList_Question.Where(q => q.QuestionList_idQuestionList.Equals(questionListId)).Select(q => q.Question_idQuestion).ToList();
            return questionIds;
        }

        public List<int> GetAnswerIds(int participantId, int questionlistId)
        {
            List<int> answerIds = db.QuestionResults.Where(q => q.Participant_idParticipant.Equals(participantId) && q.QuestionList_idQuestionList.Equals(questionlistId)).Select(q => q.AnswerOption_idAnswer).ToList();
            return answerIds;
        }

        public ActionResult QuestionlistResult(int participantId, int questionlistId)
        {
            Participant participant = db.Participants.Find(participantId);
            QuestionList qlist = db.QuestionLists.Find(questionlistId);
            ViewBag.questionList = qlist;

            List<KeyValuePair<int, AnswerOption>> givenanswerList = new List<KeyValuePair<int, AnswerOption>>();

            List<int> answerids = GetAnswerIds(participantId, questionlistId);

            List<AnswerOption> givenAnswers = db.AnswerOptions.Where(ao => answerids.Any(s => ao.idAnswer.Equals(s))).ToList();

            foreach (var givenAnswer in givenAnswers)
            {
                int questionId = db.Questions.Where(q => q.idQuestion.Equals(givenAnswer.Question_idQuestion)).Select(q => q.idQuestion).Single();
                givenanswerList.Add(new KeyValuePair<int, AnswerOption>(questionId, givenAnswer));
            }


            List<KeyValuePair<int, AnswerOption>> correctAnswerList = new List<KeyValuePair<int, AnswerOption>>();

            List<int> questionIds = GetQuestionIds(questionlistId);

            List<Question> questions = db.Questions.Where(q => questionIds.Any(s => q.idQuestion.Equals(s))).ToList();

            List<int?> attempts = db.QuestionResults.Where(q => q.Participant_idParticipant.Equals(participantId) && q.QuestionList_idQuestionList.Equals(questionlistId)).Select(q => q.attempt).Distinct().ToList();

            List<KeyValuePair<int?, List<AnswerOption>>> results = new List<KeyValuePair<int?, List<AnswerOption>>>();

            foreach (var attempt in attempts)
            {
                List<int> answerOptionIds = db.QuestionResults.Where(q => q.Participant_idParticipant.Equals(participantId) && q.QuestionList_idQuestionList.Equals(questionlistId) && q.attempt == attempt).Select(q => q.AnswerOption_idAnswer).ToList();
                List<AnswerOption> answers = db.AnswerOptions.Where(q => answerOptionIds.Any(s => q.idAnswer.Equals(s))).ToList();
                results.Add(new KeyValuePair<int?, List<AnswerOption>>(attempt, answers));
            }
            ViewBag.givenAnswers = results;
            ViewBag.questions = questions;

            List<AnswerOption> correctAnswers = db.AnswerOptions.Where(q => questionIds.Any(s => q.Question_idQuestion.Equals(s)) && q.correctAnswer == 1).ToList();

            foreach (var correctAnswer in correctAnswers)
            {
                int questionId = db.Questions.Where(q => q.idQuestion.Equals(correctAnswer.Question_idQuestion)).Select(q => q.idQuestion).Single();
                correctAnswerList.Add(new KeyValuePair<int, AnswerOption>(questionId, correctAnswer));
            }
            ViewBag.correctAnswers = correctAnswerList;

            return View(participant);
        }

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
