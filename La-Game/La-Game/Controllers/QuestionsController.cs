using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using La_Game.Models;
using La_Game.ViewModels;
using Microsoft.WindowsAzure.Storage.Blob;

namespace La_Game.Controllers
{
    public class QuestionsController : Controller
    {
        private LaGameDBContext db = new LaGameDBContext();
        private string fileName;
        private string audioName;
        private Stream audioStream;
        private Stream imageStream;
        private string containerName;
        BlobsController blobsController = new BlobsController();
        AnswerOptionsController answerOptionsController = new AnswerOptionsController();
        private string fileUpdateName;

        // GET: Questions
        public ActionResult Index()
        {
            return View(db.Questions.ToList());
        }

        // GET: Questions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        // GET: Questions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Questions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idQuestion,picture,audio,questionText")] Question question, HttpPostedFileBase FileImage, HttpPostedFileBase FileAudio)
        {
            if (ModelState.IsValid)
            {

                String answerType = Request.Form["answerType"];
                // If likert scale is selected get the value of the choosing option for likert scale and write it to the database.
                if (answerType == "likert")
                {                  
                    byte test = byte.Parse(Request.Form["likertOption"]);                    
                    question.likertScale = test;
                }

                var max = db.Questions.Max(q => q.idQuestion) + 1;                
                CloudBlobContainer container = blobsController.GetCloudBlobContainer(max.ToString());
                containerName = container.Name;
                

                // Checks if there is a image uploaded.
                // If there is a image upload it to the blob and write the filename to the database.
                FileImage = Request.Files[0];
                if (FileImage.ContentLength > 0)
                {
                    fileName = Path.GetFileName(FileImage.FileName);
                    imageStream = FileImage.InputStream;

                    blobsController.UploadBlob(fileName, imageStream, containerName);
                    question.picture = fileName;

                }

                // Checks if there is a audiofile uploaded
                // If there is a audiofile upload it to the blob and write the filename to the database.
                FileAudio = Request.Files[1];
                if (FileAudio.ContentLength > 0)
                {
                    audioName = Path.GetFileName(FileAudio.FileName);
                    audioStream = FileAudio.InputStream;

                    //Use questionnumber as last parameter to search right container.
                    blobsController.UploadBlob(audioName, audioStream, containerName);
                    question.audio = audioName;
                }

                // Get the question Text from the from and add it to the database with Multilingual.
                string qText = question.questionText;
                string queryText = "INSERT INTO Question(questionText, picture, audio) VALUES (N'" + qText +"', '" + fileName +"','" + audioName +"')";
                db.Database.ExecuteSqlCommand(queryText);

                // If the Question option is likert write 5 anwser to the database with values -2 to 2.
                if (answerType == "likert")
                {
                    int count = -2;

                    while (count <= 2)
                    {
                        AnswerOption option = new AnswerOption();
                        String text = count.ToString();
                        option.answerText = text;
                        option.correctAnswer = 1;
                        option.Question_idQuestion = max;
                        answerOptionsController.Create(option);
                        count++;
                    }

                }
                // If multiple choice is selected put the anwsers in a array and write it to the database.
                
                else if (answerType == "multiplechoice")
                {
                    string text = Request.Form["answerText"];
                    string[] answers = text.Split(',');
                    string correct = Request.Form["correctAnswer"];
                    string[] bools = correct.Split(',');


                    int count = 0;

                    while (count <= answers.Length - 1)
                    {
                        AnswerOption answerOption = new AnswerOption();
                        string text2 = Request.Form["correctAnswer"];
                        answerOption.answerText = answers[count];
                        answerOption.Question_idQuestion = max;
                        if (bools[count] == "1")
                        {
                            answerOption.correctAnswer = 1;
                        }
                        else if (bools[count] == "0")
                        {
                            answerOption.correctAnswer = 0;
                        }

                        answerOptionsController.Create(answerOption);
                        count++;
                    }
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(question);
        }

        // GET: Questions/Edit/5
        public ActionResult Edit(int? id)
        {
            String idString = id.ToString();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            BlobsController blobsController = new BlobsController();

            CloudBlobContainer container = blobsController.GetCloudBlobContainer(idString);

            if (question.picture != "" && question.picture != null)
            {
                CloudBlockBlob blob = container.GetBlockBlobReference(question.picture);
                ViewData["Blob"] = blob;
            }
            return View(question);
        }

        // POST: Questions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idQuestion,picture,audio,questionText")] Question question, HttpPostedFileBase ImageUpdate, HttpPostedFileBase AudioUpdate)
        {
            if (ModelState.IsValid)
            {
                CloudBlobContainer container = blobsController.GetCloudBlobContainer(question.idQuestion.ToString());
                int id = question.idQuestion;
                containerName = container.Name;


                // Checks if there is a image uploaded.
                // If there is a image upload it to the blob and update the filename in the database.
                ImageUpdate = Request.Files[0];
                if (ImageUpdate.ContentLength > 0)
                {
                    fileUpdateName = ImageUpdate.FileName;

                    fileName = Path.GetFileName(ImageUpdate.FileName);
                    imageStream = ImageUpdate.InputStream;

                    blobsController.UploadBlob(fileUpdateName, imageStream, containerName);
                    string queryText = "UPDATE Question SET picture = '" + fileUpdateName + "' WHERE idQuestion = " + id;
                    db.Database.ExecuteSqlCommand(queryText);

                }

                // Checks if there is a audio uploaded.
                // If there is a audio upload it to the blob and update the filename in the database.
                AudioUpdate = Request.Files[1];
                if (AudioUpdate.ContentLength > 0)
                {
                    fileUpdateName = AudioUpdate.FileName;

                    fileName = Path.GetFileName(AudioUpdate.FileName);
                    imageStream = AudioUpdate.InputStream;

                    blobsController.UploadBlob(fileUpdateName, imageStream, containerName);
                    string queryText = "UPDATE Question SET audio = '" + fileUpdateName + "' WHERE idQuestion = " + id;
                    db.Database.ExecuteSqlCommand(queryText);

                }

                // Update the Question in the database.
                string updateQuestion = question.questionText;
                string queryQuestion = "Update Question SET questionText = (N'" + updateQuestion + "') WHERE idQuestion = " + id;
                db.Database.ExecuteSqlCommand(queryQuestion);


                return RedirectToAction("Index");
            }


            return View(question);
        }

        // GET: Questions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        // POST: Questions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Question question = db.Questions.Find(id);
            db.Questions.Remove(question);
            db.SaveChanges();
            return RedirectToAction("Index");
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
