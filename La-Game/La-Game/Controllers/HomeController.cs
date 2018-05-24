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

        public ActionResult Index(string participationCode)
        {

            string sqlString = "SELECT * FROM QuestionList WHERE participationCode ='" + participationCode + "'";
            List<QuestionList> data = db.QuestionLists.SqlQuery(sqlString).ToList<QuestionList>();


            if (data.Count != 0)
            {
                ViewBag.data = data;
                return View("../StudentTest/MultipleChoice");
            }
            else if(participationCode != null)
            {
                ViewBag.Message = "participation code doesn't exist";
                return View();
            }
            return View();
        }

        
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}