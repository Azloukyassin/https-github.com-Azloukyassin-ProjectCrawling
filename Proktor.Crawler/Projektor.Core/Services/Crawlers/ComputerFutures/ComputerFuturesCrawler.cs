using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using Projektor.Core.Enums;
using Projektor.Core.Services.Crawlers.Base;
using Projektor.SeleniumParallelLibrary;

namespace Projektor.Core.Services.Crawlers.ComputerFutures
{
    public class ComputerFuturesCrawler : CrawlerBase
    {
        public ComputerFuturesCrawler(SeleniumTaskScheduler scheduler, ProjectService projectService, DataParserProvider factory, Database.Database database, ILogger logger) : base(new [] {new Uri("https://www.computerfutures.com/de-de/job-search/") }, CrawlerName.ComputerFutures, scheduler, projectService, factory, database, logger)
        {
        }
        public override void Navigate(Uri searchResult, CancellationToken token)
        {
            Scheduler.Schedule(new SeleniumTask(async driver =>
            {
                var millisecondsUntilPageIsLoaded = 3000;
                var nav = driver.Navigate();
                nav.GoToUrl(searchResult);
                await Task.Delay(millisecondsUntilPageIsLoaded, token);
                var cookieNotice = driver.FindElement(By.ClassName("cookie-notice__close"));
                cookieNotice.Click();
                var employmentTypes = driver.FindElements(By.ClassName("job-search__checkbox"));
                var contracting = employmentTypes.First(element => element.Text.Contains("Freie Mitarbeit") || element.Text.Contains("Projektmitarbeit"));
                contracting.Click();
                do
                {
                    //ToDo Nav or for Solcom
                    await Task.Delay(millisecondsUntilPageIsLoaded, token);
                    var html = driver.FindElement(By.TagName("html")).GetAttribute("outerHTML");
                    var doc = await HtmlParser.ParseDocumentAsync(html, token);
                    ProcessSearchResult(doc);
                    driver.FindElement(By.CssSelector("a.page-link.next")).SendKeys(Keys.Enter);
                } while (driver.FindElements(By.CssSelector("a.page-link.next")).Count > 0);
            }));
        }
    }
}
