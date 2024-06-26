using EShop_Site.Components;
using EShop_Site.Extensions;
using EShop_Site.Helpers;
using EShop_Site.Models;
using EShop_Site.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SharedLibrary.Models.ClientDtoModels.MainModels;
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
        _logger.LogInformation("Opening the privacy view");
        return View();
    }

    public IActionResult Terms()
    {
        _logger.LogInformation("Opening the terms view");
        return View();
    }

    public IActionResult About()
    {
        _logger.LogInformation("Opening the about view");
        return View();
    }

    [Authorize]
    public async Task<IActionResult> Profile()
    {
        //Get current user info from bd
        var response = await _httpClientService.SendRequestAsync(
            new RestRequestForm(ApiRoutes.Controllers.UserContr + ApiRoutes.UniversalActions.GetByIdAction, 
                HttpMethod.Get, jsonData: JsonConvert.SerializeObject(CookiesHelper.GetUserId(_httpContextAccessor.HttpContext))));

        var handleResult = await ResponseHandler.HandleUniversalResponseAsync<UserCDTO>(response);

        if (!handleResult.IsSuccessful)
        {
            _logger.LogWarning("Error receiving user data for profile display");
            return View(new UserForm());
        }

        var userDto = handleResult.Result;

        //Set user info to form
        var userForm = userDto.ToUserForm();
        
        _logger.LogInformation("Successful receiving user data for profile display");
        
        return View(userForm);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}