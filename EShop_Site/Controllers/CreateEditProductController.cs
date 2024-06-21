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
public class CreateEditProductController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHttpClientService _httpClient;

    public CreateEditProductController(ILogger<HomeController> logger, IHttpClientService httpClientService,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClientService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<IActionResult> CreateProductAsync()
    {
        var model = new EditProductRequest();

        return View("EditProduct", model);
    }

    public async Task<IActionResult> EditProductAsync(Guid id)
    {
        var sellerDataJson = TempData["EditProductRequest"] as string;
        if (sellerDataJson is null) throw new Exception("Edit data not found");

        var models = JsonConvert.DeserializeObject<List<EditProductRequest>>(sellerDataJson);
        var model = models.FirstOrDefault(m => m.ProductId.Equals(id));
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> EditProductAsync(EditProductRequest editProductRequest)
    {
        if (!ModelState.IsValid) return View(editProductRequest);

        HttpResponseMessage response;

        if (editProductRequest.ProductId.Equals(Guid.Empty))
        {
            if (editProductRequest.SellerId.Equals(Guid.Empty))
            {
                var userId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "id")!.Value;
                editProductRequest.SellerId = await GetSellerIdByUserIdAsync(new Guid(userId));
            }

            response = await _httpClient.SendRequestAsync(new RestRequestForm(
                ApiRoutes.Controllers.ProductContr + ApiRoutes.ProductActions.CreatePath,
                HttpMethod.Post,
                jsonData: JsonConvert.SerializeObject(editProductRequest)));
        }
        else
        {
            if (editProductRequest.SellerId.Equals(Guid.Empty)) throw new Exception("Seller id is empty");

            response = await _httpClient.SendRequestAsync(new RestRequestForm(
                ApiRoutes.Controllers.ProductContr + ApiRoutes.ProductActions.EditPath,
                HttpMethod.Put,
                jsonData: JsonConvert.SerializeObject(editProductRequest)));
        }


        return await ResponseHandlerAsync<Product>(response);
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

    private async Task<Guid> GetSellerIdByUserIdAsync(Guid userId)
    {
        var response = await _httpClient.SendRequestAsync(new RestRequestForm(
            ApiRoutes.Controllers.SellerContr + ApiRoutes.SellerActions.GetSellerIdByUserIdPath, HttpMethod.Get,
            jsonData: JsonConvert.SerializeObject(userId)));
        
        var serverResponse = await JsonHelper.GetTypeFromResponseAsync<LambdaResponse<string>>(response);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Can`t find seller");
        }
        
        if (serverResponse.ResponseObject is null) throw new Exception("Response object is null");

        Guid sellerId = new Guid(serverResponse.ResponseObject);

        if (sellerId.Equals(Guid.Empty)) throw new Exception("Response object is null");

        return sellerId;
    }
}