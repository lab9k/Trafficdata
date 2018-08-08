using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebDownload
{
    public class IOManager
    {
        public static IActionResult GetFileResult(List<ParserSegment> segments, string fileName, PageModel model)
        {
            
            string path = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "wwwroot", (fileName + ".csv").Replace('-', '_').Replace(":", ""));
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            string s = segments[0].CSVHeader(";") + "\n";
            foreach (var segment in segments)
            {
                s += segment.ToCSVLine(";") + "\n";
            }

            byte[] byteArray = Encoding.ASCII.GetBytes(s);
            MemoryStream stream = new MemoryStream(byteArray);
            stream.Position = 0;
            return model.File(stream, GetContentType(path), Path.GetFileName(path));
            /*
            using (MemoryStream stream = new MemoryStream())
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.WriteLine(segments[0].CSVHeader(";"));
                foreach (var segment in segments)
                {
                    writer.WriteLine(segment.ToCSVLine(";"));
                    writer.Flush();
                }
                stream.Position = 0;
                return model.File(stream, GetContentType(path), Path.GetFileName(path));
            }*/


        }

        private static string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private static Dictionary<string, string> GetMimeTypes()
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

        public static string[] ListCreatedFiles()
        {
            string path = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "wwwroot");
            return Directory.GetFiles(path);
            
        }


    }
}
