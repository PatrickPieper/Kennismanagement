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
    public class LanguagesController : Controller
    {
        private LaGameDBContext db = new LaGameDBContext();

        // GET: Languages
        public ActionResult Index()
        {
            return View(db.Languages.ToList());
        }

        // GET: Languages/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Language language = db.Languages.Find(id);
            if (language == null)
            {
                return HttpNotFound();
            }
            return View(language);
        }

        // GET: Languages/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Languages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idLanguage,languageName")] Language language)
        {
            if (ModelState.IsValid)
            {
                db.Languages.Add(language);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(language);
        }

        // GET: Languages/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Language language = db.Languages.Find(id);
            if (language == null)
            {
                return HttpNotFound();
            }
            return View(language);
        }

        // POST: Languages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idLanguage,languageName")] Language language)
        {
            if (ModelState.IsValid)
            {
                db.Entry(language).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(language);
        }

        // GET: Languages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Language language = db.Languages.Find(id);
            if (language == null)
            {
                return HttpNotFound();
            }
            return View(language);
        }

        // POST: Languages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Language language = db.Languages.Find(id);
            db.Languages.Remove(language);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Languages/GetLanguageMembers/[1]
        public PartialViewResult GetLanguageMembers(int? id)
        {
            // Get the members from the database
            String selectQuery = "SELECT * FROM Member WHERE idMember IN(SELECT Member_idMember FROM Language_Member WHERE Language_idLanguage = " + id + ");";
            IEnumerable<Member> data = db.Database.SqlQuery<Member>(selectQuery);

            // Set the languageId and return the PartialView
            ViewBag.languageId = id;
            return PartialView("_AddLanguageMembersTable", data);
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
