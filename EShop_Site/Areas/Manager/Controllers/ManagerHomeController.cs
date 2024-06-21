using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Routes;

namespace EShop_Site.Areas.Manager.Controllers;

[Authorize(Policy = Policy.OnlyForManagers)]
[Area("Manager")]
public class ManagerHomeController : Controller
{
    private readonly ILogger<ManagerHomeController> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ManagerHomeController(ILogger<ManagerHomeController> logger, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public IActionResult Home()
    {
        return View();
    }
}