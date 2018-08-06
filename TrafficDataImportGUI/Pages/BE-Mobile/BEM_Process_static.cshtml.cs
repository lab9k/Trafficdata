using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TraveltimesDocumentCreator;

namespace TrafficDataImportGUI.Pages.BE_Mobile
{
    public class BEM_Process_staticModel : PageModel
    {
        public void OnGet()
        {

        }

        public void OnPost()
        {
            var InURL = Request.Form["InputURL"];
            var InKey = Request.Form["InputKey"];
            var InDB = Request.Form["InputDB"];
            var InColl = Request.Form["InputColl"];
            var OutUrl = Request.Form["OutputURL"];
            var OutKey = Request.Form["OutputKey"];
            var OutDb = Request.Form["OutputDb"];
            var OutColl = Request.Form["OutputColl"];
            var gApiKey = Request.Form["apiKey"];

            BemStaticAggregator.AggregateStaticData(gApiKey, InURL, InKey, InDB, InColl, OutColl, OutDb, OutUrl, OutKey);

        }
    }
}