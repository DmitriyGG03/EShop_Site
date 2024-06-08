using EShop_Site.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace EShop_Site.Areas.Manager.Controllers;

[Area("Manager")]
public class ProductsController : Controller
{
    private readonly ILogger<ManagerHomeController> _logger;

    public ProductsController(ILogger<ManagerHomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        
        return View();
    }
}