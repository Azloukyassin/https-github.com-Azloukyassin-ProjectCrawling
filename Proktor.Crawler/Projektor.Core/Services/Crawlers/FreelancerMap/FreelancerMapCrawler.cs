using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using Projektor.Core.Enums;
using Projektor.Core.Services.Crawlers.Base;
using Projektor.SeleniumParallelLibrary;

namespace Projektor.Core.Services.Crawlers.FreelancerMap
{
    public class FreelancerMapCrawler : CrawlerBase
    {
        public FreelancerMapCrawler(SeleniumTaskScheduler scheduler, ProjectService projectService, DataParserProvider factory, Database.Database database, ILogger logger) 
            : base(new[] { new Uri("https://www.freelancermap.de/?module=projekt&func=suchergebnisse&pq=.net&profisuche=0&pq_sorttype=1&area=newpb&redirect=1") }, CrawlerName.FreelancerMap, scheduler, projectService, factory, database, logger)
        {
        }

        public override void Navigate(Uri searchResult, CancellationToken token)
        {
            var millisecondsUntilPageIsLoaded = 0;
            var nextButton = "next";

            Scheduler.Schedule(new SeleniumTask(async driver =>
            {
                driver.Navigate().GoToUrl(searchResult);
                do
                {
                    await Task.Delay(millisecondsUntilPageIsLoaded, token);
                    var html = driver.FindElement(By.TagName("html")).GetAttribute("outerHTML");
                    var doc = await HtmlParser.ParseDocumentAsync(html, token);
                    ProcessSearchResult(doc);
                    driver.FindElement(By.CssSelector(nextButton)).SendKeys(Keys.Enter);
                } while (driver.FindElements(By.CssSelector(nextButton)).Count > 0);
            }));
        }
    }
}
