using Microsoft.AspNetCore.Mvc;

namespace EShop_Site.Controllers;

public class AuthenticationController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public AuthenticationController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Login()
    {
        return View();
    }
    
    public IActionResult Register()
    {
        return View();
    }
}