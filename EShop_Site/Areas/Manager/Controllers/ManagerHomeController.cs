using Microsoft.AspNetCore.Mvc;

namespace EShop_Site.Areas.Manager.Controllers;

[Area("Manager")]
public class ManagerHomeController : Controller
{
    private readonly ILogger<ManagerHomeController> _logger;

    public ManagerHomeController(ILogger<ManagerHomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }
}