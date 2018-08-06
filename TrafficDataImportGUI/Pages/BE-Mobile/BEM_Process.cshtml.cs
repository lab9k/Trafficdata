using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TrafficDataImportGUI.BeMobile;
using TraveltimesDocumentCreator.DocumentCreator.Transformation.Be_Mobile;

namespace TrafficDataImportGUI.Pages
{
    public class BeMobileProcessModel : PageModel
    {
        private IDataNormalizer _dataNormalizer;

        public BeMobileProcessModel(IDataNormalizer dataNormalizer)
        {
            _dataNormalizer = dataNormalizer;
        }

        public void OnGet()
        {
        }

        public void OnPost()
        {
            string[] trajects = Request.Form["trajects"].ToString().Split(new char[] { ';' });

            var InUrl = Request.Form["InUrl"];
            var InKey = Request.Form["InKey"];
            var OutUrl = Request.Form["OutUrl"];
            var OutKey = Request.Form["OutKey"];
            var InDb = Request.Form["InDb"];
            var InColl = Request.Form["InColl"];
            var InStat = Request.Form["InStat"];
            var OutDb = Request.Form["OutDb"];
            var OutColl = Request.Form["OutColl"];

            _dataNormalizer.StartNormalization(InUrl, InKey, InDb, InStat, InColl,trajects, OutColl, OutDb, OutUrl, OutKey);

        }
    }
}