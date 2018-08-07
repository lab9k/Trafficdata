using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebDownload
{
    public class IOManager
    {
        public static string WriteSegmentsToFile(List<ParserSegment> segments, string fileName)
        {
            
            string path = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "wwwroot", (fileName + ".csv").Replace('-', '_').Replace(":", ""));
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            using (StreamWriter writer = System.IO.File.AppendText(path))
            {
                writer.WriteLine(segments[0].CSVHeader(";"));
                foreach (var segment in segments)
                {
                    Console.WriteLine("Writing segment to file");
                    writer.WriteLine(segment.ToCSVLine(";"));

                }
            }
            return path.Substring(Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "wwwroot").Length + 1);
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
