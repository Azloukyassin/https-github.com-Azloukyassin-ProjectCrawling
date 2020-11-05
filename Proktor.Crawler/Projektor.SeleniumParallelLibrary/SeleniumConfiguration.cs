namespace Projektor.SeleniumParallelLibrary
{
    public class SeleniumConfiguration
    {
        public int WorkerCount { get; }
        public bool Headless { get; }

        public SeleniumConfiguration(int workerCount, bool headless)
        {
            WorkerCount = workerCount;
            Headless = headless;
        }
    }
}
