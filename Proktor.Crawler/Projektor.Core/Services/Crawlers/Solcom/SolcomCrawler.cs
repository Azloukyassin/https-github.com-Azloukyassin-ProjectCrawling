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
            : base(new[] { new Uri("https://www.solcom.de/de/projektportal.aspx") }, CrawlerName.Solcom, scheduler, projectService, factory, database, logger)
        {
        }

        public override void Navigate(Uri searchResult, CancellationToken token)
        {
            var millisecondsUntilPageIsLoaded = 2000;
            Scheduler.Schedule(new SeleniumTask(async driver =>
            {                
                driver.Navigate().GoToUrl(searchResult);
                var inputSearchTerm = driver.FindElement(By.Id("SearchTerm"));
                var showResult = driver.FindElement(By.Id("startSearch"));
                var nextButton = "a.next";
                inputSearchTerm.SendKeys(".Net");
                await Task.Delay(millisecondsUntilPageIsLoaded, token);
                showResult.Click();
                do
                {
                    await Task.Delay(millisecondsUntilPageIsLoaded, token);
                    var html = driver.FindElement(By.TagName("html")).GetAttribute("outerHTML");
                    var doc = await HtmlParser.ParseDocumentAsync(html, token);
                    ProcessSearchResult(doc);
                    var currentPage = int.Parse(driver.FindElement(By.CssSelector("div.count span.current")).Text);
                    var nextPageUrl = driver.FindElement(By.CssSelector(nextButton)).GetAttribute("href");
                    var pageNumber = int.Parse(nextPageUrl.Substring(nextPageUrl.IndexOf("page=") + 5));
                    if(pageNumber == currentPage)
                    {
                        break;
                    }
                    driver.FindElement(By.CssSelector(nextButton)).SendKeys(Keys.Enter);
                } while (true);
            }));
        }
    } 
}
