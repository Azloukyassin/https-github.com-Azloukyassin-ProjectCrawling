using AngleSharp.Dom;
using OpenQA.Selenium;
using Projektor.Core.Models;
using Projektor.Core.Services.Crawlers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Projektor.Core.Services.Crawlers.MichaelPage
{
    class MichaelPageDataParser : IDataParser
    {
        private string noInformation = "Kein Vorhanden ";
        private string baseUrlForCreation = "https://www.michaelpage.de";
        public IEnumerable<string> GetAdvantages(IDocument doc)
        {
            var resultatAdvantage = " ";
            var details = doc.GetElementsByClassName("job-desc-deal-wrapper");
            foreach (var list in details)
            {
                var items = list.GetElementsByTagName("ul")
                               .Select(x => x.TextContent);
                var advantItems = items.First();
                resultatAdvantage += advantItems.Trim();
            }
            return new[] { resultatAdvantage };
        }

        public Contact GetContact(IDocument doc)
        {
            var ansprechPartner = "";
            var telefon = "";
            var detailsAnsp = doc.GetElementsByClassName("job-consultant").First();
            var ansprechItems = detailsAnsp.GetElementsByClassName("field-item even")
                                      .Select(x => x.TextContent);
            ansprechPartner += ansprechItems.First();
            var detailsTel = doc.GetElementsByClassName("job-phone").First();
            var telItems = detailsTel.GetElementsByClassName("field-item even")
                                      .Select(x => x.TextContent);
            telefon += telItems.First();
            return new Contact(ansprechPartner, telefon, noInformation);
        }
        public string GetDuration(IDocument doc)
        {
            return noInformation;
        }
        public IEnumerable<string> GetEmployerInformation(IDocument doc)
        {
            var resultatAdvantage = "";
            var details = doc.GetElementsByClassName("job-desc-deal-wrapper");
            foreach (var list in details)
            {
                var items = list.GetElementsByTagName("ul")
                               .Select(x => x.TextContent);
                var advantItems = items.First();
                resultatAdvantage += advantItems.Trim();
            }
            return new[] { resultatAdvantage };
        }
        public IEnumerable<string> GetEmploymentInformation(IDocument doc)
        {
            var resultatAdvantage = " ";
            var details = doc.GetElementsByClassName("job-desc-deal-wrapper");
            foreach (var list in details)
            {
                var items = list.GetElementsByTagName("ul")
                                .Select(x => x.TextContent);
                var advantItems = items.First();
                resultatAdvantage += advantItems.Trim();
            }
            return new[] { resultatAdvantage };
        }
        public IEnumerable<string> GetIncomeInformation(IDocument doc)
        {
            var resultatAdvantage = " ";
            var details = doc.GetElementsByClassName("job-desc-deal-wrapper");
            foreach (var list in details)
            {
                var items = list.GetElementsByTagName("ul")
                                .Select(x => x.TextContent);
                var advantItems = items.First();
                resultatAdvantage += advantItems.Trim();
            }
            return new[] { resultatAdvantage };
        }
        public string GetLocation(IDocument doc)
        {
            var locResult = "";
            var details = doc.GetElementsByClassName("summary-detail-field");
            int i = 1;
            foreach (var list in details)
            {
                if (i == 4)
                {
                    var items = list.GetElementsByClassName("summary-detail-field-value")
                            .Select(x => x.TextContent);
                    var locItems = items.First();
                    locResult += locItems.Trim();
                }
                i++;
            }
            return locResult;
        }
        public string GetPosition(IDocument doc)
        {
            var posResult = "";
            var details = doc.GetElementsByClassName("summary-detail-field");
            int i = 1;
            foreach (var list in details)
            {
                if (i == 5)
                {
                    var items = list.GetElementsByClassName("summary-detail-field-value")
                            .Select(x => x.TextContent);
                    var posItems = items.First();
                    posResult += posItems.Trim();
                }
                i++;
            }
            return posResult;
        }
        public IEnumerable<string> GetProjectInformation(IDocument doc)
        {
            var resultatAdvantage = " ";
            var details = doc.GetElementsByClassName("job-desc-deal-wrapper");
            foreach (var list in details)
            {
                var items = list.GetElementsByTagName("ul")
                               .Select(x => x.TextContent);
                var advantItems = items.First();
                resultatAdvantage += advantItems.Trim();
            }
            return new[] { resultatAdvantage };
        }
        public IEnumerable<Uri> GetProjectUrls(IDocument doc)
        {
            List<Uri> uris = new List<Uri>();
            var projects = doc.GetElementsByClassName("job-title no-logo");
            foreach (var links in projects)
            {
                var str = links.GetElementsByTagName("a")
                                 .Where(element => element.HasAttribute("href") && element.Attributes["href"].Value
                                 .Contains("/job-detail/"))
                                 .Select(x => x.Attributes["href"].Value);
                foreach (var href in str)
                {
                    var baseUrl = baseUrlForCreation;
                    baseUrl += href;
                    uris.Add(new Uri(baseUrl));
                }
            }
            return uris;
        }
        public IEnumerable<string> GetQualifications(IDocument doc)
        {
            var resultatQualifikation = " ";
            var details = doc.GetElementsByClassName("job-desc-candidate-wrapper");
            foreach (var list in details)
            {
                var items = list.GetElementsByTagName("ul")
                               .Select(x => x.TextContent);
                var qualItems = items.First();
                resultatQualifikation += qualItems.Trim();
            }
            return new[] { resultatQualifikation };
        }
        public string GetReferenceNumber(IDocument doc)
        {
            var refNumResult = "";
            var details = doc.GetElementsByClassName("summary-detail-field");
            int i = 1;
            foreach (var list in details)
            {
                if (i == 8)
                {
                    var items = list.GetElementsByClassName("summary-detail-field-value")
                                    .Select(x => x.TextContent);
                    var refNumItems = items.First();
                    refNumResult += refNumItems.Trim();
                }
                i++;
            }
            return refNumResult;
        }
        public string GetStartingDate(IDocument doc)
        {
            return noInformation;
        }
        public IEnumerable<string> GetTasks(IDocument doc)
        {
            var resultatTask = " ";
            var details = doc.GetElementsByClassName("job-desc-role-wrapper");
            foreach (var list in details)
            {
                var items = list.GetElementsByTagName("ul")
                              .Select(x => x.TextContent);
                var taskItems = items.First();
                resultatTask += taskItems.Trim();
            }
            return new[] { resultatTask };
        }
        public string GetTitle(IDocument doc)
        {
            var details = doc.GetElementsByClassName("job-header");
            var TitleResultat = details.Select(x => x.TextContent).First();
            var title = TitleResultat.Trim();
            return title;
        }
    }
}
