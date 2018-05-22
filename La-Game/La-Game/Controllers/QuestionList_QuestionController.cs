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
    public class QuestionList_QuestionController : Controller
    {
        private LaGameDBContext db = new LaGameDBContext();

        // GET: QuestionList_Question
        public ActionResult Index()
        {
            var questionList_Question = db.QuestionList_Question.Include(q => q.Question).Include(q => q.QuestionList);
            return View(questionList_Question.ToList());
        }

        // GET: QuestionList_Question/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuestionList_Question questionList_Question = db.QuestionList_Question.Find(id);
            if (questionList_Question == null)
            {
                return HttpNotFound();
            }
            return View(questionList_Question);
        }

        // GET: QuestionList_Question/Create
        public ActionResult Create()
        {
            ViewBag.Question_idQuestion = new SelectList(db.Questions, "idQuestion", "questionText");
            ViewBag.QuestionList_idQuestionList = new SelectList(db.QuestionLists, "idQuestionList", "questionListName");
            return View();
        }

        // POST: QuestionList_Question/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idQuestionList_Question,Question_idQuestion,QuestionList_idQuestionList")] QuestionList_Question questionList_Question)
        {
            if (ModelState.IsValid)
            {
                db.QuestionList_Question.Add(questionList_Question);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Question_idQuestion = new SelectList(db.Questions, "idQuestion", "questionText", questionList_Question.Question_idQuestion);
            ViewBag.QuestionList_idQuestionList = new SelectList(db.QuestionLists, "idQuestionList", "questionListName", questionList_Question.QuestionList_idQuestionList);
            return View(questionList_Question);
        }

        // GET: QuestionList_Question/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuestionList_Question questionList_Question = db.QuestionList_Question.Find(id);
            if (questionList_Question == null)
            {
                return HttpNotFound();
            }
            ViewBag.Question_idQuestion = new SelectList(db.Questions, "idQuestion", "questionText", questionList_Question.Question_idQuestion);
            ViewBag.QuestionList_idQuestionList = new SelectList(db.QuestionLists, "idQuestionList", "questionListName", questionList_Question.QuestionList_idQuestionList);
            return View(questionList_Question);
        }

        // POST: QuestionList_Question/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idQuestionList_Question,Question_idQuestion,QuestionList_idQuestionList")] QuestionList_Question questionList_Question)
        {
            if (ModelState.IsValid)
            {
                db.Entry(questionList_Question).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Question_idQuestion = new SelectList(db.Questions, "idQuestion", "questionText", questionList_Question.Question_idQuestion);
            ViewBag.QuestionList_idQuestionList = new SelectList(db.QuestionLists, "idQuestionList", "questionListName", questionList_Question.QuestionList_idQuestionList);
            return View(questionList_Question);
        }

        // GET: QuestionList_Question/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuestionList_Question questionList_Question = db.QuestionList_Question.Find(id);
            if (questionList_Question == null)
            {
                return HttpNotFound();
            }
            return View(questionList_Question);
        }

        // POST: QuestionList_Question/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            QuestionList_Question questionList_Question = db.QuestionList_Question.Find(id);
            db.QuestionList_Question.Remove(questionList_Question);
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
