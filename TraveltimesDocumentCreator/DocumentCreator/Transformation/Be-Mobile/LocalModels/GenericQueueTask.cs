using System;
using System.Collections.Generic;
using System.Text;

namespace TraveltimesDocumentCreator.DocumentCreator.Transformation.Be_Mobile.LocalModels
{
    public delegate void TaskFunc<S>(S obj);
    public class GenericQueueTask<T>
    {
        public TaskFunc<T> Function { get; set; }
        public T Item { get; set; }

        public void ExecuteTask()
        {
            Function(Item);
        }
    }
}
