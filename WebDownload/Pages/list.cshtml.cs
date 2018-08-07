using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebDownload.Pages
{
    public class ListModel : PageModel
    {
        [BindProperty]
        public string[] Files { get; set; }

        public void OnGet()
        {
            Files = IOManager.ListCreatedFiles();
        }
    }
}