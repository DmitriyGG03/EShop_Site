using EShop_Site.Components;
using EShop_Site.Helpers;
using EShop_Site.Models;
using EShop_Site.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SharedLibrary.Models.MainModels;
using SharedLibrary.Responses;
using SharedLibrary.Routes;

namespace EShop_Site.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHttpClientService _httpClientService;

    public HomeController(ILogger<HomeController> logger, IHttpClientService httpClientService,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClientService = httpClientService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Terms()
    {
        return View();
    }

    public IActionResult About()
    {
        return View();
    }
    
    public IActionResult Home()
    {
        return View();
    }

    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var userId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "id")!.Value;
        
        var response = await _httpClientService.SendRequestAsync(
            new RestRequestForm(ApiRoutes.Controllers.User + ApiRoutes.User.GetById, 
                HttpMethod.Get, jsonData: JsonConvert.SerializeObject(userId)));
        
        var errorResponse = await JsonHelper.GetTypeFromResponseAsync<LambdaResponse<User>>(response);
        
        if (!response.IsSuccessStatusCode)
        {
            return View(new UserData() {Message = errorResponse.Info});
        }

        User user = errorResponse.ResponseObject;

        return View(new UserData()
        {
            Name = user.Name,
            LastName = user.LastName,
            Patronymic = user.Patronymic,
            
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
        });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}