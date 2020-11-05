using System;
using Microsoft.Extensions.DependencyInjection;

namespace Projektor.SeleniumParallelLibrary.Configuration
{
    public static class SeleniumParallelLibraryConfiguration
    {
        public static IServiceCollection AddSeleniumParallelLibrary(this IServiceCollection services, Func<SeleniumConfiguration> buildConfiguration)
        {
            return services.AddSingleton<SeleniumTaskScheduler>()
                           .AddSingleton(buildConfiguration.Invoke());
        }
    }
}
