using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using La_Game.Models;

namespace La_Game.Controllers
{
    public class LanguagesController : Controller
    {
        private LaGameDBContext db = new LaGameDBContext();

        /// <summary>
        /// GET: Languages
        /// Get a overview of all the current languages.
        /// </summary>
        public ActionResult Index()
        {
            // Return view containing all the languages
            return View(db.Languages.ToList());
        }

        /// <summary>
        /// GET: Languages/Details/[id]
        /// Get the details of a language and show it on a seperate page.
        /// </summary>
        /// <param name="id"> Id of the language. </param>
        public ActionResult Details(int? id)
        {
            // Check if id was given
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Try to find the language, if it does not exist return 404
            Language language = db.Languages.Find(id);
            if (language == null)
            {
                return HttpNotFound();
            }

            // Redirect to the detail page
            return View(language);
        }

        /// <summary>
        /// GET: Languages/Create
        /// Redirect to the creation page to add a new language to the database.
        /// </summary>
        public ActionResult Create()
        {
            // Go to create page
            return View();
        }

        /// <summary>
        /// POST: Lessons/Create
        /// After pressing the button check if the data is valid then add it to the database.
        /// </summary>
        /// <param name="language"> The data that has to be added. </param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idLanguage,languageName")] Language language)
        {
            // Check if the data is valid
            if (ModelState.IsValid)
            {
                // If valid, add it to the database
                db.Languages.Add(language);
                db.SaveChanges();

                // Redirect to index
                return RedirectToAction("Index");
            }

            // If not valid, stay on the edit page with the current data
            return View(language);
        }

        /// <summary>
        /// GET: Languages/Edit/[id]
        /// Find the language that has to be changed and redirect to a seperate edit page.
        /// </summary>
        /// <param name="id"> Id of the language that has to be changed. </param>
        public ActionResult Edit(int? id)
        {
            // Check if id was given
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Try to find the language, if it does not exist return 404
            Language language = db.Languages.Find(id);
            if (language == null)
            {
                return HttpNotFound();
            }

            // Redirect to the edit page
            return View(language);
        }

        /// <summary>
        /// POST: Languages/Edit
        /// After pressing the button check if the data is valid then save it to database.
        /// </summary>
        /// <param name="language"> The data that has to be saved. </param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idLanguage,languageName")] Language language)
        {
            // Check if the data is valid
            if (ModelState.IsValid)
            {
                // If valid, save it to the database
                db.Entry(language).State = EntityState.Modified;
                db.SaveChanges();

                // Redirect to index
                return RedirectToAction("Index");
            }

            // If not valid, stay on the edit page with the current data
            return View(language);
        }

        /// <summary>
        /// GET: Languages/Delete/[id]
        /// Find the language that has to be deleted and redirect to a seperate deletion page for confirmation.
        /// </summary>
        /// <param name="id"> Id of the language that has to be deactivated. </param>
        public ActionResult Delete(int? id)
        {
            // Check if id was given
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Try to find the language, if it does not exist return 404
            Language language = db.Languages.Find(id);
            if (language == null)
            {
                return HttpNotFound();
            }

            // Return the delete page with the language information
            return View(language);
        }

        /// <summary>
        /// POST: Languages/Delete/[id]
        /// After confirming that the language can be deleted, deactivate it in the database.
        /// </summary>
        /// <param name="id"> Id of the language that has to be deactivated. </param>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                // TODO - Add isHidden field to language
                Language language = db.Languages.Find(id);
                db.Languages.Remove(language);
                db.SaveChanges();

                // Find the language and set it to hidden
                //Language language = db.Languages.Find(id);
                //language.isHidden = 1;

                // Save the changes
                //db.Entry(language).State = EntityState.Modified;
                //db.SaveChanges();
            }
            catch
            {
                // Delete failed
            }

            // Redirect to index
            return RedirectToAction("Index");
        }

        /// <summary>
        /// GET: Languages/GetLanguageMembers/[id]
        /// Get the list of members that belong to the language.
        /// </summary>
        /// <param name="id"> Id of the language. </param>
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
