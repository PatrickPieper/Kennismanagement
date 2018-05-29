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
    public class AnswerOptionsController : Controller
    {
        private LaGameDBContext db = new LaGameDBContext();

        public ActionResult AddAnswerOption()
        {
            return PartialView("_PartialView");
        }



        // GET: AnswerOptions
        public ActionResult Index()
        {
            var answerOptions = db.AnswerOptions.Include(a => a.Question);
            return View(answerOptions.ToList());
        }

        // GET: AnswerOptions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AnswerOption answerOption = db.AnswerOptions.Find(id);
            if (answerOption == null)
            {
                return HttpNotFound();
            }
            return View(answerOption);
        }

        // GET: AnswerOptions/Create
        public ActionResult Create()
        {
            ViewBag.Question_idQuestion = new SelectList(db.Questions, "idQuestion", "questionText");
            return View();
        }

        // POST: AnswerOptions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idAnswer,Question_idQuestion,correctAnswer,answerText")] AnswerOption answerOption)
        {
            if (ModelState.IsValid)
            {
                db.AnswerOptions.Add(answerOption);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Question_idQuestion = new SelectList(db.Questions, "idQuestion", "questionText", answerOption.Question_idQuestion);
            return View(answerOption);
        }

        // GET: AnswerOptions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AnswerOption answerOption = db.AnswerOptions.Find(id);
            if (answerOption == null)
            {
                return HttpNotFound();
            }
            ViewBag.Question_idQuestion = new SelectList(db.Questions, "idQuestion", "questionText", answerOption.Question_idQuestion);
            return View(answerOption);
        }

        // POST: AnswerOptions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idAnswer,Question_idQuestion,correctAnswer,answerText")] AnswerOption answerOption)
        {
            if (ModelState.IsValid)
            {
                db.Entry(answerOption).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Question_idQuestion = new SelectList(db.Questions, "idQuestion", "questionText", answerOption.Question_idQuestion);
            return View(answerOption);
        }

        // GET: AnswerOptions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AnswerOption answerOption = db.AnswerOptions.Find(id);
            if (answerOption == null)
            {
                return HttpNotFound();
            }
            return View(answerOption);
        }

        // POST: AnswerOptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AnswerOption answerOption = db.AnswerOptions.Find(id);
            db.AnswerOptions.Remove(answerOption);
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
