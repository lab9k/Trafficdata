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

namespace TrafficDataImportGUI.Pages.BE_Mobile
{
    public class BEM_import_staticModel : PageModel
    {
        private ILogger _logger;

        public BEM_import_staticModel(ILogger<BEM_import_staticModel> logger)
        {
            _logger = logger;
        }
        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var BlobName = Request.Form["blobName"];
            var BlobKey = Request.Form["blobKey"];
            var BlobContainer = Request.Form["blobContainer"];
            var CosmosUrl = Request.Form["cosmosUrl"];
            var CosmosKey = Request.Form["cosmosKey"];
            var DatabaseName = Request.Form["OutputDB"];
            var CollectionName = Request.Form["OutputColl"];
            var Path = Request.Form["folder"];

            BlobManager blobManager = new BlobManager(BlobName, BlobKey);
            List<IListBlobItem> items = blobManager.ListAllBlobsAsync(BlobContainer, Path).Result;
            _logger.LogInformation($"{items.Count} items found to import");
            List<TraveltimeSegmentStatic> segments = new List<TraveltimeSegmentStatic>();
            foreach (IListBlobItem item in items)
            {
                segments.AddRange(CSVReader.GetStaticsFromCSVString(blobManager.DownloadBlob(item.Container.Name, BlobManager.getBlobIdFromURI(item.Uri)).Result));

            }
            _logger.LogInformation($"{segments.Count} documents parsed");

            CrudManager<TraveltimeSegmentStatic> crudManager = new CrudManager<TraveltimeSegmentStatic>
            {
                DatabaseUri = CosmosUrl,
                DatabaseKey = CosmosKey,
                DatabaseName = DatabaseName,
                CollectionName = CollectionName
            };
            crudManager.Init();

            foreach (TraveltimeSegmentStatic segment in segments)
            {
                await crudManager.Create(segment);
            }

            return RedirectToPage("/BE-Mobile/BEM_import_static");

        }

    }
}