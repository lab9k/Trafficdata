using System;
using System.Collections.Generic;
using System.Text;

namespace TraveltimesDocumentCreator
{
    public enum Verification
    {
        IgnoreAll, //Fetch all documents en process all results
        FromDate, //Fetch all documents from a specific date and process all results
        CheckExisting, //Fetch all documents and process results that do not exist already
        FromLastDate,  //Fetch biggest timestamp from documents en process all from that date
        ReplaceExisting
    }
}
