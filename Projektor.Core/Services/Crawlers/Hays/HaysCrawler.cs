using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using Projektor.Core.Enums;
using Projektor.Core.Services.Crawlers.Base;
using Projektor.SeleniumParallelLibrary;

namespace Projektor.Core.Services.Crawlers.Hays
{
    public class HaysCrawler : CrawlerBase
    {
        public HaysCrawler(SeleniumTaskScheduler scheduler, ProjectService projectService, DataParserProvider factory, Database.Database database, ILogger logger) :
            base(new[] { new Uri("https://www.hays.de/jobsuche/stellenangebote-jobs/j/Contracting/3/c/Deutschland/D1641BCE-D56C-11D3-AFB2-00105AB00B48/p/1/?q=.Net") }, CrawlerName.Hays, scheduler, projectService, factory, database, logger)
        {
        }

        public override void Navigate(Uri searchResult, CancellationToken token)
        {
            var millisecondsUntilPageIsLoaded = 0;
            var nextButton = "search__results__pagination__next";            
            Scheduler.Schedule(new SeleniumTask(async driver =>
            {
                driver.Navigate().GoToUrl(searchResult);
                async Task ParseDocument()
                {
                    await Task.Delay(millisecondsUntilPageIsLoaded, token);
                    var html = driver.FindElement(By.TagName("html")).GetAttribute("outerHTML");
                    var doc = await HtmlParser.ParseDocumentAsync(html, token);
                    ProcessSearchResult(doc);
                }
                await ParseDocument();
                while (driver.FindElements(By.CssSelector(nextButton)).Count > 0)
                {
                    driver.FindElement(By.CssSelector(nextButton)).SendKeys(Keys.Enter);
                    await ParseDocument();
                }
            }));
        }
    }
}
