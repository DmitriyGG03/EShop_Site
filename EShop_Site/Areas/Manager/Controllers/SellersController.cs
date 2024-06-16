using EShop_Site.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace EShop_Site.Areas.Seller.Controllers;

[Area("Manager")]
public class SellersController : Controller
{
    private readonly ILogger<ManagerHomeController> _logger;

    public SellersController(ILogger<ManagerHomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }
}