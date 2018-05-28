using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
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
                db.Questions.Add(question);
                db.SaveChanges();
                var max = db.Questions.Max(q => q.idQuestion);
                String answerType = Request.Form["answerType"];
                FileImage = Request.Files[0];
                BlobsController blobsController = new BlobsController();
                CloudBlobContainer container = blobsController.GetCloudBlobContainer(max.ToString());
                containerName = container.Name;
                AnswerOptionsController answerOptionsController = new AnswerOptionsController();
                
                if (answerType == "likert")
                {
                    int count = -2;

                    while (count <= 2)
                    {
                        AnswerOption option = new AnswerOption();
                        String text = count.ToString();
                        option.answerText = text;
                        option.correctAnswer = 0;
                        option.Question_idQuestion = max;
                        answerOptionsController.Create(option);
                        count++;
                    }
                    
                }
                else if (answerType == "meerkeuze")
                {
                    string text = Request.Form["answerText"];
                    string[] answers = text.Split(',');

                    int count = 0;

                    while (count <= answers.Length - 1)
                    {
                        AnswerOption answerOption = new AnswerOption();
                        string text2 = Request.Form["correctAnswer"];
                        answerOption.answerText = answers[count];
                        answerOption.Question_idQuestion = max;
                        answerOption.correctAnswer = 0;
                        answerOptionsController.Create(answerOption);
                        count++;
                    }
                }

                if (FileImage.ContentLength > 0)
                {
                    fileName = Path.GetFileName(FileImage.FileName);
                    imageStream = FileImage.InputStream;

                    blobsController.UploadBlob(fileName, imageStream, containerName);
                    question.picture = fileName;
                }

                FileAudio = Request.Files[1];
                if (FileAudio.ContentLength > 0)
                {
                    audioName = Path.GetFileName(FileAudio.FileName);
                    audioStream = FileAudio.InputStream;

                    //Use questionnumber as last parameter to search right container
                    blobsController.UploadBlob(audioName, audioStream, containerName);
                    question.audio = audioName;
                }
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

            if(question.picture != "" && question.picture != null)
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
        public ActionResult Edit([Bind(Include = "idQuestion,picture,audio,questionText")] Question question)
        {
            if (ModelState.IsValid)
            {
                db.Entry(question).State = EntityState.Modified;
                db.SaveChanges();
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
