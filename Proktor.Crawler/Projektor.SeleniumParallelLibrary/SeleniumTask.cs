using System;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace Projektor.SeleniumParallelLibrary
{
    public class SeleniumTask
    {
        private readonly Func<IWebDriver, Task> _func;
        public SeleniumTask(Func<IWebDriver, Task> func)
        {
            _func = func;
        }

        public Task ExecuteAsync(IWebDriver driver)
        {
            return _func.Invoke(driver);
        }
    }
}
