using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Routes;

namespace EShop_Site.Areas.Seller.Controllers;


[Authorize(Policy = Policy.OnlyForSellers)]
[Area("Seller")]
public class SellerHomeController : Controller
{
    private readonly ILogger<ManagerHomeController> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SellerHomeController(ILogger<ManagerHomeController> logger, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public IActionResult Home()
    {
        return View();
    }
}