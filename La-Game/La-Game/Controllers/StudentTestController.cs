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

        // GET: StudentTest
        [HttpPost]
        public ActionResult Index(int? index)
        {
            ViewBag.index = index;
            return View();
        }
    }
}