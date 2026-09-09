using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BerberVio.Areas.Cms.Controllers;

[Area("Cms")]
[Authorize]
public abstract class BaseController : Controller
{
}
