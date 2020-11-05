using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AngleSharp.Common;
using AngleSharp.Dom;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using Projektor.Core.Models;
using Projektor.Core.Services.Crawlers.Base;
using Projektor.Core.Services.Crawlers.Hays;

namespace Projektor.Core.Services.Crawlers.ComputerFutures
{
    public class ComputerFuturesDataParser : IDataParser
    {
        public static string NoInformation = "Keine vorhanden";
        private static string baseUrlForCreation = "https://www.computerfutures.com";
        public string GetTitle(IDocument doc)
        {
            var headers = doc.GetElementsByClassName("job-details__title");
            var header = headers.First();
            var title = header.TextContent.Trim();
            return title;

        }
        public string GetLocation(IDocument doc)
        {
            var locResult = "";
            var details = doc.GetElementsByClassName("job-details__info-panel");
            int i = 1;
            foreach (var list in details)
            {
                if (i == 1)
                {
                    var items = list.GetElementsByClassName("job-details__info-item")
                                    .Select(x => x.TextContent);
                    var locItems = items.First();
                    locResult += locItems.Substring(5).Trim();
                }
                i++;
            }
            return locResult;
        }
        public string GetPosition(IDocument doc)
        {
            var posResult = "";
            var details = doc.GetElementsByClassName("job-details__info-item");
            int i = 1;
            foreach (var list in details)
            {
                if (i == 4)
                {
                    var items = list.TextContent;
                    var posItems = items.Substring(16).Trim();
                    posResult += posItems;
                }
                i++;
            }
            return posResult;
        }
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
        public string GetStartingDate(IDocument doc) => NoInformation;
        public string GetDuration(IDocument doc) => NoInformation;
        public IEnumerable<string> GetTasks(IDocument doc) => GetJobDetailValues(doc, "Aufgaben", "Funktion", "Tag", "IHRE AUFGABEN", "Responsibilities:");
        public IEnumerable<string> GetQualifications(IDocument doc) => GetJobDetailValues(doc, "Anforderungen", "mitbringen", "Profil", "mitbringst");
        public IEnumerable<string> GetAdvantages(IDocument doc) => GetJobDetailValues(doc, "erwartet", "erwarten", "geboten", "Vorteile", "Benefits");
        public IEnumerable<string> GetProjectInformation(IDocument doc) => new[] { NoInformation };
        public IEnumerable<string> GetIncomeInformation(IDocument doc) => new[] { GetDetailsInfoPanelValue(doc, "Gehalt:") };
        public IEnumerable<string> GetEmployerInformation(IDocument doc) => new[] { NoInformation };
        public IEnumerable<string> GetEmploymentInformation(IDocument doc) => new[] { NoInformation };
        public Contact GetContact(IDocument doc)
        {
            return new Contact("Bodo Albrechtsen", "040 360 06 1099", "b.albrechtsen@computerfutures.de");
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
            if (!existsIdentifiedData) return new[] { NoInformation };
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
