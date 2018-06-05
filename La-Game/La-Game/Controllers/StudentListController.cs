using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using La_Game.Models;

namespace La_Game.Controllers
{
    public class StudentListController : Controller
    {
        private LaGameDBContext db = new LaGameDBContext();
        // GET: StudentList
        public ActionResult Index()
        {            
            return View("CreateCodes");
        }

        

        public ActionResult CreateCodes(int? total, int? totalParticipants)
        {
            Random rnd = new Random();
            int? unique = rnd.Next(10000, 99999);
            unique = total;

            return View();
        }
    }
}