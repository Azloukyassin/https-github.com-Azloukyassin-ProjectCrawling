using Projektor.Core.Enums;

namespace Projektor.Core.Models
{
    /* ToDo State information about a crawler needed */
    public class CrawlerReport
    {
        public CrawlerReport(CrawlerName name)
        {
            Name = name;
        }
        public CrawlerName Name { get; }
    }
}
