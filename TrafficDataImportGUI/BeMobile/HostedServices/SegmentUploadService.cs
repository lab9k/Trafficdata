using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TrafficDataImportGUI.BeMobile.Import;

namespace TrafficDataImportGUI.BeMobile.HostedServices
{
    public class SegmentUploadService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private bool isRunning = false;
        private IBlockingQueue<BeMobileTaskModel, BEMobileSegmentTaskModel> _queue;
        private readonly CancellationTokenSource _stoppingCts =
                                                   new CancellationTokenSource();
        private Task _currentTask;

        public SegmentUploadService(ILogger<SegmentUploadService> logger, IBlockingQueue<BeMobileTaskModel,BEMobileSegmentTaskModel> queue)
        {
            _logger = logger;
            _queue = queue;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Segment Background Service is starting.");
            isRunning = true;

            _currentTask = new Task(DoWork, cancellationToken);
            _currentTask.Start();

            return Task.CompletedTask;
        }

        private void DoWork()
        {
            _logger.LogInformation("Segment Background Service is working.");
            while (isRunning)
            {
                var timeout = TimeSpan.FromMilliseconds(100);
                List<Task> runningTasks = new List<Task>();
                BEMobileSegmentTaskModel t;
                if (runningTasks.Count == 0)
                {
                    _logger.LogInformation("Waiting for Segments in queue to process");
                    runningTasks.Add(_queue.DocumentQueue.Take().Execute(_queue));
                }
                _logger.LogInformation("Checking for additional segments");
                while (_queue.DocumentQueue.TryTake(out t, timeout)){
                    runningTasks.Add(t.Execute(_queue));
                }
                _logger.LogInformation("Waiting Segments to finish processing");
                Task.WaitAll(runningTasks.ToArray<Task>());
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Segment Background Service is stopping.");

            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }
    }
}
