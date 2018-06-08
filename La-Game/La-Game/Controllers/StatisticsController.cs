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
        /// <summary>
        /// GET: CommonWrongAnswerFilter
        /// </summary>
        /// <param name="idLanguage">Language filter</param>
        /// <param name="idLesson">Lesson filter</param>
        /// <returns>Partial view for the common wrong answer filter</returns>
        public PartialViewResult CommonWrongAnswerFilter(int? idLanguage, int? idLesson)
        {
            List<Language> lst_Language = db.Languages.ToList();
            List<Lesson> lst_Lesson;
            List<QuestionList> lst_QuestionList;

            //Create the query string for lessons
            StringBuilder lessonQueryString = new StringBuilder();
            lessonQueryString.Append("SELECT le.* FROM Lesson AS le");
            //If a language id is given, a language has been set and narrow the lesson results to said language
            if (idLanguage != null && idLanguage != -1)
            {
                lessonQueryString.Append(" WHERE le.Language_idLanguage = " + idLanguage);

            }
            lst_Lesson = db.Database.SqlQuery<Lesson>(lessonQueryString.ToString()).ToList();

            //Check if there's no lessons in the previous result, if there is none, there's also no questionlists to be displayed
            if (lst_Lesson.Count != 0)
            {
                StringBuilder questionListQueryString = new StringBuilder();
                questionListQueryString.Append("SELECT ql.* FROM QuestionList AS ql");
                //Check if lesson is set, if it is, narrow the results based down to lessons
                if (idLesson != null && idLesson != -1)
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

            //Create the selectlists for the dropdowns
            ViewBag.lst_Languages = new SelectList(lst_Language, "idLanguage", "languageName");
            ViewBag.lst_Lessons = new SelectList(lst_Lesson, "idLesson", "lessonName");
            ViewBag.lst_QuestionLists = new SelectList(lst_QuestionList, "idQuestionList", "questionListName");

            return PartialView("_CommonWrongAnswerFilter");
        }
        #endregion
        #region Json Results
        /// <summary>
        /// Method to create the barchart data for the common wrong answers page
        /// </summary>
        /// <returns>Returns a JsonResult with the required chart.js data</returns>
        public JsonResult BarChartDataCommonWrongAnswers(int? idLanguage, int? idLesson, int? idQuestionList)
        {
            var random = new Random();
            //Query to select questions that were answered wrongly the most, with their amount
            StringBuilder sqlQueryString = new StringBuilder();
            sqlQueryString.Append("select q.idQuestion, q.questionText,count(*) as 'wrongCount' from Question as q " +
                                "join AnswerOption as ao on q.idQuestion = ao.Question_idQuestion " +
                                "join QuestionResult as qr on ao.idAnswer = qr.AnswerOption_idAnswer " +
                                "left join QuestionList_Question as qlq on qlq.Question_idQuestion = q.idQuestion " +
                                "left join QuestionList as ql on qlq.QuestionList_idQuestionList = ql.idQuestionList " +
                                "left join Lesson_QuestionList as lq on ql.idQuestionList = lq.QuestionList_idQuestionList " +
                                "left join Lesson as le on lq.Lesson_idLesson = le.idLesson " +
                                "left join[Language] as la on le.Language_idLanguage = la.idLanguage " +
                                "where ao.correctAnswer = 0 ");
            //string sqlQuery = "select q.idQuestion, q.questionText,count(*) as 'wrongCount' from Question as q join AnswerOption as ao on q.idQuestion = ao.Question_idQuestion join QuestionResult as qr on ao.idAnswer = qr.AnswerOption_idAnswer where ao.correctAnswer = 0 group by q.idQuestion, q.questionText";

            if (idLanguage != null && idLanguage != -1)
            {
                sqlQueryString.Append(" and la.idLanguage = " + idLanguage);
            }
            if (idLesson != null && idLesson != -1)
            {
                sqlQueryString.Append(" and le.idLesson = " + idLesson);
            }
            if(idQuestionList != null && idQuestionList != -1)
            {
                sqlQueryString.Append(" and ql.idQuestionList = " + idQuestionList);
            }
            sqlQueryString.Append(" group by q.idQuestion, q.questionText");
            var queryResult = db.Database.SqlQuery<CommonWrongQuestionResult>(sqlQueryString.ToString()).ToList();

            //Turn query result into two separate list for use as data/labels for chart.js
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
            //For every result, generate a random bar color
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