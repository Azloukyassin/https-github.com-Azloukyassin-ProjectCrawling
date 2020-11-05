using System;
using System.Collections.Generic;
using AngleSharp.Dom;
using Projektor.Core.Models;

namespace Projektor.Core.Services.Crawlers.Base
{
    public interface IDataParser
    {
        string GetTitle(IDocument doc);
        string GetLocation(IDocument doc);
        string GetPosition(IDocument doc);
        string GetReferenceNumber(IDocument doc);
        string GetStartingDate(IDocument doc);
        string GetDuration(IDocument doc);
        IEnumerable<string> GetTasks(IDocument doc);
        IEnumerable<string> GetQualifications(IDocument doc);
        IEnumerable<string> GetAdvantages(IDocument doc);
        IEnumerable<string> GetProjectInformation(IDocument doc);
        IEnumerable<string> GetIncomeInformation(IDocument doc);
        IEnumerable<string> GetEmployerInformation(IDocument doc);
        IEnumerable<string> GetEmploymentInformation(IDocument doc);
        IEnumerable<Uri> GetProjectUrls(IDocument doc);
        Contact GetContact(IDocument doc);
    }
}
