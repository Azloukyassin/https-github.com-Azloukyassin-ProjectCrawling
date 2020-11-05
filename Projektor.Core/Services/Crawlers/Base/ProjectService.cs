using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using AngleSharp.Dom;
using Projektor.Core.Models;

namespace Projektor.Core.Services.Crawlers.Base
{
    public class ProjectService
    {
        private BlockingCollection<Project> _projects;
        public ProjectService()
        {
            _projects = new BlockingCollection<Project>();
        }

        public IEnumerable<Project> Projects => _projects.ToArray();
        public ProjectResult TryBuildProject(IDocument doc, Uri uri, IDataParser provider)
        {
            try
            {
                var title = provider.GetTitle(doc);
                var location = provider.GetLocation(doc);
                var referenceNumber = provider.GetReferenceNumber(doc);
                var startingDate = provider.GetStartingDate(doc);
                var duration = provider.GetDuration(doc);
                var position = provider.GetPosition(doc);
                var tasks = provider.GetTasks(doc);
                var qualifications = provider.GetQualifications(doc);
                var advantages = provider.GetAdvantages(doc);
                var incomeInformation = provider.GetIncomeInformation(doc);
                var projectInformation = provider.GetProjectInformation(doc);
                var employerInformation = provider.GetEmployerInformation(doc);
                var employmentInformation = provider.GetEmploymentInformation(doc);
                var contact = provider.GetContact(doc);
                var project = new Project(title, location, referenceNumber, startingDate, duration, position, tasks,
                    qualifications, advantages, incomeInformation, projectInformation, employerInformation,
                    employmentInformation, contact, uri);
                return new ProjectResult(true, null, project);
            }
            catch (Exception ex)
            {
                return new ProjectResult(false, ex, null);
            }
        }

        public void AddProject(Project project)
        {
            _projects.Add(project);
        }
    }
}
