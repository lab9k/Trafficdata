using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using TraveltimesDocumentCreator.DocumentCreator.Transformation.Be_Mobile.LocalModels;

namespace TraveltimesDocumentCreator.DocumentCreator.Transformation.Be_Mobile
{
    public class TrajectQueue : IBlockingQueue<GenericQueueTask<TrajectTaskModel>>
    {
        public BlockingCollection<GenericQueueTask<TrajectTaskModel>> Queue { get; set; }
        public int Finished { get; set; }
        public int Skipped { get; set; }
        public int Failed { get; set; }

        public TrajectQueue()
        {
            Queue = new BlockingCollection<GenericQueueTask<TrajectTaskModel>>();
        }
    }
}
