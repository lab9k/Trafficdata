using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlobToCosmos;
using Database_Access_Object;
using DocumentModels.BeMobile;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using TrafficDataImportGUI.BeMobile;
using TrafficDataImportGUI.BeMobile.Import;

namespace TrafficDataImportGUI.Pages.BE_Mobile
{
    public class BEM_importModel : PageModel
    {

        IBlockingQueue<BeMobileTaskModel, BEMobileSegmentTaskModel> _blockingQueue;
        private ILogger<BEM_importModel> _logger;

        public BEM_importModel(IBlockingQueue<BeMobileTaskModel, BEMobileSegmentTaskModel> queue, ILogger<BEM_importModel> logger)
        {
            _blockingQueue = queue;
            _logger = logger;
        }
        public void OnGet()
        {

        }
        
        public void OnPost()
        {
            string[] months = Request.Form["month"].ToString().Split(new char[] { ';' });
            string[] trajects = Request.Form["traject"].ToString().Split(new char[] { ';' });

            _logger.LogInformation($"Adding {months.Length * trajects.Length} tasks");
            foreach(string month in months)
            {
                foreach(string traject in trajects)
                {
                    BeMobileTaskModel task = new BeMobileTaskModel
                    {
                        BlobName = Request.Form["blobName"],
                        BlobKey = Request.Form["blobKey"],
                        BlobContainer = Request.Form["blobContainer"],
                        Month = month,
                        Traject = traject,
                        CosmosUrl = Request.Form["cosmosUrl"],
                        CosmosKey = Request.Form["cosmosKey"],
                        DatabaseName = Request.Form["OutputDB"],
                        CollectionName = Request.Form["OutputColl"],

                    };
                    _blockingQueue.JobQueue.Add(task);
                }
            }          

        }
        
    }
}