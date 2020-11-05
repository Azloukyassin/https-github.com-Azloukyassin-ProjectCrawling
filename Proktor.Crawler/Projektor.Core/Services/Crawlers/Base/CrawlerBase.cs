using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using Projektor.Core.Enums;
using Projektor.SeleniumParallelLibrary;

namespace Projektor.Core.Services.Crawlers.Base
{
    public abstract class CrawlerBase
    {
        private readonly ProjectService _projectService;
        private readonly Database.Database _database;
        private readonly ILogger _logger;
        private readonly IDataParser _dataProvider;        
        private readonly BlockingCollection<Uri> _foundProjects;
        private readonly BlockingCollection<Uri> _parsedProjects;
        protected CrawlerBase(IEnumerable<Uri> searchResults, CrawlerName name, SeleniumTaskScheduler scheduler, ProjectService projectService, DataParserProvider dataProviderFactory, Database.Database database, ILogger logger)
        {
            _projectService = projectService;
            _database = database;
            _logger = logger;
            _dataProvider = dataProviderFactory.GetProvider(name);
            _foundProjects = new BlockingCollection<Uri>();
            _parsedProjects = new BlockingCollection<Uri>();
            HtmlParser = new HtmlParser();
            TokenSource = new CancellationTokenSource();
            SearchResults = searchResults;
            Scheduler = scheduler;
            Name = name;
        }
        
        public CrawlerName Name { get; }
        public HtmlParser HtmlParser { get; }        
        public IEnumerable<Uri> SearchResults { get; }
        public SeleniumTaskScheduler Scheduler { get; }
        public CancellationTokenSource TokenSource { get; }
        public bool Finished => _parsedProjects.Count == _foundProjects.Count;
        public abstract void Navigate(Uri searchResult, CancellationToken token);
        public void Start()
        {
            Parallel.ForEach(SearchResults, searchResult =>
            {
                Navigate(searchResult, TokenSource.Token);
            });
        }

        public void Stop()
        {
            TokenSource.Cancel();
        }
        public void ProcessSearchResult(IDocument searchResult)
        {
            var projectUrls = _dataProvider.GetProjectUrls(searchResult);
            foreach (var url in projectUrls)
            {
                _foundProjects.Add(url, TokenSource.Token);
            }
            Parallel.ForEach(projectUrls, projectUrl => ScheduleAddProject(projectUrl, TokenSource.Token));
        }
        public void ScheduleAddProject(Uri projectUrl, CancellationToken token)
        {
            Scheduler.Schedule(new SeleniumTask(async driver =>
            {
                driver.Navigate().GoToUrl(projectUrl);
                var html = driver.FindElement(By.TagName("html")).GetAttribute("outerHTML");
                var doc = await HtmlParser.ParseDocumentAsync(html, token);
                var result = _projectService.TryBuildProject(doc, projectUrl, _dataProvider);                
                if (result.Success)
                {
                    var project = result.Project;
                    _projectService.AddProject(project);
                    await _database.StoreProjects(Name, project);
                }
                else
                {
                    var ex = result.Exception;
                    var message = $"Encountered an error while trying to parse project: {projectUrl}. View logs for detailed information.";
                    _logger.LogError($"{message}{Environment.NewLine}{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                }
                _parsedProjects.Add(projectUrl);                
            }));
        }
    }
}
