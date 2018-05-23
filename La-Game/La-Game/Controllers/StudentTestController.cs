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
        private readonly LaGameDBContext db = new LaGameDBContext();

        // GET: StudentTest
        public ActionResult Index(string participationCode)
        {
            if (participationCode == null)            
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
                return View();
        }
    }
}