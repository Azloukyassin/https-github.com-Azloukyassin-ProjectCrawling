using AngleSharp.Dom;
using OpenQA.Selenium;
using Projektor.Core.Models;
using Projektor.Core.Services.Crawlers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Projektor.Core.Services.Crawlers.AllgeierExpertsGo
{

    class AllgeierExpertsGoDataParser : IDataParser
    {
        public static string baseUrlForCreation = "https://www.allgeier-experts-go.com";
        public string noinformation = "keine Vorhanden";
        private int index;

        public IEnumerable<string> GetAdvantages(IDocument doc)
        {
            return new[] { noinformation };
        }
        public Contact GetContact(IDocument doc)
        {
            var ansprechsParnter = "";
            var telefon = "";
            var result = "";
            var email = "";
            var result1 = "";
            var detailsContact = doc.GetElementsByClassName("jobOfferContact");
            foreach (var list in detailsContact)
            {
                var items = list.GetElementsByTagName("div").First();
                result = items.TextContent;
                ansprechsParnter += result.Substring(0, 30);
                result1 += result.Remove(0, ansprechsParnter.Length);
                telefon += result1.Substring(27, 33);

            }
            return new Contact(ansprechsParnter, telefon, email);
        }
        public string GetDuration(IDocument doc)
        {
            var result = "bis ";
            var details = doc.GetElementsByClassName("jobPageAddressField");
            int i = 1;
            foreach (var list in details)
            {
                if (i == 4)
                {
                    var duartion = list.NextElementSibling;
                    var items = duartion.TextContent;
                    result += items;
                }
                i++;
            }
            return result;
        }
        public IEnumerable<string> GetEmployerInformation(IDocument doc)
        {
            return new[] { noinformation };
        }
        public IEnumerable<string> GetEmploymentInformation(IDocument doc)
        {
            return new[] { noinformation };
        }
        public IEnumerable<string> GetIncomeInformation(IDocument doc)
        {
            return new[] { noinformation };
        }
        public string GetLocation(IDocument doc)
        {
            var result = "";
            var details = doc.GetElementsByClassName("jobPageAddressField");
            int i = 1;
            foreach (var list in details)
            {
                if (i == 2)
                {
                    var location = list.NextElementSibling;
                    var items = location.TextContent;
                    result += items;
                }
                i++;
            }
            return result;
        }

        public string GetPosition(IDocument doc)
        {
            var result = "";
            var details = doc.GetElementsByClassName("jobPageAddressField");
            int i = 1;
            foreach (var list in details)
            {
                if (i == 5)
                {
                    var position = list.NextElementSibling;
                    var items = position.TextContent;
                    result += items;
                }
                i++;
            }
            return result;
        }
        public IEnumerable<string> GetProjectInformation(IDocument doc)
        {
            var info = "";
            var items = doc.GetElementsByClassName("jobPageAddress");
            foreach (var list in items)
            {
                var ele = list.GetElementsByClassName("jobPageAddressField")
                                 .Select(x => x.TextContent);
                if (ele.Contains("Start") == true)
                {
                    var date = list.Children
                                       .Select(x => x.TextContent);
                    var infoItem = date.First();
                    info += infoItem;
                    return new[] { info };
                }
            }
            return new[] { noinformation };
        }
        public IEnumerable<Uri> GetProjectUrls(IDocument doc)
        {
            List<Uri> uris = new List<Uri>();
            var projects = doc.GetElementsByClassName("jobList");
            foreach (var links in projects)
            {
                var str = links.GetElementsByTagName("a")
                                 .Where(element => element.HasAttribute("href") && element.Attributes["href"].Value
                                                          .Contains("/recommend") == false)
                                 .Select(x => x.Attributes["href"].Value);
                int i = 1;
                foreach (var href in str)
                {
                    var baseUrl = baseUrlForCreation;
                    if (i % 2 != 0)
                    {
                        baseUrl += href;
                        uris.Add(new Uri(baseUrl));
                    }
                    i++;
                }
            }
            return uris;
        }
        public IEnumerable<string> GetQualifications(IDocument doc) => GetValuesList(doc, "Anforderung:", "Qualification");
        public string GetReferenceNumber(IDocument doc)
        {
            var titelelements = doc.GetElementsByClassName("jobPageId")
                .Select(x => x.TextContent);
            var title = titelelements.First()
                .Trim();
            return title;
        }
        public string GetStartingDate(IDocument doc)
        {
            var result = "";
            var details = doc.GetElementsByClassName("jobPageAddressField");
            int i = 1;
            foreach (var list in details)
            {
                if (i == 3)
                {
                    var datum = list.NextElementSibling;
                    var items = datum.TextContent;
                    result += items;
                }
                i++;
            }
            return result;
        }

        public IEnumerable<string> GetTasks(IDocument doc) => GetValuesList(doc, "Aufgabe:", "Task");
        public string GetTitle(IDocument doc)
        {
            var titelelements = doc.GetElementsByClassName("jobPageTitle")
                .Select(x => x.TextContent);
            var title = titelelements.First()
                .Trim();
            return title;
        }
        private IEnumerable<string> GetValuesList(IDocument doc, params string[] identifiers)
        {
            var details = doc.GetElementsByClassName("jobPageContentInner");
            var detail = details.First();
            var detailChildren = detail.Children.ToList();
            var identifiedData = detailChildren.Where(child => identifiers.Any(identifier => child.TextContent.Contains(identifier)));
            var existsIdentifiedData = identifiedData.Any();
            if (!existsIdentifiedData) return new[] { noinformation };
            var data = identifiedData.First();
            var searchinDataIndex = detailChildren.IndexOf(data);
            var ul = detailChildren[searchinDataIndex];
            var resulta = ul.TextContent;
            return new[] { resulta };
        }
    }
}
