using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Dom;
using Projektor.Core.Models;
using Projektor.Core.Services.Crawlers.Base;

namespace Projektor.Core.Services.Crawlers.FreelancerMap
{
    public class FreelancerMapDataProvider : IDataParser
    {
        public string GetTitle(IDocument doc)
        {
            var titleElements = doc.GetElementsByName("title")
                .Select(x => x.Text());
            var title = titleElements.First()
                .Trim();
            return title;
        }

        public string GetLocation(IDocument doc)
        {
            throw new NotImplementedException();
        }

        public string GetPosition(IDocument doc)
        {
            throw new NotImplementedException();
        }

        public string GetReferenceNumber(IDocument doc)
        {
            throw new NotImplementedException();
        }

        public string GetStartingDate(IDocument doc)
        {
            throw new NotImplementedException();
        }

        public string GetDuration(IDocument doc)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetTasks(IDocument doc)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetQualifications(IDocument doc)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetAdvantages(IDocument doc)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetProjectInformation(IDocument doc)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetIncomeInformation(IDocument doc)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetEmployerInformation(IDocument doc)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetEmploymentInformation(IDocument doc)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Uri> GetProjectUrls(IDocument doc)
        {
            var urls = doc.GetElementsByTagName("A")
                .Where(element => element.HasAttribute("href") && element.Attributes["href"].Value
                                      .Contains("/projektboerse/projekte"))
                .Select(element => element.Attributes["href"].Value)
                .Distinct()
                .Select(element => string.Concat("https://www.freelancermap.de", element))
                .Select(url => new Uri(url));
            return urls;
        }

        public Contact GetContact(IDocument doc)
        {
            throw new NotImplementedException();
        }
    }
}
