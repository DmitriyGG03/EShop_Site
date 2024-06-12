using EShop_Site.Components;
using EShop_Site.Helpers;
using EShop_Site.Models;
using EShop_Site.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SharedLibrary.Requests;
using SharedLibrary.Responses;
using SharedLibrary.Routes;

namespace EShop_Site.Controllers;

public class AuthenticationController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IHttpClientService _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthenticationController(ILogger<HomeController> logger, IHttpClientService httpClientService,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _httpClient = httpClientService;
    }

    public async Task<IActionResult> LoginAsync()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> LoginAsync(LoginRequest loginRequest)
    {
        if (!ModelState.IsValid) return View();

        var response = await _httpClient.SendRequestAsync(new RestRequestForm(ApiRoutes.Authentication.Login,
            HttpMethod.Post,
            jsonData: JsonConvert.SerializeObject(loginRequest)));

        var authResponse = await JsonHelper.GetTypeFromResponseAsync<AuthenticationResponse>(response);

        if (!response.IsSuccessStatusCode)
        {
            foreach (var info in authResponse.Info ?? throw new Exception("Response info is null!"))
            {
                ModelState.AddModelError(string.Empty, info);
            }

            return View();
        }

        var jsonCookies = new UserCookies()
        {
            JwtToken = authResponse.Token
        };

        _httpContextAccessor.HttpContext.Response.Cookies.Append("TradeWave", JsonConvert.SerializeObject(jsonCookies));

        return RedirectToAction("Index", "ManagerHome", new { area = "Manager" });
    }

    public async Task<IActionResult> RegisterAsync()
    {
        return View();
    }
}