using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Dom;
using Projektor.Core.Models;
using Projektor.Core.Services.Crawlers.Base;
using Projektor.Core.Services.Crawlers.Hays;

namespace Projektor.Core.Services.Crawlers.ComputerFutures
{
    public class ComputerFuturesDataParser : IDataParser
    {
        public string GetTitle(IDocument doc)
        {
            var headers = doc.GetElementsByClassName("job-details__title");
            var header = headers.First();
            var title = header.TextContent.Trim();
            return title;
        }

        public string GetLocation(IDocument doc) => GetDetailsInfoPanelValue(doc, "Ort:");

        public string GetPosition(IDocument doc) => GetDetailsInfoPanelValue(doc, "Anstellungsart:");

        public string GetReferenceNumber(IDocument doc)
        {
            var buttons = doc.GetElementsByClassName("job-details__button");
            var button = buttons.FirstOrDefault();
            var existsButton = button != null;
            if (!existsButton)
                return "";
            var link = button.Attributes["href"].Value;
            link = link.Replace("/apply", "");
            var subsetsLink = link.Split('/');
            var referenceNumber = subsetsLink.Last();
            return referenceNumber;
        }
        public string GetStartingDate(IDocument doc) => HaysDataParser.NoInformation;
        public string GetDuration(IDocument doc) => HaysDataParser.NoInformation;
        public IEnumerable<string> GetTasks(IDocument doc) => GetJobDetailValues(doc, "Aufgaben", "Funktion", "Tag");
        public IEnumerable<string> GetQualifications(IDocument doc) => GetJobDetailValues(doc, "Anforderungen", "mitbringen", "Profil", "mitbringst");
        public IEnumerable<string> GetAdvantages(IDocument doc) => GetJobDetailValues(doc, "erwartet", "erwarten", "geboten", "Vorteile", "Benefits");
        public IEnumerable<string> GetProjectInformation(IDocument doc) => new []{ HaysDataParser.NoInformation };
        public IEnumerable<string> GetIncomeInformation(IDocument doc) => new [] {GetDetailsInfoPanelValue(doc, "Gehalt:")};
        public IEnumerable<string> GetEmployerInformation(IDocument doc) => new[] { HaysDataParser.NoInformation };
        public IEnumerable<string> GetEmploymentInformation(IDocument doc) => new[] { HaysDataParser.NoInformation };
        public Contact GetContact(IDocument doc)
        {            
            return new Contact(HaysDataParser.NoInformation, HaysDataParser.NoInformation, HaysDataParser.NoInformation);
        }

        private string GetDetailsInfoPanelValue(IDocument doc, string identifier)
        {
            var panels = doc.GetElementsByClassName("job-details__info-panel");
            var panel = panels.First();
            var panelItems = panel.Children;
            var identifiedData = panelItems.Where(item => item.TextContent.Contains(identifier));
            var existsIdentifiedData = identifiedData.Any();
            if (!existsIdentifiedData) return "";
            var data = identifiedData.First().TextContent;
            var value = data.Replace(identifier, "").Trim();
            return value;
        }

        private IEnumerable<string> GetJobDetailValues(IDocument doc, params string[] possibleIdentifiers)
        {
            var details = doc.GetElementsByClassName("job-details__content");
            var detail = details.First();
            var detailChildren = detail.Children.ToList();
            var identifiedData = detailChildren.Where(child => possibleIdentifiers.Any(identifier => child.TextContent.Contains(identifier) && child.TextContent.Length < 125));
            var existsIdentifiedData = identifiedData.Any();
            if (!existsIdentifiedData) return new[] { HaysDataParser.NoInformation };
            var data = identifiedData.First();
            var searchinDataIndex = detailChildren.IndexOf(data) + 1;
            var ul = detailChildren[searchinDataIndex];
            var lis = ul.Children;
            var values = lis.Select(li => li.TextContent.Trim());
            return values;
        }

        public IEnumerable<Uri> GetProjectUrls(IDocument doc)
        {
            var projects = doc.GetElementById("jobListing");
            var uris = projects.GetElementsByTagName("A")
                          .Where(x => x.HasAttribute("href"))
                          .Select(x => new Uri($"https://www.computerfutures.com{x.Attributes["href"].Value}"));
            return uris;
        }
    }
}
