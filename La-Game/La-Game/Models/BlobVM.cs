using La_Game.Models;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace La_Game.ViewModels
{
    public class BlobVM
    {
        public Question question { get; set; }
        public CloudBlockBlob blob { get; set;}
    }
}