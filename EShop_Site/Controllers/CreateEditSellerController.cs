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
public class CreateEditSellerController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHttpClientService _httpClient;

    public CreateEditSellerController(ILogger<HomeController> logger, IHttpClientService httpClientService,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClientService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<IActionResult> CreateSellerAsync()
    {
        var model = new EditSellerRequest();
        
        return View("EditSeller", model);
    }

    public async Task<IActionResult> EditSellerAsync(Guid id)
    {
        var sellerDataJson = TempData["EditSellerRequest"] as string;
        if (sellerDataJson is null) throw new Exception("Edit data not found");

        var models = JsonConvert.DeserializeObject<List<EditSellerRequest>>(sellerDataJson);
        var model = models.FirstOrDefault(m => m.SellerId.Equals(id));
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> EditSellerAsync(EditSellerRequest editSellerRequest)
    {
        if (!ModelState.IsValid) return View(editSellerRequest);

        HttpResponseMessage response;

        if (editSellerRequest.SellerId.Equals(Guid.Empty))
        {
            response = await _httpClient.SendRequestAsync(new RestRequestForm(
                ApiRoutes.Controllers.SellerContr + ApiRoutes.SellerActions.CreatePath,
                HttpMethod.Post,
                jsonData: JsonConvert.SerializeObject(editSellerRequest)));
        }
        else
        {
            response = await _httpClient.SendRequestAsync(new RestRequestForm(
                ApiRoutes.Controllers.SellerContr + ApiRoutes.SellerActions.EditPath,
                HttpMethod.Put,
                jsonData: JsonConvert.SerializeObject(editSellerRequest)));
        }


        return await ResponseHandlerAsync<Seller>(response);
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

        return RedirectToAction("SellersList", "Sellers", new { area = "" });
    }
}