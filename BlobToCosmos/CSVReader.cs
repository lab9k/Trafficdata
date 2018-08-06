using DocumentModels.BeMobile;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlobToCosmos
{
    public class CSVReader
    {
        public static List<TraveltimeSegment> GetSegmentsFromCSVString(string csv)
        {
            var lines = csv.Split(new[] {'\n','\r' },StringSplitOptions.RemoveEmptyEntries);
            List<TraveltimeSegment> res = new List<TraveltimeSegment>();

            for(int i = 1; i < lines.Length; i++)
            {
                string[] args = lines[i].Split(new char[] { ';' });
                if(args.Length == 3)
                {
                    //Console.WriteLine("Creating segments");
                    res.Add(new TraveltimeSegment
                    {
                        Id = args[0],
                        timestamp = args[1],
                        durationInMs = int.Parse(args[2])

                    });
                }
                
            }
            return res;
        }
        public static List<TraveltimeSegmentStatic> GetStaticsFromCSVString(string csv)
        {
            var lines = csv.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            List<TraveltimeSegmentStatic> res = new List<TraveltimeSegmentStatic>();

            for (int i = 1; i < lines.Length; i++)
            {
                string[] args = lines[i].Split(new char[] { ';' });
                if (args.Length == 8)
                {
                    //Console.WriteLine("Creating segments");
                    res.Add(new TraveltimeSegmentStatic
                    {
                        Id = args[0],
                        SegmentID = int.Parse(args[1]),
                        lengthmm = int.Parse(args[2]),
                        optimalspeedkph = int.Parse(args[3]),
                        beginnodelatitude = double.Parse(args[4]),
                        beginnodelongitude = double.Parse(args[5]),
                        endnodelatitude = double.Parse(args[6]),
                        endnodelongitude = double.Parse(args[7])
                    });
                }

            }
            return res;
        }
    }
}
