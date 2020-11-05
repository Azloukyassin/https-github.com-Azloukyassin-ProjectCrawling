using System;
using Microsoft.Extensions.DependencyInjection;
using Projektor.Core.Services.Crawlers.Base;
using Projektor.Core.Services.Crawlers.ComputerFutures;
using Projektor.Core.Services.Crawlers.Hays;
using Projektor.Core.Services.Crawlers.Solcom;
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
                    //.AddSingleton<IDataParser, SolcomDataParser>()
                    //.AddSingleton<CrawlerBase, SolcomCrawler>()
                    //.AddSingleton<CrawlerBase, FreelancerMapCrawler>()
                    //.AddSingleton<IDataParser, FreelancerMapDataProvider>()
                    //.AddSingleton<IDataParser, ComputerFuturesDataParser>()
                    //.AddSingleton<CrawlerBase, ComputerFuturesCrawler>()
                    .AddSingleton<ProjectService>();
            return services;
        }
    }
}
