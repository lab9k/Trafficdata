using Database_Access_Object;
using DocumentModels.BeMobile;
using DocumentModels.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace TraveltimesDocumentCreator.DocumentCreator.Transformation.Be_Mobile.LocalModels
{
    public class SegmentTaskModel
    {
        public QueryManager<GenericTraveltimeSegment> OutputQueryManager { get; set; }
        public TraveltimeSegment TraveltimeSegment { get; set; }
        public TraveltimeStatic TraveltimeStatic { get; set; }
    }
}
