using EShop_Site.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SharedLibrary.Models.Enums;
using SharedLibrary.Routes;

namespace EShop_Site.Areas.Seller.Controllers;

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