using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrafficDataImportGUI.BeMobile.Import
{
    public class BlobCopyQueue: IBlockingQueue<BeMobileTaskModel,BEMobileSegmentTaskModel>
    {        
        public BlockingCollection<BeMobileTaskModel> JobQueue { get; set; }
        public BlockingCollection<BEMobileSegmentTaskModel> DocumentQueue { get; set; }
        public int JobsFinished { get; set; }
        public int JobsSkipped { get; set; }
        public int JobsFailed { get; set; }
        public int DocumentsFinished { get; set; }
        public int DocumentsSkipped { get; set; }
        public int DocumentsFailed { get; set; }
        public BlobCopyQueue()
        {
            JobQueue = new BlockingCollection<BeMobileTaskModel>();
            DocumentQueue = new BlockingCollection<BEMobileSegmentTaskModel>();
        }

    }
}
