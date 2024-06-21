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

[Authorize]
public class CreateEditUserController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHttpClientService _httpClient;

    public CreateEditUserController(ILogger<HomeController> logger, IHttpClientService httpClientService,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClientService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<IActionResult> EditUserAsync()
    {
        var userDataJson = TempData["EditUserRequest"] as string;
        if (userDataJson is null) throw new Exception("Edit data not found");

        var model = JsonConvert.DeserializeObject<EditUserRequest>(userDataJson);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> EditUserAsync(EditUserRequest editUserRequest)
    {
        if (!ModelState.IsValid) return View();

        var response = await _httpClient.SendRequestAsync(new RestRequestForm(
            ApiRoutes.Controllers.UserContr + ApiRoutes.UserActions.EditPath,
            HttpMethod.Put,
            jsonData: JsonConvert.SerializeObject(editUserRequest)));

        return await ResponseHandlerAsync<User>(response);
    }

    private async Task<IActionResult> ResponseHandlerAsync<T>(HttpResponseMessage response) where T : class
    {
        var serverResponse = await JsonHelper.GetTypeFromResponseAsync<LambdaResponse<T>>(response);

        if (!response.IsSuccessStatusCode)
        {
            MessageStorage.ErrorMessage = serverResponse.ErrorInfo ?? "";

            return View();
        }

        MessageStorage.InfoMessage = serverResponse.Info ?? "";

        return RedirectToAction("", "", new { area = "" });
    }
}