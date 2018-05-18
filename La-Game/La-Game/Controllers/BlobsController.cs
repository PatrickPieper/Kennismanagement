using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace La_Game.Controllers
{
    public class BlobsController : Controller
    {
        // GET: Blobs
        public ActionResult Index()
        {
            return View();
        }

        private CloudBlobContainer GetCloudBlobContainer()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                    CloudConfigurationManager.GetSetting("lagame2_AzureStorageConnectionString"));
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("test-blob-container");
            return container;
        }

        public ActionResult CreateBlobContainer()
        {
            CloudBlobContainer container = GetCloudBlobContainer();

            ViewBag.Success = container.CreateIfNotExists();
            ViewBag.BlobContainerName = container.Name;

            return View();
        }

        public string UploadBlob()
        {
            String fileName = "File.jpg";
            CloudBlobContainer container = GetCloudBlobContainer();
            CloudBlockBlob blob = container.GetBlockBlobReference(fileName);
            blob.Properties.ContentType = "image/jpg";
            

            //Use filename from uploaded file here
            using (var fileStream = System.IO.File.OpenRead(@"c:\Users\ppiep\Downloads\La-Game.jpg"))
            {
                blob.UploadFromStream(fileStream);
            }
            return "success!";
        }

        public ActionResult ListBlobs()
        {
            CloudBlobContainer container = GetCloudBlobContainer();
            List<String> blobs = new List<String>();
            foreach (IListBlobItem item in container.ListBlobs(useFlatBlobListing: true))
            {
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob blob = (CloudBlockBlob)item;
                    blobs.Add(blob.Name + "" + blob.Properties.ContentLanguage);
                    
                }
            }

            return View(blobs);
        }

        public CloudBlockBlob GetCloudBlockBlob(String fileName)
        {
            CloudBlobContainer container = GetCloudBlobContainer();
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
            blockBlob.Properties.ContentType = "image/jpg";
            blockBlob.SetProperties();
            return blockBlob;
        }

        public string DeleteBlob()
        {
            CloudBlobContainer container = GetCloudBlobContainer();
            CloudBlockBlob blob = container.GetBlockBlobReference("myBlob");
            blob.Delete();
            return "success!";
        }
    }
}