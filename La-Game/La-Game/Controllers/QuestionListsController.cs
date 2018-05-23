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

        // GET: QuestionLists
        public ActionResult Index()
        {
            var questionLists = db.QuestionLists.Include(q => q.Lesson);
            return View(questionLists.ToList());
        }

        // GET: QuestionLists/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuestionList questionList = db.QuestionLists.Find(id);
            if (questionList == null)
            {
                return HttpNotFound();
            }
            String selectQuery = "SELECT * FROM Question WHERE idQuestion IN(SELECT Question_idQuestion FROM QuestionList_Question WHERE QuestionList_idQuestionList = " + id + "); ";
            IEnumerable<Question> data = db.Database.SqlQuery<Question>(selectQuery);

            ViewBag.questions = data.ToList();

            return View(questionList);
        }

        #region Create QuestionList
        // GET: QuestionLists/Create
        public ActionResult Create()
        {
            ViewBag.Lesson_idLesson = new SelectList(db.Lessons, "idLesson", "lessonName");
            return View();
        }

        // POST: QuestionLists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idQuestionList,Lesson_idLesson,questionListName,questionListDescription,participationCode,isActive")] QuestionList questionList)
        {
            if (ModelState.IsValid)
            {
                db.QuestionLists.Add(questionList);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Lesson_idLesson = new SelectList(db.Lessons, "idLesson", "lessonName", questionList.Lesson_idLesson);
            return View(questionList);
        }
        #endregion

        #region Edit QuestionList
        // GET: QuestionLists/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuestionList questionList = db.QuestionLists.Find(id);
            if (questionList == null)
            {
                return HttpNotFound();
            }
            ViewBag.Lesson_idLesson = new SelectList(db.Lessons, "idLesson", "lessonName", questionList.Lesson_idLesson);
            return View(questionList);
        }

        // POST: QuestionLists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idQuestionList,Lesson_idLesson,questionListName,questionListDescription,participationCode,isActive")] QuestionList questionList)
        {
            if (ModelState.IsValid)
            {
                db.Entry(questionList).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Lesson_idLesson = new SelectList(db.Lessons, "idLesson", "lessonName", questionList.Lesson_idLesson);
            return View(questionList);
        }
        #endregion

        #region Delete Questionlist
        // GET: QuestionLists/Delete/5
        public ActionResult Delete(int? id, int? listId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            QuestionList questionList = db.QuestionLists.Find(id);

            if (questionList == null)
            {
                return HttpNotFound();
            }

            return View(questionList);
        }

        // POST: QuestionLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            QuestionList questionList = db.QuestionLists.Find(id);
            db.QuestionLists.Remove(questionList);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        #endregion

        #region Adding/Deleting questions from the list
        public ActionResult ModifyQuestionList(int? id, string filter)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Filter by name
            String selectQuery = "SELECT * FROM Question WHERE idQuestion IN(SELECT Question_idQuestion FROM QuestionList_Question WHERE QuestionList_idQuestionList = " + id + "); ";
            IEnumerable<Question> questions = db.Database.SqlQuery<Question>(selectQuery);

            //If a name was given, use it to filter the results
            if (!String.IsNullOrEmpty(filter))
            {
                questions = questions.Where(s => s.questionText.Contains(filter));
            }

            ViewBag.listId = id;
            return View(questions);
        }

        public ActionResult AddQuestionToList(int? id, int? listId)
        {
            try
            {
                if (id == null || listId == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                
                var add = "INSERT INTO QuestionList_Question (Question_idQuestion, QuestionList_idQuestionList) VALUES (" + id + ", " + listId + ");"; 
                db.Database.ExecuteSqlCommand(add);
                db.SaveChanges();
            }
            catch
            {
                // Failed to add
            }

            return RedirectToAction("ModifyQuestionList", new { id = listId });
        }

        // GET: QuestionLists/DeleteQuestionFromList/5?listId=2
        public ActionResult DeleteQuestionFromList(int? id, int? listId)
        {
            try
            {
                if (id == null || listId == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var delete = "DELETE FROM QuestionList_Question WHERE Question_idQuestion = " + id + " AND QuestionList_idQuestionList = " + listId + ";";
                db.Database.ExecuteSqlCommand(delete);
                db.SaveChanges();
            }
            catch
            {
                // Failed to delete
            }

            return RedirectToAction("ModifyQuestionList", new { id = listId });
        }

        public PartialViewResult GetQuestionTable(int? id)
        {
            //Filter by name
            List<Question> allQuestions = (from q in db.Questions select q).ToList();

            String selectQuery = "SELECT * FROM Question WHERE idQuestion IN(SELECT Question_idQuestion FROM QuestionList_Question WHERE QuestionList_idQuestionList = " + id + "); ";
            List<Question> questionsAlreadyInQuestionList = db.Database.SqlQuery<Question>(selectQuery).ToList();

            foreach (Question questionToRemove in questionsAlreadyInQuestionList)
            {
                allQuestions.RemoveAll(item => item.idQuestion == questionToRemove.idQuestion);
            }

            //If a name was given, use it to filter the results
            ViewBag.listId = id;
            return PartialView("_AddQuestionTable", allQuestions);
        }
        #endregion

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
