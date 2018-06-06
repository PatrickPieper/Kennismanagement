using La_Game.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace La_Game.Controllers
{
    public class StatisticsController : Controller
    {
        private LaGameDBContext db = new LaGameDBContext();
        // GET: Statistics
        public ActionResult Index()
        {
            return View();
        }
        // GET: QuestionStatistics
        public ActionResult QuestionStatistics()
        {
            return View();
        }
        public ActionResult CommonWrongAnswers()
        {
            return View();
        }
        public JsonResult BarChartDataCommonWrongAnswers()
        {
            var random = new Random();
            string sqlQuery = "select q.idQuestion, q.questionText,count(*) as 'wrongCount' from Question as q join AnswerOption as ao on q.idQuestion = ao.Question_idQuestion join QuestionResult as qr on ao.idAnswer = qr.AnswerOption_idAnswer where ao.correctAnswer = 0 group by q.idQuestion, q.questionText";
            var queryResult = db.Database.SqlQuery<CommonWrongQuestionResult>(sqlQuery).ToList();

            List<string> questionTexts = new List<string>();
            foreach(CommonWrongQuestionResult entry in queryResult)
            {
                questionTexts.Add(entry.idQuestion + ":" + entry.questionText);
            }
            List<int> questionData = new List<int>();
            foreach (CommonWrongQuestionResult entry in queryResult)
            {
                questionData.Add(entry.wrongCount);
            }
            List<string> barColors = new List<string>();
            foreach (CommonWrongQuestionResult entry in queryResult)
            {
                barColors.Add(String.Format("#{0:X6}", random.Next(0x1000000)));
            }

            Chart _chart = new Chart();
            _chart.labels = questionTexts.ToArray();
            _chart.datasets = new List<Datasets>();
            List<Datasets> _dataSet = new List<Datasets>();
            _dataSet.Add(new Datasets()
            {
                label = "Number of Wrong Answers",
                data = questionData.ToArray(),
                backgroundColor = barColors.ToArray(),
                borderColor = barColors.ToArray(),
                borderWidth = "1"
            });
            _chart.datasets = _dataSet;
            return Json(_chart, JsonRequestBehavior.AllowGet);
        }

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