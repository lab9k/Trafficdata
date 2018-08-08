using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DocumentModels.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace WebDownload.Pages
{
    public class IndexModel : PageModel
    {
        private static List<Location> _locations;
        public void OnGet()
        {

             Items = Locations.Select(c => new SelectListItem
            {
                Value = c.TrajectID,
                Text = c.FullName

            });
        }

        [BindProperty]
        public IEnumerable<SelectListItem> Items { get; set; } 

        public List<Location> Locations { get {
                if(_locations == null)
                {
                    var path = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "wwwroot", "segments.json");
                    string json = System.IO.File.ReadAllText(path);
                    _locations = JsonConvert.DeserializeObject<List<Location>>(json);
                }
                return _locations;
            }
            set { _locations = value; }
        }

        [BindProperty]
        public Location SelectedLocation { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            string fromDate = Request.Form["from-date"];
            string toDate = Request.Form["to-date"];
            string segment = Request.Form["SelectedLocation"];
            string filename = $"{segment}_{fromDate}-{toDate}";
            Task<List<ParserSegment>> task = new Task<List<ParserSegment>>(() => RecordFetcher.GetResults(fromDate, toDate, segment));
            task.Start();
            List<ParserSegment> results = await task;
            if (results.Count > 0)
                return IOManager.GetFileResult(results, filename, this);
            else
                return Content("No records found");
        }

        public IActionResult Get(MemoryStream memory,string filename)
        {
            var path = Path.Combine(
                          Directory.GetCurrentDirectory(),
                          "wwwroot", filename);
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(path));
        }

        public async Task<IActionResult> Get(string id)
        {
            string filename = id;
            if (filename == null)
                return Content("filename not present");

            var path = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "wwwroot", filename);
            Console.WriteLine(path);
            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(path));
        }
        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats.officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }
    }
}
