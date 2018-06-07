using La_Game.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
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
        #region Partial Views
        public PartialViewResult CommonWrongAnswerFilter(int? idLanguage, int? idLesson)
        {
            List<Language> lst_Language = db.Languages.ToList();
            List<Lesson> lst_Lesson;
            List<QuestionList> lst_QuestionList;

            StringBuilder lessonQueryString = new StringBuilder();
            lessonQueryString.Append("SELECT le.* FROM Lesson AS le");
            if (idLanguage != null)
            {
                lessonQueryString.Append(" WHERE le.Language_idLanguage = " + idLanguage);

            }
            lst_Lesson = db.Database.SqlQuery<Lesson>(lessonQueryString.ToString()).ToList();

            if(lst_Lesson.Count != 0)
            {
                StringBuilder questionListQueryString = new StringBuilder();
                questionListQueryString.Append("SELECT ql.* FROM QuestionList AS ql");
                if (idLesson != null)
                {
                    questionListQueryString.Append(" JOIN Lesson_QuestionList AS lq on ql.idQuestionList = lq.QuestionList_idQuestionList");
                    questionListQueryString.Append(" WHERE lq.Lesson_idLesson = " + idLesson);

                }
                lst_QuestionList = db.Database.SqlQuery<QuestionList>(questionListQueryString.ToString()).ToList();
            }
            else
            {
                lst_QuestionList = new List<QuestionList>();
            }


            ViewBag.lst_Languages = new SelectList(lst_Language, "idLanguage", "languageName");
            ViewBag.lst_Lessons = new SelectList(lst_Lesson, "idLesson", "lessonName");
            ViewBag.lst_QuestionLists = new SelectList(lst_QuestionList, "idQuestionList", "questionListName");

            return PartialView("_CommonWrongAnswerFilter");
        }
        #endregion
        #region Json Results
        public JsonResult BarChartDataCommonWrongAnswers()
        {
            var random = new Random();
            string sqlQuery = "select q.idQuestion, q.questionText,count(*) as 'wrongCount' from Question as q join AnswerOption as ao on q.idQuestion = ao.Question_idQuestion join QuestionResult as qr on ao.idAnswer = qr.AnswerOption_idAnswer where ao.correctAnswer = 0 group by q.idQuestion, q.questionText";
            var queryResult = db.Database.SqlQuery<CommonWrongQuestionResult>(sqlQuery).ToList();

            List<string> questionTexts = new List<string>();
            foreach (CommonWrongQuestionResult entry in queryResult)
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
        #endregion JsonResults
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