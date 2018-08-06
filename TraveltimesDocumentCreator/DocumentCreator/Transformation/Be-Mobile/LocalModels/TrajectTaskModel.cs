using Database_Access_Object;
using DocumentModels.BeMobile;
using DocumentModels.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace TraveltimesDocumentCreator.DocumentCreator.Transformation.Be_Mobile.LocalModels
{
    public class TrajectTaskModel
    {
        public List<TraveltimeStatic> StaticData { get; set; }
        public string Segment { get; set; }
        public QueryManager<TraveltimeSegment> InputQueryManager { get; set; }
        public QueryManager<GenericTraveltimeSegment> OutputQueryManager { get; set; }
    }
}
