using Microsoft.AspNetCore.Mvc;

namespace OIG.Assessment.Api.Controllers;

public class HomeController : ControllerBase
{
    [HttpGet]
    public IActionResult Index()
    {
        return new RedirectResult("~/swagger");
    }
}

