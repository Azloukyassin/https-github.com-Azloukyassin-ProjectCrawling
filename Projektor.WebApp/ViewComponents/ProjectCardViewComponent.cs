using Microsoft.AspNetCore.Mvc;
using Projektor.Core.Models;
using System.Threading.Tasks;

namespace Projektor.WebApp.ViewComponents
{
    public class ProjectCardViewComponent : ViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync(Project project)
        {
            return Task.FromResult((IViewComponentResult)View(project));
        }
    }
}
