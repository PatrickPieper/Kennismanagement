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