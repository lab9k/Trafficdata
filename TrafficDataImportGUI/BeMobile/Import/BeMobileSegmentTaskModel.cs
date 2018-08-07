using Database_Access_Object;
using DocumentModels.BeMobile;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrafficDataImportGUI.BeMobile.Import
{
    public delegate Task HandleCreation(TraveltimeSegment input,int tryCount,int maxTries);

    public class BEMobileSegmentTaskModel
    {
        public HandleCreation CreationMethod { get; set; }
        public TraveltimeSegment SegmentToCreate { get; set; }

        public async Task Execute(IBlockingQueue<BeMobileTaskModel, BEMobileSegmentTaskModel> queue)
        {
            await CreationMethod(SegmentToCreate,0,10);
            queue.DocumentsFinished++;
        }
    }
}
