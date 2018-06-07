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

        public ActionResult Index(string participationCode,string firstName, string lastName, string studentCode)
        {
            Participant participant =null;
            string sqlString = "SELECT * FROM QuestionList WHERE participationCode ='" + participationCode + "'";
            List<QuestionList> questionListData = db.QuestionLists.SqlQuery(sqlString).ToList<QuestionList>();

            if (firstName != null && lastName != null && studentCode != null)
            { 
                sqlString = "SELECT * FROM PARTICIPANT WHERE studentCode='" + studentCode + "' AND firstName='" + firstName + "' AND lastName='" + lastName+"'";


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
                " JOIN QuestionList AS ql on ql.idQuestionList = qo.QuestionList_idQuestionList WHERE ql.participationCode = '" + participationCode + "' ORDER BY qo.[order]";

                List<Question> questionData = db.Questions.SqlQuery(sqlString).ToList<Question>();

                TempData["questionListData"] = questionListData;
                TempData["questionData"] = questionData;
                TempData["participant"] = participant;
                return RedirectToAction("Index", "StudentTest");
                    //View("~/Views/StudentTest/MultipleChoice.cshtml");
            }
            else if(participationCode != null || firstName != null || lastName != null || studentCode != null)
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

        public ActionResult Dashboard()
        {
            return View();
        }

        /// <summary>
        /// GET: Home/GetLanguageOverview/[memberId]
        /// Return a PartialView containing a list of all languages that the member belongs to.
        /// </summary>
        /// <param name="memberId"> Id of the member. </param>
        public PartialViewResult GetLanguageOverview(int? memberId)
        {
            // Get the lists from the database
            String selectQuery = "SELECT * FROM Language WHERE idLanguage IN(SELECT Language_idLanguage FROM Language_Member WHERE Member_idMember = " + memberId + ");";
            IEnumerable<Language> data = db.Database.SqlQuery<Language>(selectQuery);

            return PartialView("_LanguageOverview", data.ToList());
        }
    }
}