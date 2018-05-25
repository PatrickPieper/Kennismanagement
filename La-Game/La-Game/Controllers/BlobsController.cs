using System;
using System.Collections.Generic;
using System.IO;
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

        public CloudBlobContainer GetCloudBlobContainer(String containerName = "")
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
            CloudConfigurationManager.GetSetting("lagame2_AzureStorageConnectionString"));
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);
            return container;
        }

        public ActionResult CreateBlobContainer(String name)
        {
            CloudBlobContainer container = GetCloudBlobContainer(name);

            ViewBag.Success = container.CreateIfNotExists();
            ViewBag.BlobContainerName = container.Name;

            return View();
        }

        public string UploadBlob(String fileName, Stream inputStream, String questionNumber)
        {
            CloudBlobContainer container = GetCloudBlobContainer("questions");
            CloudBlockBlob blob = container.GetBlockBlobReference(fileName);
            string[] strings = fileName.Split('.');
            
            switch(strings[strings.Length - 1])
            {
                case "jpg":
                    blob.Properties.ContentType = "image/jpg";
                    break;
            }

            blob.UploadFromStream(inputStream);

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
                    blobs.Add(blob.Name);
                    
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