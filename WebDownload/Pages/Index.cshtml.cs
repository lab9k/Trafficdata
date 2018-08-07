using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebDownload.Pages
{
    public class IndexModel : PageModel
    {
        public void OnGet()
        {

        }

        public void OnPost()
        {
            string fromDate = Request.Form["from-date"];
            string toDate = Request.Form["to-date"];
            string segment = Request.Form["segmentName"];

            DateTime from = DateTime.Parse(fromDate,)

            Console.WriteLine($"{fromDate} -> {toDate} : {segment}");
        }
    }
}
