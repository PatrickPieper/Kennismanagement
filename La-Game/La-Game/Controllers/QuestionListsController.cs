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
        /// GET: /QuestionLists
        /// Get a overview of all active questionlists.
        /// </summary>
        /// <param name="filter"> Optional filter for admin to see deactivated items. </param>
        public ActionResult Index(string filter)
        {
            // Get all the active lists
            var lists = db.QuestionLists.Where(s => s.isHidden != 1);

            // If the filter was given, use it
            if (!String.IsNullOrEmpty(filter))
            {
                switch (filter)
                {
                    case "active":
                        break;
                    case "inactive":
                        lists = db.QuestionLists.Where(l => l.isHidden == 1);
                        break;
                    case "all":
                        lists = db.QuestionLists;
                        break;
                }
            }
            
            // Create a dictionary that shows if the list can be changed or not
            Dictionary<int, bool> dictionary = new Dictionary<int, bool>();
            foreach (QuestionList list in lists)
            {
                if (db.QuestionResults.Where(r => r.QuestionList_idQuestionList == list.idQuestionList).Count() == 0)
                {
                    // If the list has not been used, it is true
                    dictionary.Add(list.idQuestionList, true);
                }
                else
                {
                    // If the list has results, it is false
                    dictionary.Add(list.idQuestionList, false);
                }
            }
            // Put the dictionary in the viewbag
            ViewBag.dictionary = dictionary;

            // Return view containing the questionlists
            return View(lists.ToList());
        }

        #region Details and Create/Edit/Delete
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

            // Create a dictionary that shows if the list can be changed or not
            Dictionary<int, bool> dictionary = new Dictionary<int, bool>();

            if (db.QuestionResults.Where(r => r.QuestionList_idQuestionList == questionList.idQuestionList).Count() == 0)
            {
                // If the list has not been used, it is true
                dictionary.Add(questionList.idQuestionList, true);
            }
            else
            {
                // If the list has results, it is false
                dictionary.Add(questionList.idQuestionList, false);
            }
            // Put the dictionary in the viewbag
            ViewBag.dictionary = dictionary;

            // Redirect to detail page
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

            // See if the questionlist has already been used by a participant
            var results = db.QuestionResults.Where(r => r.QuestionList_idQuestionList == questionList.idQuestionList).ToList();
            if (results.Count() == 0)
            {
                // Redirect to the edit page
                return View(questionList);
            }
            else
            {
                // Redirect to overview
                return RedirectToAction("Index", "QuestionLists");
            }
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
        /// GET: /QuestionLists/Delete?idQuestionList=[idQuestionList]
        /// Find the list that has to be deleted and redirect to a seperate deletion page for confirmation.
        /// </summary>
        /// <param name="idQuestionList"> Id of the list that has to be deactivated. </param>
        public ActionResult Delete(int? idQuestionList, int? listId)
        {
            // Check if id was given
            if (idQuestionList == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Try to find the list, if it does not exist return 404
            QuestionList questionList = db.QuestionLists.Find(idQuestionList);
            if (questionList == null)
            {
                return HttpNotFound();
            }

            // Return the page with the questionlist information
            return View(questionList);
        }

        /// <summary>
        /// POST: /QuestionLists/Delete?idQuestionList=[idQuestionList]
        /// After confirming that the questionlist can be deleted, deactivate it in the database.
        /// </summary>
        /// <param name="idQuestionList"> Id of the list that has to be deactivated. </param>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int idQuestionList)
        {
            try
            {
                // Find the questionlist
                QuestionList questionList = db.QuestionLists.Find(idQuestionList);

                // See if the questionlist has already been used by a participant
                var results = db.QuestionResults.Where(r => r.QuestionList_idQuestionList == questionList.idQuestionList).ToList();
                if (results.Count() == 0)
                {
                    if (questionList.isHidden == 1)
                    {
                        // If the list was hidden, reactivate it
                        questionList.isHidden = 0;
                    }
                    else
                    {
                        // If the list was not hidden, hide it
                        questionList.isHidden = 1;

                        // Check if the questionlist is active, if true deactivate the list
                        if (questionList.isActive == 1)
                        {
                            questionList.participationCode = null;
                            questionList.isActive = 0;
                        }
                    }

                    // Save the changes
                    db.Entry(questionList).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    // Remove the questionorder
                    string deleteQuestionOrder = "DELETE FROM QuestionOrder WHERE QuestionList_idQuestionList = " + questionList + ";";
                    db.Database.ExecuteSqlCommand(deleteQuestionOrder);

                    // Remove the questions from the list
                    string deleteQuestions = "DELETE FROM QuestionList_Question WHERE QuestionList_idQuestionList = " + questionList + ";";
                    db.Database.ExecuteSqlCommand(deleteQuestions);

                    // After removing the order and the questions, delete the list
                    db.QuestionLists.Remove(questionList);
                    db.SaveChanges();
                }
            }
            catch
            {
                // Remove/Activation failed
            }

            // Redirect to index
            return RedirectToAction("Index");
        }
        #endregion

        #region Functions for adding and removing questions within the list
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
            return RedirectToAction("ModifyQuestionList", new { id = collection.Get("QuestionList_idQuestionList") });
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
            return RedirectToAction("ModifyQuestionList", new { id = collection.Get("QuestionList_idQuestionList") });
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
            return RedirectToAction("ModifyQuestionList", new { id = collection.Get("QuestionList_idQuestionList") });
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
        #endregion

        /// <summary>
        /// GET: QuestionLists/GetQuestionListTableForLesson/[id]
        /// Return a PartialView containing all the lists that belong to a certain lesson.
        /// </summary>
        /// <param name="idLesson"> Id of the lesson. </param>
        public PartialViewResult GetQuestionListsTableForLesson(int? idLesson)
        {
            // Get the lists from the database
            String selectQuery = "SELECT * FROM QuestionList WHERE idQuestionList IN(SELECT QuestionList_idQuestionList FROM Lesson_QuestionList WHERE Lesson_idLesson = " + idLesson + ");";
            IEnumerable<QuestionList> data = db.Database.SqlQuery<QuestionList>(selectQuery);

            // Set the lessonId and return the PartialView
            ViewBag.lessonID = idLesson;
            return PartialView("_AddQuestionListLessonTable", data);
        }

        /// <summary>
        /// POST: /QuestionLists/ActivateList
        /// </summary>
        /// <param name="collection"> Collection containing the necessary ids and a ActivationString </param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActivateList([Bind(Include = "idQuestionList,idLesson,ActivationString")] FormCollection collection)
        {
            // Check if id was given
            if (collection.Get("idQuestionList") == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Try to find the list, if it does not exist return 404
            QuestionList questionList = db.QuestionLists.Find(int.Parse(collection.Get("idQuestionList")));
            if (questionList != null)
            {
                // Check if you want to activate or deactivate the list
                if (collection.Get("ActivationString") == "activate" || collection.Get("ActivationString") == null)
                {
                    // Create a participationcode
                    int participationCode;
                    Random rng = new Random();
                    while (true)
                    {
                        participationCode = rng.Next(1000, 9999);
                        string sqlstring = "SELECT * FROM QuestionList WHERE participationCode = " + int.Parse(collection.Get("idQuestionList"));
                        List<QuestionList> lists = db.QuestionLists.SqlQuery(sqlstring).ToList();

                        if (lists.Count == 0)
                        {
                            // Activate the list
                            questionList.participationCode = participationCode.ToString();
                            questionList.isActive = 1;

                            // Change the database entry and save the changes
                            db.Entry(questionList).State = EntityState.Modified;
                            db.SaveChanges();

                            break;
                        }
                    }
                }
                else if (collection.Get("ActivationString") == "deactivate")
                {
                    // Deactivate the list
                    questionList.participationCode = null;
                    questionList.isActive = 0;

                    // Change the database entry and save the changes
                    db.Entry(questionList).State = EntityState.Modified;
                    db.SaveChanges();
                }

                // Return to the view
                return RedirectToAction("Details", "Lessons", new { idLesson = collection.Get("idLesson") });
            }
            else
            {
                // Questionlist was not found
                return HttpNotFound();
            }
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
