using Microsoft.AspNetCore.Mvc;
using Projektor.WebApp.Pages.Components.TdUl;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Projektor.WebApp.ViewComponents
{
    public class TdUlViewComponent : ViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync(IEnumerable<string> source)
        {
            return Task.FromResult((IViewComponentResult)View(new TdUlModel(source)));
        }
    }
}
