using System;
using System.Collections.Generic;
using System.Linq;
using Projektor.Core.Enums;

namespace Projektor.Core.Services.Crawlers.Base
{
    public class DataParserProvider
    {
        private IEnumerable<IDataParser> Providers;
        public DataParserProvider(IEnumerable<IDataParser> providers)
        {
            Providers = providers;
        }

        public IDataParser GetProvider(CrawlerName name)
        {
            //ToDo check general naming conventions in complete solution
            var adapter = Providers.FirstOrDefault(a => a.GetType().Name.Replace("DataParser", "") == name.ToString());
            var existsAdapter = adapter != null;
            if (!existsAdapter) throw new ArgumentException($"DataProvider {name} was requested, but not registered. Please register the DataProvider.");
            return adapter;
        }
    }
}
