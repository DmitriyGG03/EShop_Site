using EShop_Site.Areas.Manager.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace EShop_Site.Areas.Customer.Controllers;

[Area("Customer")]
public class CustomerHomeController : Controller
{
    private readonly ILogger<ManagerHomeController> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CustomerHomeController(ILogger<ManagerHomeController> logger, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public IActionResult Home()
    {
        return View();
    }
}