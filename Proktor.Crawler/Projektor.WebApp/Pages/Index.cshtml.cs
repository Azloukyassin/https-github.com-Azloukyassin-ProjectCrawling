using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Projektor.Core;
using Projektor.Core.Models;

namespace Projektor.WebApp.Pages
{
    public class IndexModel : PageModel
    {
        public readonly Engine Engine;
        [BindProperty(SupportsGet = true)]
        public int Pagination { get; set; }
        public bool IsEngineSearching => Engine.IsSearching;
        public string TimeRemaining
        {
            get
            {
                var time = Engine.RemainingTimeUntilCrawlingIsStartedAgain;
                if (time == TimeSpan.MaxValue) return "Suche wurde nicht gestartet.";
                //ToDo Remove hours, because was added for docker container
                var text = $"Eine neue Suche findet um {DateTime.Now.Add(time).AddHours(2).ToString("HH:mm:ss")} statt.";
                return text;
            }
        }

        public string SearchTitle
        {
            get
            {
                var title = $"Suche {(IsEngineSearching ? "läuft" : "abgeschlossen")}";
                return title;
            }
        }
        public IEnumerable<Project> Projects => ProjectsOnPagePage(Pagination);
        public bool HasProjects => Projects.Any();
        public bool IsEngineStarted => Engine.IsStarted;
        public bool HasNextPage => ProjectsOnPagePage(Pagination + 1).Any();
        public bool HasPreviousPage => ProjectsOnPagePage(Pagination - 1).Any() && Pagination > 0;
        public string SearchText
        {
            get
            {
                var text = IsEngineSearching
                    ? "Bitte laden Sie ab und zu die Seite neu, um die aktuellsten Projekte zu erhalten."
                    : TimeRemaining;
                return text;
            }
        }

        public IndexModel(Engine engine)
        {
            Engine = engine;
        }

        public async Task<IActionResult> OnPostStartEngine(CancellationToken token)
        {
            if (!Engine.IsStarted) Engine.Start();
            await Task.Delay(4000, token);
            return RedirectToPage("Index");
        }

        public IActionResult OnPostStopEngine(CancellationToken token)
        {
            Engine.Stop();
            return RedirectToPage("Index");
        }
        private IEnumerable<Project> ProjectsOnPagePage(int pageNumber)
        {
            var projectsToDisplay = 10;
            var projectsToSkip = projectsToDisplay * pageNumber;
            var projects = Engine.Projects.Skip(projectsToSkip).Take(projectsToDisplay);
            return projects;
        }
    }
}