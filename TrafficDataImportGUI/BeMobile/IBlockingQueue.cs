using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrafficDataImportGUI.BeMobile
{
    public interface IBlockingQueue<T,S>
    {
        BlockingCollection<T> JobQueue { get; set; }
        BlockingCollection<S> DocumentQueue { get; set; }
         int JobsFinished { get; set; }
         int JobsSkipped { get; set; }
         int JobsFailed { get; set; }
         int DocumentsFinished { get; set; }
         int DocumentsSkipped { get; set; }
         int DocumentsFailed { get; set; }

    }
}
