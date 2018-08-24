using La_Game.Models;
using La_Game.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace La_Game.Controllers
{
    /// <summary>
    /// Dashboard Controller.
    /// </summary>
    public class DashboardController : Controller
    {
        // Database Context
        private LaGameDBContext db = new LaGameDBContext();

        /// <summary>
        /// GET: /Dashboard
        /// Go to the user dashboard.
        /// </summary>
        public ActionResult Index()
        {
            // Get member from database and go to dashboard
            Member member = db.Members.Where(u => u.email == User.Identity.Name).FirstOrDefault();
            return View(member);
        }

        public ActionResult Live()
        {
            DashboardService service = new DashboardService();

            var viewModel = service.CreateLiveViewModel();
            return View(viewModel);
        }

        /// <summary>
        /// GET: /Dashboard/GetLanguageOverview?memberId=[memberId]
        /// Return a PartialView containing a list of all languages that the member belongs to.
        /// </summary>
        /// <param name="memberId"> Id of the member. </param>
        public PartialViewResult GetLanguageOverview(int? memberId)
        {
            // Get all the languages the member is assigned to
            String selectQuery = "SELECT * FROM Language WHERE idLanguage IN(SELECT Language_idLanguage FROM Language_Member WHERE Member_idMember = " + memberId + ");";
            IEnumerable<Language> data = db.Database.SqlQuery<Language>(selectQuery).Where(l => l.isHidden != 1);

            // Return the partialview containing the language table
            return PartialView("_LanguageOverview", data.ToList());
        }

        public PartialViewResult Participants(int? idQuestionlist)
        {
            if (!idQuestionlist.HasValue)
            {
                return null;
            }

            DashboardService service = new DashboardService();
            var viewModel = service.CreateLiveTableViewModel(idQuestionlist.Value);

            return PartialView("_Live", viewModel);

            
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