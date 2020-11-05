using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Dom;
using Projektor.Core.Services.Crawlers.Base;
using Projektor.Core.Models;

namespace Projektor.Core.Services.Crawlers.Hays
{
    public class HaysDataParser : IDataParser
    {
        public static string NoInformation = "Keine Informationen vorhanden";
        public string GetTitle(IDocument doc)
        {
            var titleElements = doc.GetElementsByClassName("hays__job__details__job__title")
                                   .Select(x => x.InnerHtml);
            var title = titleElements.First()
                                     .Trim();
            return title;
        }

        public string GetLocation(IDocument doc)
        {
            var locationElements = doc.GetElementsByClassName("hays__job__details__job__location")
                                      .Select(x => x.InnerHtml);
            var locationRoleElement = locationElements.First()
                                                       .Split('|');
            var location = locationRoleElement.First()
                                              .Trim();
            return location;
        }

        public string GetPosition(IDocument doc)
        {
            var locationElements = doc.GetElementsByClassName("hays__job__details__job__location")
                                      .Select(x => x.InnerHtml);
            var locationRoleElement = locationElements.First()
                                                      .Split('|');
            var position = locationRoleElement.Last()
                                              .Trim();
            return position;
        }

        public string GetReferenceNumber(IDocument doc) => GetMetaItemValue(doc, "Referenznummer", "Reference number");

        public string GetStartingDate(IDocument doc) => GetMetaItemValue(doc, "Startdatum", "Start date");

        public string GetDuration(IDocument doc) => GetMetaItemValue(doc, "Projektdauer", "Project duration");

        public IEnumerable<string> GetTasks(IDocument doc) => GetListItemValues(doc, "Meine Aufgaben", "My duties");

        public IEnumerable<string> GetQualifications(IDocument doc) => GetListItemValues(doc, "Meine Qualifikationen", "My qualifications");

        public IEnumerable<string> GetAdvantages(IDocument doc) => GetListItemValues(doc, "Meine Vorteile", "My benefits");

        public IEnumerable<string> GetProjectInformation(IDocument doc) => GetListItemValues(doc, "Projektinformationen", "Project information");

        public IEnumerable<string> GetIncomeInformation(IDocument doc) => GetListItemValues(doc, "Gehaltsinformationen");

        public IEnumerable<string> GetEmployerInformation(IDocument doc) => GetListItemValues(doc, "Mein Arbeitgeber");

        public IEnumerable<string> GetEmploymentInformation(IDocument doc) => GetListItemValues(doc, "Mein Einsatz");

        public IEnumerable<Uri> GetProjectUrls(IDocument doc)
        {
            var urls = doc.GetElementsByTagName("A")
                .Where(element => element.HasAttribute("href") && element.Attributes["href"].Value
                                      .Contains("jobsuche/stellenangebote-jobs-detail-"))
                .Select(element => element.Attributes["href"].Value)
                .Distinct()
                .Select(url => new Uri(url));
            return urls;
        }

        public Contact GetContact(IDocument doc)
        {
            var nameIdentifiers = new[] { "Mein Ansprechpartner" };
            var contactOptionsIdentifier = new[] { "Kontakt aufnehmen", "Contact" };
            const string phoneIdentifier = "callto:";
            const string mailIdentifier = "mailto:";
            var nameOption = GetContactOption(doc, nameIdentifiers);
            var hasNameOption = nameOption != null;
            var name = hasNameOption ? nameIdentifiers.Aggregate(nameOption.TextContent.Trim(), (old, @new) => old.Replace(@new, "")) : NoInformation;
            var contactOption = GetContactOption(doc, contactOptionsIdentifier);
            var phoneNumber = GetContactOptionValue(contactOption, phoneIdentifier);
            var email = GetContactOptionValue(contactOption, mailIdentifier);
            return new Contact(name.Trim(), phoneNumber, email.Trim());
        }

        private string GetContactOptionValue(IElement element, string identifier)
        {
            var hasContactOption = element != null;
            if (!hasContactOption)
                return NoInformation;
            var anchorElements = element.GetElementsByTagName("span")
                                        .SelectMany(option => option.GetElementsByTagName("a"));
            var existsAnchorElements = anchorElements.Any();
            if (!existsAnchorElements) return NoInformation;
            var value = anchorElements.FirstOrDefault(anchor => anchor.Attributes["href"].Value.Contains(identifier));
            var hasValue = value != null;
            return hasValue ? value.TextContent.Trim() : NoInformation;
        }

        private IElement GetContactOption(IDocument doc, string[] identifiers)
        {
            var contactAccordionIdentifiers = new[] { "Mein Kontakt bei Hays", "My contact at Hays" };
            var accordions = doc.GetElementsByClassName("hays__job__detail__accordion");
            var contactAccordion = accordions.Where(accordion =>
            {
                var headerWrapper = accordion.Children.First();
                var header = headerWrapper.Children.First();
                var title = header.Attributes["title"].Value;
                var isContactAccordion = contactAccordionIdentifiers.Contains(title);
                return isContactAccordion;
            });
            var paragraphs = contactAccordion.First().GetElementsByTagName("P");
            var contactOption = paragraphs.FirstOrDefault(paragraph => identifiers.Any(paragraph.TextContent.Contains));
            return contactOption;
        }

        private string GetMetaItemValue(IDocument doc, params string[] identifiers)
        {
            var metaItems = doc.GetElementsByClassName("row hays__job__details__job__meta__item");
            var metaItemParent = metaItems.FirstOrDefault(item => item.Children.Any(child => identifiers.Contains(child.InnerHtml.Trim())));
            var existsStartingDate = metaItemParent != null;
            if (!existsStartingDate) return NoInformation;
            var metaItemChild = metaItemParent.Children.Last();
            var metaItemValue = metaItemChild.TextContent.Trim();
            return metaItemValue;
        }

        private IEnumerable<string> GetListItemValues(IDocument doc, params string[] identifiers)
        {
            var accordions = doc.GetElementsByClassName("hays__job__detail__accordion");
            var listIdentifier = 2;
            var accordion = accordions.FirstOrDefault(a =>
            {
                var accordionChildCount = a.Children.Count();
                var isList = accordionChildCount == listIdentifier;
                if (!isList) return false;
                var header = a.Children.First();
                var title = header.Children.First().Attributes["title"].Value;
                var isLookedForList = identifiers.Contains(title);
                return isLookedForList;
            });
            var existsAccordion = accordion != null;
            if (!existsAccordion) return new[] { NoInformation };
            var body = accordion.Children.Last();
            var unorderedList = body.Children.First();
            var listItems = unorderedList.Children;
            var contents = listItems.Select(item => item.TextContent.Trim());
            var hasContentItems = contents.Any();
            if (!hasContentItems)
            {
                var text = unorderedList.TextContent.Trim();
                contents = new[] { text };
            }
            return contents;
        }
    }
}
