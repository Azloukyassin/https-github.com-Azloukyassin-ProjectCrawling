using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Projektor.SeleniumParallelLibrary
{
    public class SeleniumTaskScheduler
    {
        private readonly ILogger _logger;
        private readonly SeleniumConfiguration _configuration;
        private BlockingCollection<SeleniumTask> _tasks;
        private CancellationToken _token;
        private bool _isRunning;

        public SeleniumTaskScheduler(SeleniumConfiguration configuration, ILogger logger)
        {
            _logger = logger;
            _tasks = new BlockingCollection<SeleniumTask>();
            _configuration = configuration;
            _isRunning = false;
        }

        public bool IsWorking => _tasks.Count > 0;

        public void Start(CancellationToken token)
        {
            if (_isRunning)
            {
                _logger.LogWarning("SeleniumTaskScheduler is already running.");
                return;
            }
            _tasks = new BlockingCollection<SeleniumTask>();
            _token = token;
            InitializeWorkers();
            _isRunning = true;
        }
        public void Stop()
        {
            if (!_isRunning)
            {
                _logger.LogWarning("Cannot stop a non running SeleniumTaskScheduler.");
                return;
            }
            _isRunning = false;
            _tasks.CompleteAdding();
        }

        public void InitializeWorkers()
        {
            var workerCount = _configuration.WorkerCount;
            for (var i = 0; i < workerCount; i++)
            {
                BuildWorker();
            }
        }
        public void Schedule(SeleniumTask task)
        {
            if (!_isRunning)
            {
                _logger.LogError("Scheduler not started");
                return;
            }
            _tasks.Add(task, _token);
        }
        private void BuildWorker()
        {
            Task.Run(async () =>
            {
                var worker = new SeleniumWorker(_configuration, _logger, _token);
                try
                {
                    foreach (var task in _tasks.GetConsumingEnumerable(_token))
                    {
                        await worker.ExecuteAsync(task);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
                finally
                {
                    worker.Dispose();
                }
            }, _token);
        }
    }
}
