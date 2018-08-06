using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace TraveltimesDocumentCreator.DocumentCreator.Transformation.Be_Mobile
{
    public interface IBlockingQueue<T>
    {
        BlockingCollection<T> Queue { get; set; }
        int Finished { get; set; }
        int Skipped { get; set; }
        int Failed { get; set; }
    }
}
