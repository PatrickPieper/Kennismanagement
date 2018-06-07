using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using La_Game.Models;

namespace La_Game.Controllers
{
    public class QuestionListsController : Controller
    {
        private LaGameDBContext db = new LaGameDBContext();

        /// <summary>
        /// GET: QuestionLists
        /// Get a overview of all active questionlists.
        /// </summary>
        public ActionResult Index()
        {
            // Return the list of all active lists
            return View(db.QuestionLists.ToList().Where(s => s.isHidden != 1));
        }

        /// <summary>
        /// GET: QuestionLists/Details/[id]
        /// Get the details of a list and show it on a seperate page.
        /// </summary>
        /// <param name="id"> Id of the questionlist. </param>
        public ActionResult Details(int? id)
        {
            // Check if id was given
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Try to find the list, if it does not exist return 404
            QuestionList questionList = db.QuestionLists.Find(id);
            if (questionList == null)
            {
                return HttpNotFound();
            }

            // Get the questions belonging to the list and put the result in the viewbag
            String selectQuery = "SELECT q.* FROM Question AS q JOIN QuestionOrder AS qo on qo.Question_idQuestion = q.idQuestion JOIN QuestionList AS ql on ql.idQuestionList = qo.QuestionList_idQuestionList WHERE ql.idQuestionList = " + id + " ORDER BY qo.[order]";
            IEnumerable<Question> data = db.Database.SqlQuery<Question>(selectQuery);
            ViewBag.questions = data.ToList();

            // Redirect to the detail page
            return View(questionList);
        }

        /// <summary>
        /// GET: QuestionLists/Create
        /// Redirect to the creation page to add a new questionlist to the database.
        /// </summary>
        public ActionResult Create()
        {
            // Go to create page
            return View();
        }

        /// <summary>
        /// POST: QuestionLists/Create
        /// After pressing the button check if the data is valid then add it to database.
        /// </summary>
        /// <param name="questionList"> The data that has to be added. </param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idQuestionList,Lesson_idLesson,questionListName,questionListDescription,participationCode,isActive")] QuestionList questionList)
        {
            // Check if the data is valid
            if (ModelState.IsValid)
            {
                // If valid, add it to the database
                db.QuestionLists.Add(questionList);
                db.SaveChanges();

                // Redirect to the page where questions can be assigned to the list
                return RedirectToAction("ModifyQuestionList", new { id = questionList.idQuestionList });
            }

            // If not valid, stay on the edit page with the current data
            return View(questionList);
        }

        /// <summary>
        /// GET: QuestionLists/Edit/[id]
        /// Find the list that has to be changed and redirect to a seperate edit page.
        /// </summary>
        /// <param name="id"> Id of the questionlist that has to be changed. </param>
        public ActionResult Edit(int? id)
        {
            // Check if id was given
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Try to find the lesson, if it does not exist return 404
            QuestionList questionList = db.QuestionLists.Find(id);
            if (questionList == null)
            {
                return HttpNotFound();
            }

            // Redirect to the edit page
            return View(questionList);
        }

        /// <summary>
        /// POST: QuestionLists/Edit
        /// After pressing the button check if the data is valid then save it to database.
        /// </summary>
        /// <param name="questionList"> The data that has to be saved. </param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idQuestionList,Lesson_idLesson,questionListName,questionListDescription,participationCode,isActive")] QuestionList questionList)
        {
            // Check if the data is valid
            if (ModelState.IsValid)
            {
                // If valid, save it to the database
                db.Entry(questionList).State = EntityState.Modified;
                db.SaveChanges();

                // Redirect to index
                return RedirectToAction("Index");
            }

            // If not valid, stay on the edit page with the current data
            return View(questionList);
        }

        /// <summary>
        /// GET: QuestionLists/Delete/[id]
        /// Find the list that has to be deleted and redirect to a seperate deletion page for confirmation.
        /// </summary>
        /// <param name="id"> Id of the list that has to be deactivated. </param>
        public ActionResult Delete(int? id, int? listId)
        {
            // Check if id was given
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Try to find the list, if it does not exist return 404
            QuestionList questionList = db.QuestionLists.Find(id);
            if (questionList == null)
            {
                return HttpNotFound();
            }

            // Return the delete page with the questionlist information
            return View(questionList);
        }

        /// <summary>
        /// POST: QuestionLists/Delete/[id]
        /// After confirming that the questionlist can be deleted, deactivate it in the database.
        /// </summary>
        /// <param name="id"> Id of the list that has to be deactivated. </param>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                // Find the questionlist and set it to hidden
                QuestionList questionList = db.QuestionLists.Find(id);
                questionList.isHidden = 1;

                // Check if the questionlist is active, if true deactivate the list
                if (questionList.isActive == 1)
                {
                    questionList.participationCode = null;
                    questionList.isActive = 0;
                }

                // Save the changes
                db.Entry(questionList).State = EntityState.Modified;
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
        /// GET: QuestionLists/ModifyQuestionList/[id]
        /// Navigate to page where questions can be added and/or removed from the list.
        /// </summary>
        /// <param name="id"> Id of the questionlist. </param>
        public ActionResult ModifyQuestionList(int? id)
        {
            // Check if id was given
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Get all the questions that are currently in the questionlist
            String selectQuery = "SELECT q.* FROM Question AS q JOIN QuestionOrder AS qo on qo.Question_idQuestion = q.idQuestion JOIN QuestionList AS ql on ql.idQuestionList = qo.QuestionList_idQuestionList WHERE ql.idQuestionList = " + id + " ORDER BY qo.[order]";
            IEnumerable<Question> questions = db.Database.SqlQuery<Question>(selectQuery);
            ViewBag.questions = questions;

            // Set the listId and go to the page
            ViewBag.listId = id;
            return View(questions);
        }

        /// <summary>
        /// POST: QuestionLists/ModifyQuestionList
        /// Make a database entry to connect the question to the list.
        /// </summary>
        /// <param name="collection"> The data that has to be added. </param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddQuestionToList([Bind(Include = "Question_idQuestion,QuestionList_idQuestionList")] FormCollection collection)
        {
            try
            {
                // Check if the neccesary ids were given
                if (collection.Get("Question_idQuestion") == null || collection.Get("QuestionList_idQuestionList") == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                // Create the connection between question and list and add it to the database
                QuestionList_Question questionlist_question = new QuestionList_Question
                {
                    Question_idQuestion = int.Parse(collection.Get("Question_idQuestion")),
                    QuestionList_idQuestionList = int.Parse(collection.Get("QuestionList_idQuestionList"))
                };
                db.QuestionList_Question.Add(questionlist_question);
                db.SaveChanges();

                // Get all questions linked to questionlist
                String selectQuery = "SELECT * FROM Question WHERE idQuestion IN(SELECT Question_idQuestion FROM QuestionList_Question WHERE QuestionList_idQuestionList = " + collection.Get("QuestionList_idQuestionList") + "); ";
                IEnumerable<Question> data = db.Database.SqlQuery<Question>(selectQuery);
                List<Question> allPresentQuestions = data.ToList();

                // Get QuestionOrders for the current questionlist, ordered by "order" weight
                int idQuestionList = int.Parse(collection.Get("QuestionList_idQuestionList"));
                List<QuestionOrder> allQuestionListOrder = db.QuestionOrders.Where(s => s.QuestionList_idQuestionList == idQuestionList).OrderBy(o => o.order).ToList();

                // Add the new QuestionOrder entry to the database
                QuestionOrder questionOrderToAdd = new QuestionOrder
                {
                    // If list count is not 0, highest order + 100, else 100
                    order = allQuestionListOrder.Count != 0 ? allQuestionListOrder[allQuestionListOrder.Count - 1].order + 100 : 100,
                    QuestionList_idQuestionList = int.Parse(collection.Get("QuestionList_idQuestionList")),
                    Question_idQuestion = int.Parse(collection.Get("Question_idQuestion"))
                };
                db.QuestionOrders.Add(questionOrderToAdd);
                db.SaveChanges();
            }
            catch
            {
                // Failed to add question to the list
            }

            // Redirect back to the list to reload the data
            return RedirectToAction("ModifyQuestionList", new { idLesson = collection.Get("QuestionList_idQuestionList") });
        }

        /// <summary>
        /// POST: QuestionLists/MoveQuestionInList
        /// Move the question from one position to another by changing the ordervalue.
        /// </summary>
        /// <param name="collection"> The data needed to determine positions. </param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MoveQuestionInList([Bind(Include = "Question_idQuestion,QuestionList_idQuestionList,movedTo,upDown")] FormCollection collection)
        {
            // Check if the neccesary ids were given
            if (collection.Get("Question_idQuestion") == null || collection.Get("QuestionList_idQuestionList") == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Get all the necessary data
            int idQuestionList = int.Parse(collection.Get("QuestionList_idQuestionList"));
            int idQuestion = int.Parse(collection.Get("Question_idQuestion"));
            int indexMovedTo = int.Parse(collection.Get("movedTo"));
            int upDown = int.Parse(collection.Get("upDown"));
            List<QuestionOrder> allQuestionListOrder = db.QuestionOrders.Where(s => s.QuestionList_idQuestionList == idQuestionList).OrderBy(o => o.order).ToList();

            double questionToWeigh = 0;
            // upDown 0 is up, 1 is down
            if (upDown == 0)
            {
                if (indexMovedTo != 0)
                {
                    questionToWeigh = (double)allQuestionListOrder[indexMovedTo - 1].order;
                }
            }
            else
            {
                if (indexMovedTo != allQuestionListOrder.Count - 1)
                {
                    questionToWeigh = (double)allQuestionListOrder[indexMovedTo + 1].order;
                }
                else
                {
                    questionToWeigh = (double)allQuestionListOrder.Last().order + 100;
                }
            }

            double questionReplace = (double)allQuestionListOrder[indexMovedTo].order;

            // Replace the old order with the new value
            QuestionOrder question = db.QuestionOrders.Where(s => s.QuestionList_idQuestionList == idQuestionList && s.Question_idQuestion == idQuestion).OrderBy(o => o.order).First();
            question.order = (questionToWeigh + questionReplace) / 2;

            // Save the changes
            db.Entry(question).State = EntityState.Modified;
            db.SaveChanges();

            // Redirect back to the list to reload the data
            return RedirectToAction("ModifyQuestionList", new { idLesson = collection.Get("QuestionList_idQuestionList") });
        }

        /// <summary>
        /// POST: QuestionLists/DeleteQuestionFromList
        /// Remove the database entry that connects the question to the questionlist.
        /// </summary>
        /// <param name="collection"> Entry that has to be removed. </param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteQuestionFromList([Bind(Include = "Question_idQuestion,QuestionList_idQuestionList")] FormCollection collection)
        {
            try
            {
                // Check if the necessary ids were given
                if (collection.Get("Question_idQuestion") == null || collection.Get("QuestionList_idQuestionList") == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                // Delete the connection between the question and the list
                var deleteQuestionEntry = "DELETE FROM QuestionList_Question WHERE Question_idQuestion = " + collection.Get("Question_idQuestion") + " AND QuestionList_idQuestionList = " + collection.Get("QuestionList_idQuestionList") + ";";
                db.Database.ExecuteSqlCommand(deleteQuestionEntry);

                // Delete the entry in the QuestionOrder table
                var deleteQuestionOrder = "DELETE FROM QuestionOrder WHERE Question_idQuestion = " + collection.Get("Question_idQuestion") + " AND QuestionList_idQuestionList = " + collection.Get("QuestionList_idQuestionList") + ";";
                db.Database.ExecuteSqlCommand(deleteQuestionOrder);

                // Save the changes
                db.SaveChanges();
            }
            catch
            {
                // Failed to delete
            }
            
            // Redirect back to the list to reload the data
            return RedirectToAction("ModifyQuestionList", new { idLesson = collection.Get("QuestionList_idQuestionList") });
        }

        /// <summary>
        /// GET: QuestionLists/GetQuestionTable/[id]
        /// Return a PartialView containing a list of all relevant questions in the database that are not currently in the list.
        /// </summary>
        /// <param name="id"> Id of the questionlist. </param>
        public PartialViewResult GetQuestionTable(int? id)
        {
            // Get list of all questions
            List<Question> allQuestions = (from q in db.Questions select q).ToList();

            // Get all the questions that are already in the list
            String selectQuery = "SELECT * FROM Question WHERE idQuestion IN(SELECT Question_idQuestion FROM QuestionList_Question WHERE QuestionList_idQuestionList = " + id + "); ";
            List<Question> questionsAlreadyInQuestionList = db.Database.SqlQuery<Question>(selectQuery).ToList();

            // Compare the two lists and remove all the questions that are already in the list
            foreach (Question questionToRemove in questionsAlreadyInQuestionList)
            {
                allQuestions.RemoveAll(item => item.idQuestion == questionToRemove.idQuestion);
            }

            // Set the listId and return the PartialView
            ViewBag.listId = id;
            return PartialView("_AddQuestionTable", allQuestions);
        }

        /// <summary>
        /// GET: QuestionLists/GetQuestionListTableForLesson/[id]
        /// Return a PartialView containing all the lists that belong to a certain lesson.
        /// </summary>
        /// <param name="id"> Id of the lesson. </param>
        public PartialViewResult GetQuestionListsTableForLesson(int? id)
        {
            // Get the lists from the database
            String selectQuery = "SELECT * FROM QuestionList WHERE idQuestionList IN(SELECT QuestionList_idQuestionList FROM Lesson_QuestionList WHERE Lesson_idLesson = " + id + ");";
            IEnumerable<QuestionList> data = db.Database.SqlQuery<QuestionList>(selectQuery);
            
            // Set the lessonId and return the PartialView
            ViewBag.lessonID = id;
            return PartialView("_AddQuestionListLessonTable", data);
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
