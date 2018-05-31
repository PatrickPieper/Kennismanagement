using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using La_Game.Models;

namespace La_Game.Controllers
{
    public class LessonsController : Controller
    {
        private LaGameDBContext db = new LaGameDBContext();

        // GET: Lessons
        public ActionResult Index()
        {
            var lessons = db.Lessons.Include(l => l.Language);
            return View(lessons.ToList().Where(s => s.isHidden != 1));
        }

        // GET: Lessons/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lesson lesson = db.Lessons.Find(id);
            if (lesson == null)
            {
                return HttpNotFound();
            }
            return View(lesson);
        }

        // GET: Lessons/Create
        public ActionResult Create()
        {
            ViewBag.Language_idLanguage = new SelectList(db.Languages, "idLanguage", "languageName");
            return View();
        }

        // POST: Lessons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idLesson,Language_idLanguage,lessonName,description")] Lesson lesson)
        {
            if (ModelState.IsValid)
            {
                db.Lessons.Add(lesson);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Language_idLanguage = new SelectList(db.Languages, "idLanguage", "languageName", lesson.Language_idLanguage);
            return View(lesson);
        }

        // GET: Lessons/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lesson lesson = db.Lessons.Find(id);
            if (lesson == null)
            {
                return HttpNotFound();
            }
            ViewBag.Language_idLanguage = new SelectList(db.Languages, "idLanguage", "languageName", lesson.Language_idLanguage);
            return View(lesson);
        }

        // POST: Lessons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idLesson,Language_idLanguage,lessonName,description")] Lesson lesson)
        {
            if (ModelState.IsValid)
            {
                db.Entry(lesson).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Language_idLanguage = new SelectList(db.Languages, "idLanguage", "languageName", lesson.Language_idLanguage);
            return View(lesson);
        }

        /// <summary>
        /// GET: Lessons/Delete/[id]
        /// Find the lesson that has to be deleted and redirect to a seperate deletion page for confirmation.
        /// </summary>
        /// <param name="id"> Id of the lesson that has to be deactivated. </param>
        public ActionResult Delete(int? id)
        {
            // Check if id was given
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Try to find the lesson, if it does not exist return 404
            Lesson lesson = db.Lessons.Find(id);
            if (lesson == null)
            {
                return HttpNotFound();
            }

            // Return the delete page with the lesson information
            return View(lesson);
        }

        /// <summary>
        /// POST: Lessons/Delete/[id]
        /// After confirming that the lesson can be deleted, deactivate it in the database.
        /// </summary>
        /// <param name="id"> Id of the lesson that has to be deactivated. </param>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                // Find the questionlist and set it to hidden
                Lesson lesson = db.Lessons.Find(id);
                lesson.isHidden = 1;

                // Save the changes
                db.Entry(lesson).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {
                // Delete failed
            }

            // Redirect to index
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
