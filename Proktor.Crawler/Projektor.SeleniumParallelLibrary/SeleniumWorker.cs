using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Projektor.SeleniumParallelLibrary
{
    public class SeleniumWorker : IDisposable
    {
        private readonly ILogger _logger;
        private readonly CancellationToken _token;
        private readonly IWebDriver _driver;
        public SeleniumWorker(SeleniumConfiguration configuration, ILogger logger, CancellationToken token)
        {
            var isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            var options = new ChromeOptions();
            if (isLinux)
            {
                options.BinaryLocation = "/usr/bin/google-chrome-stable";
                options.AddArguments("--no-sandbox", "--disable-gpu", "--disable-dev-shm-usage");
            }
            if (configuration.Headless)
            {
                options.AddArgument("headless");
            }
            _driver = new ChromeDriver(AppDomain.CurrentDomain.BaseDirectory, options);
            _logger = logger;
            _token = token;
        }

        public async Task ExecuteAsync(SeleniumTask task)
        {
            try
            {
                await task.ExecuteAsync(_driver);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Encountered an error while trying to execute a task.{Environment.NewLine}Error message: {ex.Message}{Environment.NewLine}Stack trace: {ex.StackTrace}");
            }
        }

        public void Dispose()
        {
            _driver.Quit();
            _driver.Dispose();
        }
    }
}
