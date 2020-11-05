using System;
using Microsoft.Extensions.DependencyInjection;
using Projektor.Core.Services.Crawlers.AllgeierExpertsGo;
using Projektor.Core.Services.Crawlers.AllgeierExpertsServices;
using Projektor.Core.Services.Crawlers.Base;
using Projektor.Core.Services.Crawlers.ComputerFutures;
using Projektor.Core.Services.Crawlers.FreelancerMap;
using Projektor.Core.Services.Crawlers.Hays;
using Projektor.Core.Services.Crawlers.MichaelPage;
using Projektor.Core.Services.Crawlers.Solcom;
using Projektor.Core.Services.Progressive;
using Projektor.SeleniumParallelLibrary;
using Projektor.SeleniumParallelLibrary.Configuration;

namespace Projektor.Core.Configuration
{
    public static class CoreConfiguration
    {
        public static IServiceCollection AddEngine(this IServiceCollection services, Func<SeleniumConfiguration> buildConfiguration)
        {
            services.AddSeleniumParallelLibrary(buildConfiguration)
                    .AddSingleton<Engine>()
                    .AddSingleton<DataParserProvider>()
                    .AddSingleton<Database.Database>()
                    .AddSingleton<IDataParser, HaysDataParser>()
                    .AddSingleton<CrawlerBase, HaysCrawler>()
                    .AddSingleton<IDataParser, SolcomDataParser>()
                    .AddSingleton<CrawlerBase, SolcomCrawler>()
                    .AddSingleton<CrawlerBase, FreelancerMapCrawler>()
                    .AddSingleton<IDataParser, FreelancerMapDataParser>()
                    .AddSingleton<IDataParser, ComputerFuturesDataParser>()
                    .AddSingleton<CrawlerBase, ComputerFuturesCrawler>()
                    //.AddSingleton<IDataParser, AllgeierExpertsGoDataParser>()
                    //.AddSingleton<CrawlerBase, AllgeierExpertsGoCrawler>()
                    //.AddSingleton<IDataParser, AllgeierExpertsServicesDataParser>() 
                    //.AddSingleton<CrawlerBase, AllgeierExpertsServicesCrawler>()
                    //.AddSingleton<IDataParser, MichaelPageDataParser>()
                    //.AddSingleton<CrawlerBase, MichaelPageCrawler>() 
                    //.AddSingleton<IDataParser, ProgressiveDataParser>()
                    //.AddSingleton<CrawlerBase, ProgressiveCrawler>()
                    .AddSingleton<ProjectService>();
            return services;
        }
    }
}
