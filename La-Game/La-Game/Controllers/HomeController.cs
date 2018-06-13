using La_Game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace La_Game.Controllers
{
    /// <summary>
    /// Home/Dashboard Controller.
    /// </summary>
    public class HomeController : Controller
    {
        // Database Context
        private LaGameDBContext db = new LaGameDBContext();

        /// <summary>
        /// GET: /Home/Index
        /// Go to the main page.
        /// </summary>
        [AllowAnonymous]
        public ActionResult Index()
        {
            // if a doneMessage was given, put it in the ViewBag
            if (TempData["doneMessage"] != null)
            {
                ViewBag.doneMessage = TempData["doneMessage"];
            }

            // Go to the page
            return View();
        }

        /// <summary>
        /// POST: /Home/Index
        /// Check the codes that were entered and try to redirect to the questionlist.
        /// </summary>
        /// <param name="model"> The codes that were entered. </param>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Index(StartListModel model)
        {
            // Check if data is valid
            if (ModelState.IsValid)
            {
                // Get the questionlist
                string sqlString = "SELECT * FROM QuestionList WHERE participationCode ='" + model.Participationcode + "'";
                List<QuestionList> questionListData = db.QuestionLists.SqlQuery(sqlString).ToList<QuestionList>();

                // Get the participant
                Participant participant = null;
                sqlString = "SELECT * FROM PARTICIPANT WHERE studentCode='" + model.Studentcode + "'";
                if (db.Participants.SqlQuery(sqlString).ToList<Participant>().Count != 0)
                {
                    participant = db.Participants.SqlQuery(sqlString).First();
                }

                // If both the codes are valid, continue
                if (questionListData.Count != 0 && participant != null)
                {
                    // Get the questions
                    int questionListID = questionListData[0].idQuestionList;
                    sqlString = "SELECT q.* FROM Question AS q JOIN QuestionOrder AS qo on qo.Question_idQuestion = q.idQuestion" +
                    " JOIN QuestionList AS ql on ql.idQuestionList = qo.QuestionList_idQuestionList WHERE ql.participationCode = '" + model.Participationcode + "' ORDER BY qo.[order]";
                    List<Question> questionData = db.Questions.SqlQuery(sqlString).ToList<Question>();

                    // Set the tempdata and redirect to the questionlist
                    TempData["questionListData"] = questionListData;
                    TempData["questionData"] = questionData;
                    TempData["participant"] = participant;
                    return RedirectToAction("Index", "StudentTest");
                }
                else
                {
                    // One of the codes was incorrect
                    ModelState.AddModelError(String.Empty, "The participationcode and/or studentcode was not valid.");
                }
            }
            else
            {
                // The data in one of the fields was invalid
                ModelState.AddModelError(String.Empty, "Invalid input(s)");
            }

            // Stay on the page with the current data
            return View(model);
        }

        /// <summary>
        /// GET: /Home/Dashboard
        /// Go to the user dashboard.
        /// </summary>
        public ActionResult Dashboard()
        {
            // Get member from database and go to dashboard
            Member member = db.Members.Where(u => u.email == User.Identity.Name).FirstOrDefault();
            return View(member);
        }

        /// <summary>
        /// GET: /Home/GetLanguageOverview?memberId=[memberId]
        /// Return a PartialView containing a list of all languages that the member belongs to.
        /// </summary>
        /// <param name="memberId"> Id of the member. </param>
        public PartialViewResult GetLanguageOverview(int? memberId)
        {
            // Get all the languages the member is assigned to
            String selectQuery = "SELECT * FROM Language WHERE idLanguage IN(SELECT Language_idLanguage FROM Language_Member WHERE Member_idMember = " + memberId + ");";
            IEnumerable<Language> data = db.Database.SqlQuery<Language>(selectQuery);

            // Return the partialview containing the language table
            return PartialView("_LanguageOverview", data.ToList());
        }
    }
}