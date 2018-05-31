using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using La_Game.Models;

namespace La_Game.Controllers
{
    public class StudentTestController : Controller
    {
        private LaGameDBContext db = new LaGameDBContext();
        // GET: StudentTest
        public ActionResult Index(int? index, string studentAnswer)
        {
            if (index == null)
            {
                ViewBag.index = 0;
            }
            else
            {
                index++;
                ViewBag.index = index;
            }


            if (TempData["questionListData"] != null && TempData["questionData"] != null)
            { 
                ViewBag.questionListData = TempData["questionListData"];
                ViewBag.questionData = TempData["questionData"];
            }

            if (TempData["answerOptions"] == null)
            {
                string selectQuery = "SELECT * FROM AnswerOption INNER JOIN Question on AnswerOption.Question_idQuestion=Question.idQuestion;";
                List<AnswerOption> answerOptions = db.AnswerOptions.SqlQuery(selectQuery).ToList<AnswerOption>();
                ViewBag.answerOptions = answerOptions;
            }
            else
            {
                ViewBag.answerOptions = TempData["answerOptions"];
            }

            if(studentAnswer != null && TempData["startTime"] != null)
            {
                string endTime = DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss:fff");
                DateTime startTime = DateTime.ParseExact((string)TempData["startTime"],"yyyy:MM:dd HH:mm:ss:fff",null);
                

            }
            
            //to do: when we have a boolean for LikertScale or MultipleChoice return the right view
            return View("MultipleChoice");
        }

        
        public ActionResult MultipleChoice()
        {
            return View();
           
        }

        public ActionResult LikertScale(int? index)
        {
            return View();

        }

    }
}