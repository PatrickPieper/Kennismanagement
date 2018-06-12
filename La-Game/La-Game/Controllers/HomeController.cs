using La_Game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace La_Game.Controllers
{
    public class HomeController : Controller
    {
        private readonly LaGameDBContext db = new LaGameDBContext();
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();

        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Index(StartListModel model)
        {
            
            if (ModelState.IsValid)
            {


                Participant participant = null;
                string sqlString = "SELECT * FROM QuestionList WHERE participationCode ='" + model.Participationcode + "'";
                List<QuestionList> questionListData = db.QuestionLists.SqlQuery(sqlString).ToList<QuestionList>();

                if ( model.Studentcode != null)
                {
                    sqlString = "SELECT * FROM PARTICIPANT WHERE studentCode='" + model.Studentcode +"'";


                    if (db.Participants.SqlQuery(sqlString).ToList<Participant>().Count != 0)
                    {
                        participant = db.Participants.SqlQuery(sqlString).First();
                    }
                }


                if (questionListData.Count != 0 && participant != null)
                {
                    int questionListID = questionListData[0].idQuestionList;
                    // String selectQuery = "SELECT * FROM Question WHERE idQuestion IN(SELECT Question_idQuestion FROM QuestionList_Question WHERE QuestionList_idQuestionList = " + questionListID + "); ";
                    sqlString = "SELECT q.* FROM Question AS q JOIN QuestionOrder AS qo on qo.Question_idQuestion = q.idQuestion" +
                   " JOIN QuestionList AS ql on ql.idQuestionList = qo.QuestionList_idQuestionList WHERE ql.participationCode = '" + model.Participationcode + "' ORDER BY qo.[order]";

                    List<Question> questionData = db.Questions.SqlQuery(sqlString).ToList<Question>();

                    TempData["questionListData"] = questionListData;
                    TempData["questionData"] = questionData;
                    TempData["participant"] = participant;
                    return RedirectToAction("Index", "StudentTest");
                    //View("~/Views/StudentTest/MultipleChoice.cshtml");
                }
                else if (model.Participationcode != null ||  model.Studentcode != null)
                {
                    ViewBag.Message = "something is not typed correctly";
                    return View();
                }

                if (TempData["doneMessage"] != null)
                {
                    ViewBag.doneMessage = TempData["doneMessage"];
                }
                return View();
            }
            else
            {
                ModelState.AddModelError(String.Empty, "Invalid input(s)");
            }

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