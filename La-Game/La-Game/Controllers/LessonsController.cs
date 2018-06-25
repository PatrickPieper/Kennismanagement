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
        /// GET: /Lessons?idLanguage=[idLanguage]
        /// Get a overview of all the active lessons.
        /// </summary>
        /// <param name="idLanguage"> Id of the language. </param>
        /// <param name="filter"> Optional filter for admin to see deactivated items. </param>
        public ActionResult Index(int? idLanguage, string filter)
        {
            // Check if id was given
            if (idLanguage == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Get all the active lessons and the relevant language
            var lessons = db.Lessons.Where(s => s.isHidden != 1 && s.Language_idLanguage == idLanguage);
            ViewBag.Language = db.Languages.Where(l => l.idLanguage == idLanguage).FirstOrDefault();

            // If the filter was given, use it
            if (!String.IsNullOrEmpty(filter))
            {
                switch (filter)
                {
                    case "active":
                        break;
                    case "inactive":
                        lessons = db.Lessons.Where(s => s.isHidden == 1 && s.Language_idLanguage == idLanguage);
                        break;
                    case "all":
                        lessons = db.Lessons.Where(s => s.Language_idLanguage == idLanguage);
                        break;
                }
            }

            // Return view containing the lessons
            return View(lessons.ToList());
        }

        /// <summary>
        /// GET: Lessons/Details/[id]?idLanguage=[idLanguage]
        /// Get the details of a lesson and show it on a seperate page.
        /// </summary>
        /// <param name="idLesson"> Id of the lesson. </param>
        public ActionResult Details(int? idLesson)
        {
            // Check if id was given
            if (idLesson == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Try to find the lesson, if it does not exist return 404
            Lesson lesson = db.Lessons.Find(idLesson);
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
        public ActionResult Create(int? idLanguage)
        {
            // Go to create page
            ViewBag.Language = idLanguage;
            return View();
        }

        /// <summary>
        /// POST: Lessons/Create
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

                // Redirect to list
                return RedirectToAction("Index", "Lessons", new { idLanguage = lesson.Language_idLanguage });
            }

            // If not valid, stay on the edit page with the current data
            return View(lesson);
        }

        /// <summary>
        /// GET: Lessons/Edit/[id]
        /// Find the lesson that has to be changed and redirect to a seperate edit page.
        /// </summary>
        /// <param name="idLesson"> Id of the lesson that has to be changed. </param>
        public ActionResult Edit(int? idLesson)
        {
            // Check if id was given
            if (idLesson == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Try to find the lesson, if it does not exist return 404
            Lesson lesson = db.Lessons.Find(idLesson);
            if (lesson == null)
            {
                return HttpNotFound();
            }

            // Redirect to the edit page
            return View(lesson);
        }

        /// <summary>
        /// POST: Lessons/Edit
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

                // Redirect to list
                return RedirectToAction("Index", "Lessons", new { idLanguage = lesson.Language_idLanguage });
            }

            // If not valid, stay on the edit page with the current data
            return View(lesson);
        }

        /// <summary>
        /// GET: /Lessons/Delete?idLesson=[idLesson]
        /// Find the lesson that has to be deleted and redirect to a seperate deletion page for confirmation.
        /// </summary>
        /// <param name="idLesson"> Id of the lesson that has to be deactivated. </param>
        public ActionResult Delete(int? idLesson)
        {
            // Check if id was given
            if (idLesson == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Try to find the lesson, if it does not exist return 404
            Lesson lesson = db.Lessons.Find(idLesson);
            if (lesson == null)
            {
                return HttpNotFound();
            }

            // Return the delete page with the lesson information
            return View(lesson);
        }

        /// <summary>
        /// POST: /Lessons/Delete?idLesson=[idLesson]
        /// After confirming that the lesson can be deleted, deactivate it in the database.
        /// </summary>
        /// <param name="idLesson"> Id of the lesson that has to be deactivated. </param>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int idLesson)
        {
            // Find the lesson
            Lesson lesson = db.Lessons.Find(idLesson);

            try
            {
                if (lesson.isHidden == 1)
                {
                    // If the lesson was hidden, reactivate it
                    lesson.isHidden = 0;
                }
                else
                {
                    // If the lesson was not hidden, hide it
                    lesson.isHidden = 1;
                }

                // Save the changes
                db.Entry(lesson).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {
                // Remove/Activation failed
            }

            // Redirect to list
            return RedirectToAction("Index", "Lessons", new { idLanguage = lesson.Language_idLanguage });
        }

        /// <summary>
        /// GET: Lessons/ParticipantLessonOverview/[questionListID]?lessonID=[lessonID]
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
        /// GET: Lessons/QuestionResultParticipantOverview/[questionListID]?lessonID=[lessonID]
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

        /// <summary>
        /// GET: Lessons/AddListToLesson/[idLesson]?idLanguage=[idLanguage]
        /// Navigate to a page where the list of questionlists can be found so they can be added to the lesson.
        /// </summary>
        /// <param name="idLesson"> Id of the lesson. </param>
        /// <param name="idLanguage"> Id of the language. </param>
        public ActionResult ManageLists(int? idLesson)
        {
            // Check if id was given
            if (idLesson == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Get list of all current questionlists
            List<QuestionList> allLists = (from q in db.QuestionLists select q).ToList();

            // Get all the lists that are already in the lesson
            String selectQuery = "SELECT * FROM QuestionList WHERE idQuestionList IN(SELECT QuestionList_idQuestionList FROM Lesson_QuestionList WHERE Lesson_idLesson = " + idLesson + "); ";
            List<QuestionList> questionlists = db.Database.SqlQuery<QuestionList>(selectQuery).ToList();

            // Set the id for the lesson and go to the page            
            ViewBag.idLesson = idLesson;
            return View(questionlists.Where(s => s.isHidden != 1));
        }

        /// <summary>
        /// GET: Lessons/GetQuestionListTable/[id]
        /// Return a PartialView containing a list of all questionlists in the database that are not currently in the lesson.
        /// </summary>
        /// <param name="idLesson"> Id of the lesson. </param>
        public PartialViewResult GetQuestionListTable(int? idLesson)
        {
            // Get list of all current questionlists
            List<QuestionList> allLists = (from q in db.QuestionLists select q).ToList();

            // Get all the lists that are already in the lesson
            String selectQuery = "SELECT * FROM QuestionList WHERE idQuestionList IN(SELECT QuestionList_idQuestionList FROM Lesson_QuestionList WHERE Lesson_idLesson = " + idLesson + "); ";
            List<QuestionList> currentLists = db.Database.SqlQuery<QuestionList>(selectQuery).ToList();

            // Compare the two lists and remove all the questions that are already in the list
            foreach (QuestionList list in currentLists)
            {
                allLists.RemoveAll(item => item.idQuestionList == list.idQuestionList);
            }

            // Set the lessonId and return the PartialView
            ViewBag.idLesson = idLesson;
            return PartialView("_AddQuestionListTable", allLists);
        }

        /// <summary>
        /// POST: Lessons/AddList
        /// Make a database entry to connect the questionlist to the lesson.
        /// </summary>
        /// <param name="collection"> The data that has to be added. </param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddList([Bind(Include = "Lesson_idLesson,QuestionList_idQuestionList")] FormCollection collection)
        {
            try
            {
                // Check if the neccesary ids were given
                if (collection.Get("Lesson_idLesson") == null || collection.Get("QuestionList_idQuestionList") == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                // Create the connection between list and lesson and add it to the database
                Lesson_QuestionList lesson_questionlist = new Lesson_QuestionList
                {
                    Lesson_idLesson = int.Parse(collection.Get("Lesson_idLesson")),
                    QuestionList_idQuestionList = int.Parse(collection.Get("QuestionList_idQuestionList"))
                };
                db.Lesson_QuestionList.Add(lesson_questionlist);
                db.SaveChanges();
            }
            catch
            {
                // Adding to list failed
            }

            // Redirect back to the list to reload the data
            return RedirectToAction("ManageLists", new { idLesson = collection.Get("Lesson_idLesson") });
        }

        /// <summary>
        /// POST: Lessons/RemoveList
        /// Remove the database entry that connects the questionlist to the lesson.
        /// </summary>
        /// <param name="collection"> Entry that has to be removed. </param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveList([Bind(Include = "Lesson_idLesson,QuestionList_idQuestionList")] FormCollection collection)
        {
            try
            {
                // Check if the neccesary ids were given
                if (collection.Get("Lesson_idLesson") == null || collection.Get("QuestionList_idQuestionList") == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                // Delete the connection between the lesson and the questionlist
                var deleteQuestionListEntry = "DELETE FROM Lesson_QuestionList WHERE Lesson_idLesson = " + collection.Get("Lesson_idLesson") + " AND QuestionList_idQuestionList = " + collection.Get("QuestionList_idQuestionList") + ";";
                db.Database.ExecuteSqlCommand(deleteQuestionListEntry);

                // Save the changes
                db.SaveChanges();
            }
            catch
            {
                // Failed to remove
            }

            // Redirect back to the list to reload the data
            return RedirectToAction("ManageLists", new { idLesson = collection.Get("Lesson_idLesson") });
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
