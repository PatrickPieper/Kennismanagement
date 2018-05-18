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

        // GET: QuestionLists/DeleteQuestionFromList/5
        public ActionResult DeleteQuestionFromList(int? id, int? listId)
        {
            if (id == null || listId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            QuestionList_Question question = db.QuestionList_Question.Find(id);

            if (question == null)
            {
                return HttpNotFound();
            }

            db.QuestionList_Question.Remove(question);
            db.SaveChanges();
            
            return View();
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
