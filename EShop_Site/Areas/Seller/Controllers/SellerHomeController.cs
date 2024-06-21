using EShop_Site.Areas.Seller.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Routes;

namespace EShop_Site.Areas.Seller.Controllers;


[Authorize(Policy = Policy.OnlyForSellers)]
[Area("Seller")]
public class SellerHomeController : Controller
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SellerHomeController(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public IActionResult Home()
    {
        return View();
    }
}