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
        /// <summary>
        /// Retrieves BlobContainer by given name
        /// </summary>
        /// <param name="containerName">Name of the container to find</param>
        /// <returns>the found blobContainer</returns>
        public CloudBlobContainer GetCloudBlobContainer(String containerName = "")
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
            CloudConfigurationManager.GetSetting("lagame2_AzureStorageConnectionString"));
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);
            return container;
        }

        /// <summary>
        /// Uploads blobs to folder of the questions
        /// </summary>
        /// <param name="fileName">Name of the file to write in folder</param>
        /// <param name="inputStream">blobData to write to storage</param>
        /// <param name="questionNumber">Name of the folder in which the file has to be set</param>
        /// <returns> a string with message in it</returns>
        public string UploadBlob(String fileName, Stream inputStream, String questionNumber)
        {
            CloudBlobContainer container = GetCloudBlobContainer("questions");
            CloudBlockBlob blob = container.GetBlockBlobReference(questionNumber + "/" + fileName);
            string[] strings = fileName.Split('.');
            
            switch(strings[strings.Length - 1])
            {
                case "jpg":
                    blob.Properties.ContentType = "image/jpg";
                    break;

                case "png":
                    blob.Properties.ContentType = "image/png";
                    break;

                case "mp3":
                    blob.Properties.ContentType = "audio/mp3";
                    break;

                case "mp4":
                    blob.Properties.ContentType = "audio/mp4";
                    break;
            }

            blob.UploadFromStream(inputStream);

            return "success!";
        }
    }
}