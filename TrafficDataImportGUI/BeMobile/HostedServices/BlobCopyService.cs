using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TrafficDataImportGUI.BeMobile.Import;
using TraveltimesDocumentCreator;

namespace TrafficDataImportGUI.BeMobile.HostedServices
{
    public class BlobCopyService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private bool isRunning = false;
        private IBlockingQueue<BeMobileTaskModel, BEMobileSegmentTaskModel> _queue;
        private IThreadPool _threadPool;
        private readonly CancellationTokenSource _stoppingCts =
                                                   new CancellationTokenSource();
        private Task _currentTask;

        public BlobCopyService(IThreadPool threadPool,ILogger<BlobCopyService> logger, IBlockingQueue<BeMobileTaskModel, BEMobileSegmentTaskModel> queue)
        {
            _logger = logger;
            _queue = queue;
            _threadPool = threadPool;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Blob Background Service is starting.");
            isRunning = true;
                     
            _currentTask = new Task(DoWork, cancellationToken);
            _currentTask.Start();

            return Task.CompletedTask;
        }

        private void DoWork()
        {
            _logger.LogInformation("Blob Background Service is working.");
            while (isRunning)
            {
                if(_threadPool.GetLock(10000) > 0)
                {
                    BeMobileTaskModel task = _queue.JobQueue.Take();
                    _logger.LogInformation("Processing new Blob task from queue");
                    task.Execute(_logger, _queue);
                    _threadPool.ReleaseLock();
                    _logger.LogInformation("Finished processing Blob task from queue");
                    _queue.JobsFinished += 1;
                }

                
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Blob Background Service is stopping.");

            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }
    }
}
