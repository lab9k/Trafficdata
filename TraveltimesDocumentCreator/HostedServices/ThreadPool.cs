using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TraveltimesDocumentCreator
{
    public interface IThreadPool
    {
        int GetLock(int timeout);
        void ReleaseLock();
    }
    public class ThreadPool : IThreadPool
    {
        private int Threadpool = 50;

        public int GetLock(int timeout)
        {

            while(Threadpool <= 0 && timeout > 0)
            {
               // Console.WriteLine("No locks left, waiting..");
                timeout -= 500;
                Thread.Sleep(500);
            }

            return timeout > 0 ? Threadpool-- : -1;
        }

        public void ReleaseLock()
        {
            Threadpool++;
            //Console.WriteLine($"Lock released, {Threadpool} locks left");
        }
    }
}
