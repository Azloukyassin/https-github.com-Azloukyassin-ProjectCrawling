using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AngleSharp;
using AngleSharp.Dom;
using OpenQA.Selenium;
using Projektor.Core.Database;
using Projektor.Core.Models;
using Projektor.Core.Services.Crawlers.Base;

namespace Projektor.Core.Services.Crawlers.FreelancerMap
{

    public class FreelancerMapDataParser : IDataParser
    {
        public static string baseUrlForCreation = "https://www.freelancermap.de";
        public static string noinfo = "Keine Information Vorhanden ";
        private int index;
        public string GetTitle(IDocument doc)
        {
            var titelelements = doc.GetElementsByClassName("top_seo")
                .Select(x => x.Text());
            var title = titelelements.First()
                .Trim();
            return title;
        }
        public string GetLocation(IDocument doc)
        {
            var locationElements = doc.GetElementsByClassName("address")
                                     .Select(x => x.TextContent);
            var locationRoleElement = locationElements.First();
            var location = locationRoleElement.Trim();
            return location;
        }
        public string GetPosition(IDocument doc)
        {
            var pos = "";
            var details = doc.GetElementsByClassName("project-detail").First();

            var element = details.GetElementsByClassName("project-detail-title")
                             .Select(x => x.TextContent);
            if (element.Contains("Vertragsart:") == true)
            {

                var position = details.GetElementsByClassName("project-detail-description")
                                   .Select(x => x.TextContent);
                var positionItem = position.First();
                pos += positionItem;
                return pos;
            }
            return noinfo;
        }
        public string GetReferenceNumber(IDocument doc)
        {
            var referenceNum = "";
            var items = doc.GetElementsByClassName("project-detail");
            foreach (var list in items)
            {
                var ele = list.GetElementsByClassName("project-detail-title")
                                 .Select(x => x.TextContent);
                if (ele.Contains("Projekt-ID:") == true)
                {
                    var numm = list.GetElementsByClassName("project-detail-description")
                                       .Select(x => x.TextContent);
                    var referItem = numm.First();
                    referenceNum += referItem;
                    return referenceNum;
                }
            }
            return noinfo;
        }
        public string GetStartingDate(IDocument doc)
        {
            var datum = "";
            var items = doc.GetElementsByClassName("project-detail");
            foreach (var list in items)
            {
                var ele = list.GetElementsByClassName("project-detail-title")
                                 .Select(x => x.TextContent);
                if (ele.Contains("Start:") == true)
                {
                    var date = list.GetElementsByClassName("project-detail-description")
                                       .Select(x => x.TextContent);
                    var datumItem = date.First();
                    datum += datumItem;
                    return datum;
                }
            }
            return noinfo;
        }
        public string GetDuration(IDocument doc)
        {
            var dauer = "";
            var items = doc.GetElementsByClassName("project-detail");
            foreach (var list in items)
            {
                var ele = list.GetElementsByClassName("project-detail-title")
                                 .Select(x => x.TextContent);
                if (ele.Contains("Dauer:") == true)
                {
                    var dauration = list.GetElementsByClassName("project-detail-description")
                                       .Select(x => x.TextContent);
                    var dauerItem = dauration.First();
                    dauer += dauerItem;
                    return dauer;
                }
            }
            return noinfo;
        }

        public IEnumerable<string> GetTasks(IDocument doc) => GetListValue(doc, "Aufgabenbeschreibung", "Aufgabe", "Ihre Aufgaben sind:", "Ihre Aufgaben:", "Aufgabenfelder:", "Task:");
        public IEnumerable<string> GetQualifications(IDocument doc)
        {
            var aufgabeItem = "";
            var details = doc.GetElementsByClassName("cat_object");
            foreach (var list in details)
            {
                var items = list.GetElementsByTagName("a")
                           .Select(x => x.TextContent);

                var aufitem = items.First();
                aufgabeItem += aufitem + " * ";
            }
            return new[] { aufgabeItem };
        }
        public IEnumerable<string> GetAdvantages(IDocument doc) => new[] { noinfo };
        public IEnumerable<string> GetProjectInformation(IDocument doc) => new[] { noinfo };
        public IEnumerable<string> GetIncomeInformation(IDocument doc) => new[] { noinfo };
        public IEnumerable<string> GetEmployerInformation(IDocument doc) => new[] { noinfo };
        public IEnumerable<string> GetEmploymentInformation(IDocument doc) => new[] { noinfo };
        public IEnumerable<Uri> GetProjectUrls(IDocument doc)
        {
            List<Uri> uris = new List<Uri>();
            var projects = doc.GetElementsByClassName("project-row");
            foreach (var links in projects)
            {
                var str = links.GetElementsByTagName("a")
                                 .Where(element => element.HasAttribute("href") && element.Attributes["href"].Value
                                          .Contains("projektboerse/projekte/it/"))
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
        public Contact GetContact(IDocument doc)
        {
            var Ansprechpartnername = "";
            var items = doc.GetElementsByClassName("project-detail");
            foreach (var list in items)
            {
                var ele = list.GetElementsByClassName("project-detail-title")
                              .Select(x => x.TextContent);
                if (ele.Contains("Ansprechpartner:") == true)
                {
                    var name = list.GetElementsByClassName("project-detail-description")
                                   .Select(x => x.TextContent);
                    var nameItem = name.First();
                    Ansprechpartnername += nameItem;
                }
            }
            if (Ansprechpartnername == "") Ansprechpartnername = "kein Vorhanden";
            return new Contact(Ansprechpartnername, noinfo, noinfo);
        }

        private IEnumerable<string> GetListValue(IDocument doc, params string[] identifiers)
        {

            var targetDescriptionMinimumChildCount = 2;
            var descriptions = doc.GetElementsByClassName("projectcontent");
            var description = descriptions.FirstOrDefault(d => d.Children.Length >= targetDescriptionMinimumChildCount);
            var existsDescription = description != null;
            if (!existsDescription) return new[] { noinfo };
            var descriptionChildren = description.Children.ToList();
            var unorderedList = descriptionChildren.Where(element => element.TagName == "UL").ToList();
            var tasksList = unorderedList[index];
            var taskItems = tasksList.Children;
            var otherUL = taskItems.Where(item => item.TagName == "UL");
            var hasOtherUL = otherUL.Any();
            var taskItemValues = new List<string>();
            if (hasOtherUL)
            {
                foreach (var ul in otherUL)
                {
                    var items = ul.Children.Select(child => child.TextContent);
                    taskItemValues.AddRange(items);
                }
            }
            else
            {
                var items = taskItems.Select(item => item.TextContent.Trim());
                taskItemValues.AddRange(items);
            }
            return taskItemValues;
        }
    }
}
