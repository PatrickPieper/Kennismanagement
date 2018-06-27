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
    public class QuestionResultsController : Controller
    {
        private LaGameDBContext db = new LaGameDBContext();

        // GET: QuestionResults
        public ActionResult Index()
        {
            var questionResults = db.QuestionResults.Include(q => q.AnswerOption).Include(q => q.Participant).Include(q => q.QuestionList);
            return View(questionResults.ToList());
        }

        // GET: QuestionResults/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuestionResult questionResult = db.QuestionResults.Find(id);
            if (questionResult == null)
            {
                return HttpNotFound();
            }
            return View(questionResult);
        }

        // GET: QuestionResults/Create
        public ActionResult Create()
        {
            ViewBag.AnswerOption_idAnswer = new SelectList(db.AnswerOptions, "idAnswer", "answerText");
            ViewBag.Participant_idParticipant = new SelectList(db.Participants, "idParticipant", "firstName");
            ViewBag.QuestionList_idQuestionList = new SelectList(db.QuestionLists, "idQuestionList", "questionListName");
            return View();
        }

        // POST: QuestionResults/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idQuestionResult,QuestionList_idQuestionList,AnswerOption_idAnswer,Participant_idParticipant,startTime,endTime")] QuestionResult questionResult)
        {
            if (ModelState.IsValid)
            {
                db.QuestionResults.Add(questionResult);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AnswerOption_idAnswer = new SelectList(db.AnswerOptions, "idAnswer", "answerText", questionResult.AnswerOption_idAnswer);
            ViewBag.Participant_idParticipant = new SelectList(db.Participants, "idParticipant", "firstName", questionResult.Participant_idParticipant);
            ViewBag.QuestionList_idQuestionList = new SelectList(db.QuestionLists, "idQuestionList", "questionListName", questionResult.QuestionList_idQuestionList);
            return View(questionResult);
        }

        // GET: QuestionResults/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuestionResult questionResult = db.QuestionResults.Find(id);
            if (questionResult == null)
            {
                return HttpNotFound();
            }
            ViewBag.AnswerOption_idAnswer = new SelectList(db.AnswerOptions, "idAnswer", "answerText", questionResult.AnswerOption_idAnswer);
            ViewBag.Participant_idParticipant = new SelectList(db.Participants, "idParticipant", "firstName", questionResult.Participant_idParticipant);
            ViewBag.QuestionList_idQuestionList = new SelectList(db.QuestionLists, "idQuestionList", "questionListName", questionResult.QuestionList_idQuestionList);
            return View(questionResult);
        }

        // POST: QuestionResults/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idQuestionResult,QuestionList_idQuestionList,AnswerOption_idAnswer,Participant_idParticipant,startTime,endTime")] QuestionResult questionResult)
        {
            if (ModelState.IsValid)
            {
                db.Entry(questionResult).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AnswerOption_idAnswer = new SelectList(db.AnswerOptions, "idAnswer", "answerText", questionResult.AnswerOption_idAnswer);
            ViewBag.Participant_idParticipant = new SelectList(db.Participants, "idParticipant", "firstName", questionResult.Participant_idParticipant);
            ViewBag.QuestionList_idQuestionList = new SelectList(db.QuestionLists, "idQuestionList", "questionListName", questionResult.QuestionList_idQuestionList);
            return View(questionResult);
        }

        // GET: QuestionResults/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuestionResult questionResult = db.QuestionResults.Find(id);
            if (questionResult == null)
            {
                return HttpNotFound();
            }
            return View(questionResult);
        }

        // POST: QuestionResults/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            QuestionResult questionResult = db.QuestionResults.Find(id);
            db.QuestionResults.Remove(questionResult);
            db.SaveChanges();
            return RedirectToAction("Index");
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
