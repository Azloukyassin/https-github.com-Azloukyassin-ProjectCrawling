using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Projektor.Core;
using Projektor.Core.Models;

namespace Projektor.WebApp.Pages
{
    public class ProjectsModel : PageModel
    {
        public List<Project> Projects { get; }
        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }
        public bool ShowProject => Id > -1;
        public Project Project => Projects[Id];
        public ProjectsModel(Engine engine)
        {
            Id = -1;
            Projects = engine.Projects.ToList();
        }
        public void OnGet()
        {
            
        }
    }
}