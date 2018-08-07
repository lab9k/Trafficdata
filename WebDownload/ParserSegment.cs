using DocumentModels.Generic;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace WebDownload
{
    public class ParserSegment : GenericTraveltimeSegment
    {
        public string CSVHeader(string separator)
        {
            return "Source" + separator
                + "Date" + separator
                + "SegmentName" + separator
                + "FromPoint" + separator
                + "Length(m)"+ separator
                + "Duration(s)" + separator
                + "Speed(kph)";
        }
        public string ToCSVLine(string separator)
        {
            return Source + separator
                + Timestamp.FullDate + separator
                + SegmentName + separator
                + FromPoint + separator
                + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", Length) + separator
                + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", Duration) + separator
                + String.Format(CultureInfo.InvariantCulture,"{0:0.00}", Speed); 
        }
    }
}
