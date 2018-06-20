using La_Game.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
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
        // GET: LessonStatistics
        public ActionResult LessonStatistics()
        {
            return View();
        }
        // GET: CommonWrongAnswers
        public ActionResult CommonWrongAnswers()
        {
            return View();
        }
        // GET: CompareLessons
        public ActionResult CompareLessons()
        {
            return View();
        }


        public ActionResult QuestionListStatistics(int? questionListId, int attempt = 1)
        {
            string sqlString = "SELECT * FROM QuestionList";
            List<QuestionList> questionLists = db.QuestionLists.SqlQuery(sqlString).ToList<QuestionList>();
            ViewBag.questionLists = questionLists;
            
            if(questionListId == null)
            {
                questionListId = db.QuestionLists.SqlQuery(sqlString).First().idQuestionList;
            }
                sqlString = "select p.idParticipant, p.firstName, p.lastName, min(qr.startTime) as 'startTime', max(qr.endTime) as 'endTime', sum(datediff(millisecond, qr.startTime,qr.endTime)) as 'totalTime',qr.attempt, count(cao.correctAnswer) as 'correctCount', count(wao.correctAnswer) as 'wrongCount' from QuestionResult as qr " +
                                    "left join AnswerOption as cao on qr.AnswerOption_idAnswer = cao.idAnswer and cao.correctAnswer = 1 " +
                                    "left join AnswerOption as wao on qr.AnswerOption_idAnswer = wao.idAnswer and wao.correctAnswer = 0 " +
                                    "left join Participant as p on qr.Participant_idParticipant = p.idParticipant " +
                                    "join QuestionList as ql on qr.QuestionList_idQuestionList = ql.idQuestionList " +
                                    "join Lesson_QuestionList as lq on ql.idQuestionList = lq.QuestionList_idQuestionList " +
                                    "where ql.idQuestionList = " + questionListId +
                                    " AND attempt = " + attempt +
                                    " group by p.firstName, p.lastName, qr.attempt, p.idParticipant";
                List<QuestionListStatisticsModel> questionListStatistics = db.Database.SqlQuery<QuestionListStatisticsModel>(sqlString).ToList();
                ViewBag.questionListStatistics = questionListStatistics;

                sqlString = "SELECT QuestionList_idQuestionList as idQuestionList," +
                    " (select count(*) FROM QuestionList JOIN QuestionList_Question on QuestionList.idQuestionList = QuestionList_Question.QuestionList_idQuestionList where QuestionList.idQuestionList = 28) as QuestionCount, MAX(attempt) as MaxAttempt FROM QuestionList" +
                    " join QuestionResult on QuestionList.idQuestionList = QuestionResult.QuestionList_idQuestionList" +
                    " where QuestionList.idQuestionList = " + questionListId +
                    " group by QuestionList_idQuestionList";
                ViewBag.questionListInfo = db.Database.SqlQuery<QuestionListInfoModel>(sqlString).ToList();

                sqlString = "Select questionListName FROM QuestionList where idQuestionList=" + questionListId;
                ViewBag.questionListName = db.Database.SqlQuery<string>(sqlString).First();
                ViewBag.attempt = attempt;
                ViewBag.questionListId = questionListId;
            
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
        /// <summary>
        /// GET: CompareLessonSelection
        /// </summary>
        /// <param name="idLanguage">Language filter</param>
        /// <param name="idLesson">Lesson filter</param>
        /// <returns>Partial view for the lesson comparison selection</returns>
        public PartialViewResult CompareLessonSelection(int? idLanguage, int? idLesson)
        {
            List<Language> lst_Language = db.Languages.ToList();
            List<Lesson> lst_Lesson;

            //Create the query string for lessons
            StringBuilder lessonQueryString = new StringBuilder();
            lessonQueryString.Append("SELECT le.* FROM Lesson AS le");
            //If a language id is given, a language has been set and narrow the lesson results to said language
            if (idLanguage != null && idLanguage != -1)
            {
                lessonQueryString.Append(" WHERE le.Language_idLanguage = " + idLanguage);

            }
            lst_Lesson = db.Database.SqlQuery<Lesson>(lessonQueryString.ToString()).ToList();

            //Create the selectlists for the dropdowns
            ViewBag.lst_Languages = new SelectList(lst_Language, "idLanguage", "languageName");
            ViewBag.lst_Lessons = new SelectList(lst_Lesson, "idLesson", "lessonName");

            return PartialView("_CompareLessonSelection");
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
            //Append the filter values to the query, if set
            if (idLanguage != null && idLanguage != -1)
            {
                sqlQueryString.Append(" and la.idLanguage = " + idLanguage);
            }
            if (idLesson != null && idLesson != -1)
            {
                sqlQueryString.Append(" and le.idLesson = " + idLesson);
            }
            if (idQuestionList != null && idQuestionList != -1)
            {
                sqlQueryString.Append(" and ql.idQuestionList = " + idQuestionList);
            }
            sqlQueryString.Append(" group by q.idQuestion, q.questionText");
            var queryResult = db.Database.SqlQuery<CommonWrongQuestionResult>(sqlQueryString.ToString()).OrderByDescending(o => o.wrongCount).ToList();

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

            //Create new chart and dataset
            Chart _chart = new Chart();
            _chart.labels = questionTexts.ToArray();
            _chart.datasets = new List<Datasets>();
            List<Datasets> _dataSet = new List<Datasets>();
            _dataSet.Add(new Datasets()
            {
                label = "Number of Wrong Answers",
                data = questionData.ToArray(),
                backgroundColor = barColors[0],
                borderColor = barColors[0],
                hoverBackgroundColor = barColors[0],
                borderWidth = "1"
            });
            _chart.datasets = _dataSet;
            return Json(_chart, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Method to get bar chart data as JsonResult from database
        /// </summary>
        /// <param name="idLanguage">Language selection</param>
        /// <param name="idLesson">Lesson selection</param>
        /// <returns>JsonResult containing 2 datasets, based on the language/lesson combination</returns>
        public JsonResult BarChartDataCompareLessons(int? idLanguage, int? idLesson)
        {
            var random = new Random();
            //Query to select questions that were answered wrongly the most, with their amount
            StringBuilder sqlQueryString = new StringBuilder();
            sqlQueryString.Append("select le.idLesson, count(cao.correctAnswer) as 'correctCount', count(wao.correctAnswer) as 'wrongCount' from QuestionResult as qr" +
                                " left join AnswerOption as cao on qr.AnswerOption_idAnswer = cao.idAnswer and cao.correctAnswer = 1" +
                                " left join AnswerOption as wao on qr.AnswerOption_idAnswer = wao.idAnswer and wao.correctAnswer = 0" +
                                " join QuestionList as ql on qr.QuestionList_idQuestionList = ql.idQuestionList" +
                                " join Lesson_QuestionList as lq on ql.idQuestionList = lq.QuestionList_idQuestionList" +
                                " join Lesson as le on lq.Lesson_idLesson = le.idLesson");
            //Append the filter values to the query, if set
            if (idLanguage != null && idLanguage != -1)
            {
                sqlQueryString.Append(" where le.Language_idLanguage = " + idLanguage);
            }
            if (idLesson != null && idLesson != -1)
            {
                sqlQueryString.Append(" and le.idLesson = " + idLesson);
            }
            sqlQueryString.Append(" group by le.idLesson");
            var queryResult = db.Database.SqlQuery<CompareLessonResult>(sqlQueryString.ToString()).First();

            //Create two datasets, one for correct, one for wrong answers
            List<Datasets> _dataSet = new List<Datasets>();
            _dataSet.Add(new Datasets()
            {
                label = "Correct Answers",
                data = new int[] {queryResult.correctCount },
                backgroundColor = ColorTranslator.ToHtml(Color.Green),
                borderColor = ColorTranslator.ToHtml(Color.Green),
                hoverBackgroundColor = ColorTranslator.ToHtml(Color.Green),
                borderWidth = "1"
            });
            _dataSet.Add(new Datasets()
            {
                label = "Wrong Answers",
                data = new int[] { queryResult.wrongCount },
                backgroundColor = ColorTranslator.ToHtml(Color.Red),
                borderColor =  ColorTranslator.ToHtml(Color.Red),
                hoverBackgroundColor = ColorTranslator.ToHtml(Color.Red),
                borderWidth = "1"
            });

            return Json(_dataSet, JsonRequestBehavior.AllowGet);
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