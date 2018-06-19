using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web.Mvc;
using La_Game.Models;

namespace La_Game.Controllers
{
    public class LanguagesController : Controller
    {
        private LaGameDBContext db = new LaGameDBContext();

        /// <summary>
        /// GET: /Languages
        /// Get a overview of all the current languages.
        /// </summary>
        /// <param name="filter"> Optional filter for admin to see deactivated items. </param>
        [AuthorizeAdmin]
        public ActionResult Index(string filter)
        {
            // Get all the active languages
            var languages = db.Languages.Where(l => l.isHidden != 1);

            // If the filter was given, use it
            if (!String.IsNullOrEmpty(filter))
            {
                switch (filter)
                {
                    case "active":
                        break;
                    case "inactive":
                        languages = db.Languages.Where(l => l.isHidden == 1);
                        break;
                    case "all":
                        languages = db.Languages;
                        break;
                }
            }

            // Return view containing the language list
            return View(languages.ToList());
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
        [AuthorizeAdmin]
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
        [AuthorizeAdmin]
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
        [AuthorizeAdmin]
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
        [AuthorizeAdmin]
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
        /// GET: Languages/Delete?idLanguage=[idLanguage]
        /// Find the language that has to be deleted and redirect to a seperate deletion page for confirmation.
        /// </summary>
        /// <param name="idLanguage"> Id of the language that has to be deactivated. </param>
        [AuthorizeAdmin]
        public ActionResult Delete(int? idLanguage)
        {
            // Check if id was given
            if (idLanguage == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Try to find the language, if it does not exist return 404
            Language language = db.Languages.Find(idLanguage);
            if (language == null)
            {
                return HttpNotFound();
            }

            // Return the delete page with the language information
            return View(language);
        }

        /// <summary>
        /// POST: Languages/Delete/?idLanguage=[idLanguage]
        /// After confirming that the language can be deleted, deactivate it in the database.
        /// </summary>
        /// <param name="idLanguage"> Id of the language that has to be deactivated. </param>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeAdmin]
        public ActionResult DeleteConfirmed(int idLanguage)
        {
            try
            {
                // Find the member and check the status
                Language language = db.Languages.Find(idLanguage);
                if (language.isHidden == 1)
                {
                    // If language was hidden, reactivate it
                    language.isHidden = 0;
                }
                else
                {
                    // If language was not hidden, hide it
                    language.isHidden = 1;
                }

                // Save the changes to the database
                db.Entry(language).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {
                // Remove/Activation failed
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
