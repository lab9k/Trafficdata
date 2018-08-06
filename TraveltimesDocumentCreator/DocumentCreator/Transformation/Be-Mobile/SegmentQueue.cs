using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using TraveltimesDocumentCreator.DocumentCreator.Transformation.Be_Mobile.LocalModels;

namespace TraveltimesDocumentCreator.DocumentCreator.Transformation.Be_Mobile
{
    public class SegmentQueue : IBlockingQueue<GenericQueueTask<SegmentTaskModel>>
    {
        public BlockingCollection<GenericQueueTask<SegmentTaskModel>> Queue { get; set; }
        public int Finished { get; set; }
        public int Skipped { get; set; }
        public int Failed { get; set; }

        public SegmentQueue()
        {
            Queue = new BlockingCollection<GenericQueueTask<SegmentTaskModel>>();
        }
    }
}
