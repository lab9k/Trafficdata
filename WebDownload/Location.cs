using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebDownload
{
    public class Location
    {
        public string TrajectID { get; set; }
        public string LocationName { get; set; }
        public string FullName { get
            {
                return TrajectID + " - " + LocationName;
            } }
    }
}
