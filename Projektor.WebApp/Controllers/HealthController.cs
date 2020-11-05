using Microsoft.AspNetCore.Mvc;
using Projektor.Core;
using System.Linq;
using System.Threading;

namespace Projektor.WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly Engine _engine;

        public HealthController(Engine engine)
        {
            _engine = engine;
        }

        [HttpGet]
        public IActionResult OnGet(CancellationToken token)
        {
            return new JsonResult(_engine.Projects.Count());
        }
    }
}
