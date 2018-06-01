﻿using La_Game.Models;
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

        public ActionResult Index(string participationCode)
        {

            string sqlString = "SELECT * FROM QuestionList WHERE participationCode ='" + participationCode + "'";
            List<QuestionList> questionListData = db.QuestionLists.SqlQuery(sqlString).ToList<QuestionList>();

            
            if (questionListData.Count != 0)
            {
                int questionListID = questionListData[0].idQuestionList;
                String selectQuery = "SELECT * FROM Question WHERE idQuestion IN(SELECT Question_idQuestion FROM QuestionList_Question WHERE QuestionList_idQuestionList = " + questionListID + "); ";
                List<Question> questionData = db.Questions.SqlQuery(selectQuery).ToList<Question>();

                TempData["questionListData"] = questionListData;
                TempData["questionData"] = questionData;
                return RedirectToAction("Index", "StudentTest");
                    //View("~/Views/StudentTest/MultipleChoice.cshtml");
            }
            else if(participationCode != null)
            {
                ViewBag.Message = "participation code doesn't exist";
                return View();
            }
            return View();
        }

        public ActionResult Dashboard()
        {
            return View();
        }
    }
}