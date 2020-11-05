using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using Projektor.Core.Enums;
using Projektor.Core.Services.Crawlers.Base;
using Projektor.SeleniumParallelLibrary;

namespace Projektor.Core.Services.Crawlers.Solcom
{
    public class SolcomCrawler : CrawlerBase
    {

        public SolcomCrawler(SeleniumTaskScheduler scheduler, ProjectService projectService, DataParserProvider factory, Database.Database database, ILogger logger)
            : base(new[] { new Uri("https://www.solcom.de/de/projektportal/projektangebote?--contenance_solcom_portal_project-index%5B%40package%5D=contenance.solcom&--contenance_solcom_portal_project-index%5B%40controller%5D=project&--contenance_solcom_portal_project-index%5B%40action%5D=index&--contenance_solcom_portal_project-index%5B%40format%5D=html") }, CrawlerName.Solcom, scheduler, projectService, factory, database, logger)
        {
        }

        public override void Navigate(Uri searchResult, CancellationToken token)
        {
            var millisecondsUntilPageIsLoaded = 0;
            var nextButton = "menu";
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
