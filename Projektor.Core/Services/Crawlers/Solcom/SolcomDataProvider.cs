using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Dom;
using Projektor.Core.Services.Crawlers.Base;
using Projektor.Core.Services.Crawlers.Hays;
using Projektor.Core.Models;
using OpenQA.Selenium;

namespace Projektor.Core.Services.Crawlers.Solcom
{
    public class SolcomDataParser : IDataParser
    {
        public static string baseUrlForCreation = "https://www.solcom.de";
        public static string NoInformation = "Keine Vorhanden";
        public string GetTitle(IDocument doc)
        {
            var headers = doc.GetElementsByClassName("hyphenate").First();
            var header = headers.TextContent;
            return header;
        }

        public string GetLocation(IDocument doc)
        {
            var details = doc.GetElementsByClassName("pin-icon").First();
            var duration = details.GetElementsByClassName("icon-value");
            var Resultatlocation = duration.Select(x => x.TextContent).First();
            return Resultatlocation.Trim();
        }

        public string GetPosition(IDocument doc)
        {
            var details = doc.GetElementsByClassName("bag-icon").First();
            var duration = details.GetElementsByClassName("icon-value");
            var Resultatpos = duration.Select(x => x.TextContent).First();
            return Resultatpos.Trim();
        }

        public string GetReferenceNumber(IDocument doc)
        {
            var jobNumbers = doc.GetElementsByClassName("project-header").First();
            var refNum = jobNumbers.GetElementsByTagName("div");
            var jobNumber = refNum.First().TextContent.Replace("Projekt-Nr.:", "").Trim();
            return jobNumber;

        }

        public string GetStartingDate(IDocument doc)
        {
            var details = doc.GetElementsByClassName("calendar-icon").First();
            var duration = details.GetElementsByClassName("icon-value");
            var Resultatstart = duration.Select(x => x.TextContent).First();
            return Resultatstart.Trim();
        }

        public string GetDuration(IDocument doc)
        {
            var details = doc.GetElementsByClassName("clock-icon").First();
            var duration = details.GetElementsByClassName("icon-value");
            var Resultatduration = duration.Select(x => x.TextContent).First();
            return Resultatduration.Trim();
        }

        public IEnumerable<string> GetTasks(IDocument doc) => GetDescriptionListItemValues(doc, 0);

        public IEnumerable<string> GetQualifications(IDocument doc) => GetDescriptionListItemValues(doc, 1);

        public IEnumerable<string> GetAdvantages(IDocument doc)
        {
            return new[] { HaysDataParser.NoInformation };
        }

        public IEnumerable<string> GetProjectInformation(IDocument doc)
        {
            var emptyResponse = new[] { HaysDataParser.NoInformation };
            var projectInformationIdentifier = "Aktuell sind wir";
            var targetDescriptionMinimumChildCount = 2;
            var descriptions = doc.GetElementsByClassName("neos-nodetypes-text projekt-desc");
            var description = descriptions.FirstOrDefault(d => d.Children.Length >= targetDescriptionMinimumChildCount);
            var existsDescription = description != null;
            if (!existsDescription) return emptyResponse;
            var descriptionChildren = description.Children.ToList();
            var projectInformation = descriptionChildren.FirstOrDefault(element => element.TextContent.Trim().Contains(projectInformationIdentifier));
            var existsProjectInformation = projectInformation != null;
            if (!existsProjectInformation) return emptyResponse;
            var information = projectInformation.TextContent.Trim();
            //Cannot use Append on emptyReponse due to Anglesharp having defined another Append method
            var projectsInformation = new[] { information };
            return projectsInformation;

        }

        public IEnumerable<string> GetIncomeInformation(IDocument doc)
        {
            return new[] { HaysDataParser.NoInformation };
        }

        public IEnumerable<string> GetEmployerInformation(IDocument doc)
        {
            return new[] { HaysDataParser.NoInformation };
        }

        public IEnumerable<string> GetEmploymentInformation(IDocument doc)
        {
            return new[] { HaysDataParser.NoInformation };
        }

        public IEnumerable<Uri> GetProjectUrls(IDocument doc)
        {

            List<Uri> uris = new List<Uri>();
            var projects = doc.GetElementsByClassName("project-header");
            foreach (var links in projects)
            {
                var str = links.GetElementsByTagName("a")
                                 .Where(element => element.HasAttribute("href") && element.Attributes["href"].Value
                                          .Contains("/de/"))
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
            return new Contact(HaysDataParser.NoInformation, HaysDataParser.NoInformation, HaysDataParser.NoInformation);
        }

        private string GetListItemValue(IDocument doc, string identifier)
        {
            var dataList = doc.GetElementsByTagName("dl").First();
            var dataListItems = dataList.Children.ToList();
            var identifiedData = dataListItems.FirstOrDefault(item => item.TextContent.Trim() == identifier);
            var existsData = identifiedData != null;
            if (!existsData) return "";
            var searchingDataIndex = dataListItems.IndexOf(identifiedData) + 1;
            var searchedData = dataListItems[searchingDataIndex].TextContent.Trim();
            return searchedData;

        }

        private IEnumerable<string> GetDescriptionListItemValues(IDocument doc, int index)
        {
            var targetDescriptionMinimumChildCount = 2;
            var descriptions = doc.GetElementsByClassName("neos-nodetypes-text projekt-desc");
            var description = descriptions.FirstOrDefault(d => d.Children.Length >= targetDescriptionMinimumChildCount);
            var existsDescription = description != null;
            if (!existsDescription) return new[] { HaysDataParser.NoInformation };
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
