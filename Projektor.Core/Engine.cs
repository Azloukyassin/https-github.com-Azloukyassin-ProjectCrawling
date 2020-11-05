using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Projektor.Core.Models;
using Projektor.Core.Services.Crawlers.Base;
using Projektor.SeleniumParallelLibrary;

namespace Projektor.Core
{
    public class Engine
    {
        private readonly ILogger _logger;
        private readonly IEnumerable<CrawlerBase> _crawlers;
        private readonly SeleniumTaskScheduler _scheduler;
        private readonly ProjectService _projectService;
        private readonly TimeSpan _frequency;
        private CancellationTokenSource _cancellationTokenSource;
        private Timer _recurringCrawl;
        private TimeSpan _lastCrawlTime;
        public Engine(ILogger logger, IEnumerable<CrawlerBase> crawlers, SeleniumTaskScheduler scheduler, ProjectService projectService)
        {
            _logger = logger;
            _crawlers = crawlers;
            _scheduler = scheduler;
            _projectService = projectService;
            _frequency = TimeSpan.FromMinutes(30);
            IsStarted = false;
        }

        private CancellationToken Token => _cancellationTokenSource.Token;
        public IEnumerable<Project> Projects => _projectService.Projects;
        public bool IsSearching => _crawlers.Any(crawler => !crawler.Finished) || _scheduler.IsWorking;
        public bool IsStarted { get; private set; }
        //ToDo Check StatusManager, Enum State existing
        public IEnumerable<CrawlerReport> CrawlerReports => _crawlers.Select(crawler => new CrawlerReport(crawler.Name));
        public TimeSpan RemainingTimeUntilCrawlingIsStartedAgain
        {
            get
            {
                if (!IsStarted) return TimeSpan.MaxValue;
                return _frequency - (DateTime.Now.TimeOfDay - _lastCrawlTime);
            }
        }

        public void Start()
        {
            if (IsStarted)
            {
                _logger.LogInformation("Engine already started");
                return;
            }
            IsStarted = true;
            _recurringCrawl = new Timer(o =>
            {
                _lastCrawlTime = DateTime.Now.TimeOfDay;
                _logger.LogInformation("Crawling started.");
                _cancellationTokenSource = new CancellationTokenSource();
                _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                _scheduler.Start(Token);
                RunCrawlers();
            }, null, 0, (int)_frequency.TotalMilliseconds);
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            _recurringCrawl.Dispose();
            _scheduler.Stop();
            IsStarted = false;
            _logger.LogInformation("Engine and its components have been stopped.");
        }

        private void RunCrawlers()
        {
            Parallel.ForEach(_crawlers, crawler =>
            {
                crawler.Start();                
            });
        }
    }
}
