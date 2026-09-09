using Microsoft.AspNetCore.Mvc;

namespace BerberVio.Areas.Cms.Controllers;

public class DashboardController : BaseController
{
    public IActionResult Index()
    {
        return View();
    }
}
