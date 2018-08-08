using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TraveltimesDocumentCreator.DocumentCreator.Transformation.Be_Mobile;
using TraveltimesDocumentCreator.DocumentCreator.Transformation.Be_Mobile.LocalModels;

namespace TraveltimesDocumentCreator.HostedServices
{
    public class BeMobileSegmentService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private bool isRunning = false;
        private readonly CancellationTokenSource _stoppingCts =
                                                   new CancellationTokenSource();
        private Task _currentTask;
        private IBlockingQueue<GenericQueueTask<SegmentTaskModel>> _queue;
        private IThreadPool _pool;

        public BeMobileSegmentService(ILogger<BeMobileSegmentService> logger, IBlockingQueue<GenericQueueTask<SegmentTaskModel>> queue, IThreadPool pool)
        {
            _logger = logger;
            _queue = queue;
            _pool = pool;
            if (_pool == null) _logger.LogWarning("Threadpool service not defined, skipping restrictions");

        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Document Processing Service is starting.");
            isRunning = true;

            _currentTask = new Task(DoWork, cancellationToken);
            _currentTask.Start();

            return Task.CompletedTask;
        }

        private void DoWork()
        {
            _logger.LogInformation("Document Processing Service is working.");
            while (isRunning)
            {
                if(_pool == null || _pool.GetLock(5000) > 0)
                {
                    GenericQueueTask<SegmentTaskModel> task = _queue.Queue.Take();
                    task.ExecuteTask();
                    _pool.ReleaseLock();
                }
                
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Document Processing Service is stopping.");

            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }


    }
}
