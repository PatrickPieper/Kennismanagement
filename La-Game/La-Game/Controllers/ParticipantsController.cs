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

                // Return a new create view
                return View();
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
            List<List<KeyValuePair<String,object>>> questionlists = new List<List<KeyValuePair<String, object>>>();
            IEnumerable<int> listIds = db.QuestionResults.Where(q => q.Participant_idParticipant.Equals(participant.idParticipant)).Select(q => q.QuestionList_idQuestionList);
            foreach (int questionlistId in listIds)
            {
                String qListName = db.QuestionLists.Where(q => q.idQuestionList.Equals(questionlistId)).Select(q => q.questionListName).Single();
                List<KeyValuePair<String, object>> questionList = new List<KeyValuePair<String, object>>();
                questionList.Add(new KeyValuePair<string, object>("ID",questionlistId));
                questionList.Add(new KeyValuePair<string, object>("Name", qListName));
                List<String> questions = new List<String>();
                IEnumerable<int> question_ids = db.QuestionList_Question.Where(q => q.QuestionList_idQuestionList.Equals(questionlistId)).Select(q => q.Question_idQuestion);

                foreach(int question_id in question_ids)
                {
                    IEnumerable<String> question = db.Questions.Where(q => q.idQuestion.Equals(question_id)).Select(q => q.questionText);
                    IEnumerable<Question> list_questions = db.Questions.Where(q => q.idQuestion.Equals(question_id));

                    questions.Add(question.Single().ToString());
                }
                questionList.Add(new KeyValuePair<string, object>("Questions", questions));
                questionlists.Add(questionList);
            }
            ViewBag.questionslist = questionlists;


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
