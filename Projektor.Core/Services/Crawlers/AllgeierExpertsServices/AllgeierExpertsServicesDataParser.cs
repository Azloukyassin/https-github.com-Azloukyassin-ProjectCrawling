using AngleSharp.Dom;
using OpenQA.Selenium;
using Projektor.Core.Models;
using Projektor.Core.Services.Crawlers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Projektor.Core.Services.Crawlers.AllgeierExpertsServices
{
    class AllgeierExpertsServicesDataParser : IDataParser
    {
        private String baseUrlForCreation = "https://bewerbung.tecops.de";
        private String noInformation = "Kein Vorhanden";
        public IEnumerable<string> GetAdvantages(IDocument doc)
        {
            return new[] { noInformation };
        }

        public Contact GetContact(IDocument doc)
        {
            return new Contact(noInformation, noInformation, "Karriere-pro@allgeier-experts.com");
        }

        public string GetDuration(IDocument doc)
        {
            return noInformation;
        }

        public IEnumerable<string> GetEmployerInformation(IDocument doc)
        {
            return new[] { noInformation };
        }
        public IEnumerable<string> GetEmploymentInformation(IDocument doc)
        {
            return new[] { noInformation };
        }
        public IEnumerable<string> GetIncomeInformation(IDocument doc)
        {
            return new[] { noInformation };
        }
        public string GetLocation(IDocument doc)
        {
            return noInformation;
        }
        public string GetPosition(IDocument doc)
        {
            return noInformation;
        }
        public IEnumerable<string> GetProjectInformation(IDocument doc)
        {
            return new[] { noInformation };
        }
        public IEnumerable<Uri> GetProjectUrls(IDocument doc)
        {
            List<Uri> uris = new List<Uri>();
            var projects = doc.GetElementsByClassName("jobPanelContent");
            foreach (var links in projects)
            {
                var str = links.GetElementsByTagName("a")
                                 .Where(element => element.HasAttribute("href") && element.Attributes["href"].Value
                                 .Contains("generator.php?id="))
                                 .Select(x => x.Attributes["href"].Value);
                foreach (var href in str)
                {
                    var baseUrl = " ";
                    baseUrl += href;
                    uris.Add(new Uri(baseUrl));
                }
            }
            return uris;
        }
        public IEnumerable<string> GetQualifications(IDocument doc)
        {
            var resultatQualifikation = "";
            var details = doc.GetElementsByClassName("col-md-12");
            int i = 1;
            foreach (var list in details)
            {
                var taskItems = list.GetElementsByTagName("ul")
                                        .Select(x => x.TextContent);
                if (i == 2)
                {
                    var items = taskItems.First();
                    resultatQualifikation += items;
                }
                i++;
            }
            return new[] { resultatQualifikation };
        }
        public string GetReferenceNumber(IDocument doc)
        {
            var referenceNumm = doc.GetElementsByClassName("refNr")
                                   .Select(x => x.TextContent);
            var refItem = referenceNumm.First();
            var refResulat = refItem.Substring(refItem.IndexOf(':') + 1);
            var resulat = refResulat.Trim();
            return resulat;
        }
        public string GetStartingDate(IDocument doc)
        {
            return noInformation;
        }
        public IEnumerable<string> GetTasks(IDocument doc)
        {
            var resultatTask = "";
            var details = doc.GetElementsByClassName("row").First();
            var taskItems = details.GetElementsByClassName("col-md-12")
                                    .Select(x => x.TextContent);
            var items = taskItems.First();
            resultatTask += items;
            return new[] { resultatTask };
        }
        public string GetTitle(IDocument doc)
        {
            var title = doc.GetElementsByTagName("h1")
                        .Select(x => x.TextContent);
            var titleItems = title.First();
            var resultatTitle = titleItems.Substring(1, titleItems.IndexOf(')'));
            var resultat = resultatTitle.Trim();
            return resultat;
        }
    }
}
