using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using La_Game.Models;

namespace La_Game.Controllers
{
    public class LessonsController : Controller
    {
        private LaGameDBContext db = new LaGameDBContext();
        
        /// <summary>
        /// GET: Lessons
        /// Get a overview of all the active lessons.
        /// </summary>
        public ActionResult Index(int? languageId)
        {
            // Include the language and then return the list of all active lessons
            var lessons = db.Lessons.Include(l => l.Language);
            return View(lessons.ToList().Where(s => s.isHidden != 1));
        }
        
        /// <summary>
        /// GET: Lessons/Details/[id]
        /// Get the details of a lesson and show it on a seperate page.
        /// </summary>
        /// <param name="id"> Id of the lesson. </param>
        public ActionResult Details(int? id)
        {
            // Check if id was given
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            // Try to find the lesson, if it does not exist return 404
            Lesson lesson = db.Lessons.Find(id);
            if (lesson == null)
            {
                return HttpNotFound();
            }

            // Redirect to the detail page
            return View(lesson);
        }
        
        /// <summary>
        /// GET: Lessons/Create
        /// Redirect to the creation page to add a new lesson to the database.
        /// </summary>
        public ActionResult Create()
        {
            // Go to create page
            ViewBag.Language = 1; //Test
            return View();
        }

        /// <summary>
        /// POST: Lessons/Create/[id]
        /// After pressing the button check if the data is valid then add it to database.
        /// </summary>
        /// <param name="lesson"> The data that has to be added. </param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idLesson,Language_idLanguage,lessonName,description")] Lesson lesson)
        {
            // Check if the data is valid
            if (ModelState.IsValid)
            {
                // If valid, add it to the database
                db.Lessons.Add(lesson);
                db.SaveChanges();

                // Redirect to index
                return RedirectToAction("Index");
            }

            // If not valid, stay on the edit page with the current data
            return View(lesson);
        }

        /// <summary>
        /// GET: Lessons/Edit/[id]
        /// Find the lesson that has to be changed and redirect to a seperate edit page.
        /// </summary>
        /// <param name="id"> Id of the lesson that has to be changed. </param>
        public ActionResult Edit(int? id)
        {
            // Check if id was given
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Try to find the lesson, if it does not exist return 404
            Lesson lesson = db.Lessons.Find(id);
            if (lesson == null)
            {
                return HttpNotFound();
            }

            // Redirect to the edit page
            return View(lesson);
        }

        /// <summary>
        /// POST: Lessons/Edit/[id]
        /// After pressing the button check if the data is valid then save it to database.
        /// </summary>
        /// <param name="lesson"> The data that has to be saved. </param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idLesson,Language_idLanguage,lessonName,description")] Lesson lesson)
        {
            // Check if the data is valid
            if (ModelState.IsValid)
            {
                // If valid, save it to the database
                db.Entry(lesson).State = EntityState.Modified;
                db.SaveChanges();

                // Redirect to index
                return RedirectToAction("Index");
            }

            // If not valid, stay on the edit page with the current data
            return View(lesson);
        }

        /// <summary>
        /// GET: Lessons/Delete/[id]
        /// Find the lesson that has to be deleted and redirect to a seperate deletion page for confirmation.
        /// </summary>
        /// <param name="id"> Id of the lesson that has to be deactivated. </param>
        public ActionResult Delete(int? id)
        {
            // Check if id was given
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Try to find the lesson, if it does not exist return 404
            Lesson lesson = db.Lessons.Find(id);
            if (lesson == null)
            {
                return HttpNotFound();
            }

            // Return the delete page with the lesson information
            return View(lesson);
        }

        /// <summary>
        /// POST: Lessons/Delete/[id]
        /// After confirming that the lesson can be deleted, deactivate it in the database.
        /// </summary>
        /// <param name="id"> Id of the lesson that has to be deactivated. </param>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                // Find the questionlist and set it to hidden
                Lesson lesson = db.Lessons.Find(id);
                lesson.isHidden = 1;

                // Save the changes
                db.Entry(lesson).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {
                // Delete failed
            }

            // Redirect to index
            return RedirectToAction("Index");
        }

        /// <summary>
        /// GET: Lessons/ParticipantLessonOverview/[id]
        /// Get a list of all participants that have done the questionlist. 
        /// </summary>
        /// <param name="questionListID"> Id of the questionlist. </param>
        /// <param name="lessonID"> Id of the lesson. </param>
        public ActionResult ParticipantLessonOverview(int? questionListID, int? lessonID)
        {
            // Check if id was given
            if (questionListID == null || lessonID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Get list of participants that have results for this questionlist+lesson combination
            String selectQuery = "SELECT DISTINCT p.* FROM Participant AS p JOIN QuestionResult AS qr on qr.Participant_idParticipant = p.idParticipant WHERE qr.QuestionList_idQuestionList = " + questionListID + " AND qr.QuestionList_idQuestionList IN(SELECT QuestionList_idQuestionList FROM Lesson_QuestionList WHERE Lesson_idLesson = " + lessonID + ") ";

            IEnumerable<Participant> data = db.Database.SqlQuery<Participant>(selectQuery);  

            // Return the overview containing the data
            ViewBag.questionListID = questionListID;
            ViewBag.lessonID = lessonID;
            return View(data);
        }
        /// <summary>
        /// GET: Lessons/QuestionResultParticipantOverview/[id]
        /// Get a list of all participants that have done the questionlist. 
        /// </summary>
        /// <param name="questionListID"> Id of the questionlist. </param>
        /// <param name="lessonID"> Id of the lesson. </param>
        public ActionResult QuestionResultParticipantOverview(int? questionListID, int? lessonID)
        {
            // Check if id was given
            if (questionListID == null || lessonID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Get list of questionresults for this questionlist+lesson combination
            String selectQuery = "SELECT qr.* FROM QuestionResult AS qr JOIN Participant AS p on qr.Participant_idParticipant = p.idParticipant WHERE qr.QuestionList_idQuestionList = " + questionListID + " AND qr.QuestionList_idQuestionList IN(SELECT QuestionList_idQuestionList FROM Lesson_QuestionList WHERE Lesson_idLesson = " + lessonID + ") ";
            IEnumerable<QuestionResult> data = db.Database.SqlQuery<QuestionResult>(selectQuery);

            // Return the overview containing the data
            return View(data);
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
