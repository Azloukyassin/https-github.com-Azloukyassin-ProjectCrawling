using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using Projektor.Core.Enums;
using Projektor.Core.Services.Crawlers.Base;
using Projektor.SeleniumParallelLibrary;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Projektor.Core.Services.Crawlers.AllgeierExpertsGo
{
    public class AllgeierExpertsGoCrawler : CrawlerBase
    {

        public AllgeierExpertsGoCrawler(SeleniumTaskScheduler scheduler, ProjectService projectService, DataParserProvider factory, Database.Database database, ILogger logger)
            : base(new[] { new Uri("https://www.allgeier-experts-go.com/expert/freelancer") }, CrawlerName.AllgeierExpertsGo, scheduler, projectService, factory, database, logger)
        {
        }




        public override void Navigate(Uri searchResult, CancellationToken token)
        {
            var millisecondsUntilPageIsLoaded = 0;
            var nextButton = "btn";
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
