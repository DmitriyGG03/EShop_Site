using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Routes;

namespace EShop_Site.Areas.Manager.Controllers;

[Authorize(Policy = Policy.OnlyForManagers)]
[Area("Manager")]
public class OrdersController : Controller
{
    private readonly ILogger<ManagerHomeController> _logger;

    public OrdersController(ILogger<ManagerHomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult OrdersList()
    {
        return View();
    }
}