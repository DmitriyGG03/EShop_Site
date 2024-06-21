using EShop_Site.Components;
using EShop_Site.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SharedLibrary.Models.DbModels.MainModels;
using SharedLibrary.Requests;
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

    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var userId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "id")!.Value;
        
        var response = await _httpClientService.SendRequestAsync(
            new RestRequestForm(ApiRoutes.Controllers.UserContr + ApiRoutes.UserActions.GetByIdPath, 
                HttpMethod.Get, jsonData: JsonConvert.SerializeObject(userId)));
        
        var userResponse = await JsonHelper.GetTypeFromResponseAsync<LambdaResponse<User>>(response);
        
        if (!response.IsSuccessStatusCode)
        {
            MessageStorage.ErrorMessage = userResponse.ErrorInfo ?? "";
            MessageStorage.InfoMessage = userResponse.Info ?? "";
            return View(new EditUserRequest());
        }

        User user = userResponse.ResponseObject ?? throw new Exception("Received null user object!");

        var request = new EditUserRequest()
        {
            UserId = user.UserId,

            Name = user.Name,
            LastName = user.LastName,
            Patronymic = user.Patronymic,

            Email = user.Email,
            PhoneNumber = user.PhoneNumber,

            RoleId = user.RoleId,
            SellerId = user.SellerId,
        };
        
        TempData["EditUserRequest"] = JsonConvert.SerializeObject(request);
        
        return View(request);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}