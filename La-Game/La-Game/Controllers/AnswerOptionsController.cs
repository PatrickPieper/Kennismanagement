using System.Web.Mvc;
using La_Game.Models;

namespace La_Game.Controllers
{
    /// <summary>
    /// Answeroption Controller.
    /// </summary>
    public class AnswerOptionsController : Controller
    {
        // Database context
        private LaGameDBContext db = new LaGameDBContext();

        /// <summary>
        /// GET: /AnswerOptions/AddAnswerOption
        /// Get the partialview where you can add the answeroption.
        /// </summary>
        public ActionResult AddAnswerOption()
        {
            // Return the partialview
            return PartialView("_QuestionPartial");
        }
        
        /// <summary>
        /// POST: /AnswerOptions/Create
        /// Create a new AnswerOption in the database.
        /// </summary>
        /// <param name="answerOption"> The answeroption that has to be added. </param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idAnswer,Question_idQuestion,correctAnswer,answerText")] AnswerOption answerOption)
        {
            // Check if the data is valid
            if (ModelState.IsValid)
            {
                // If valid, add it to the database
                db.AnswerOptions.Add(answerOption);
                db.SaveChanges();

                // Redirect to index
                return RedirectToAction("Index");
            }

            // Add modelerror and redirect to index
            ModelState.AddModelError(string.Empty, "Something went wrong while adding the answeroption.");
            return RedirectToAction("Index");
        }
    }
}
