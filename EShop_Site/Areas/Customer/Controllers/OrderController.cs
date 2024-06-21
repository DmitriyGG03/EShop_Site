using EShop_Site.Components;
using EShop_Site.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SharedLibrary.Models.DbModels.MainModels;
using SharedLibrary.Requests;
using SharedLibrary.Responses;
using SharedLibrary.Routes;

namespace EShop_Site.Areas.Customer.Controllers;

[Area("Customer")]
public class OrderController : Controller
{
    private readonly IHttpClientService _httpClient;
    private readonly IHttpContextAccessor _httpContext;

    public OrderController(IHttpClientService httpClient, IHttpContextAccessor httpContext)
    {
        _httpContext = httpContext;
        _httpClient = httpClient;
    }

    public async Task<IActionResult> AddProductToCartAsync(Guid id)
    {
        var response = await _httpClient.SendRequestAsync(
            new RestRequestForm(ApiRoutes.Controllers.OrderContr + ApiRoutes.OrderActions.AddProductPath, HttpMethod.Post,
                jsonData: JsonConvert.SerializeObject(new ProductCartRequest(id,
                    new Guid(_httpContext.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "id")!.Value)))));

        var serverResponse =
            await JsonHelper
                .GetTypeFromResponseAsync<LambdaResponse>(response);

        if (!response.IsSuccessStatusCode)
        {
            MessageStorage.ErrorMessage = serverResponse.ErrorInfo ?? "";
            return LocalRedirect($"~/customer/Products/ProductsList");
        }

        MessageStorage.InfoMessage = serverResponse.Info ?? "";
        return LocalRedirect($"~/customer/Products/ProductsList");
    }

    public async Task<IActionResult> DeleteProductFromCartAsync(Guid id)
    {
        var response = await _httpClient.SendRequestAsync(
            new RestRequestForm(ApiRoutes.Controllers.OrderContr + ApiRoutes.OrderActions.DeleteProductPath, HttpMethod.Delete,
                jsonData: JsonConvert.SerializeObject(new ProductCartRequest(id,
                    new Guid(_httpContext.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "id")!.Value)))));

        var serverResponse =
            await JsonHelper
                .GetTypeFromResponseAsync<LambdaResponse>(response);

        if (!response.IsSuccessStatusCode)
        {
            MessageStorage.ErrorMessage = serverResponse.ErrorInfo ?? "";
            return LocalRedirect($"~/customer/Products/ProductsList");
        }

        MessageStorage.InfoMessage = serverResponse.Info ?? "";
        return LocalRedirect($"~/customer/Products/ProductsList");
    }

    public async Task<IActionResult> CartView()
    {
        var response = await _httpClient.SendRequestAsync(
            new RestRequestForm(ApiRoutes.Controllers.OrderContr + ApiRoutes.OrderActions.GetCartPath, HttpMethod.Get,
                jsonData: JsonConvert.SerializeObject(
                    new Guid(_httpContext.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "id")!.Value))));

        var serverResponse =
            await JsonHelper
                .GetTypeFromResponseAsync<LambdaResponse<Order>>(response);

        if (!response.IsSuccessStatusCode)
        {
            MessageStorage.ErrorMessage = serverResponse.ErrorInfo ?? "";
            return View();
        }

        if (serverResponse.ResponseObject is null) throw new Exception("Response object is null");

        return View(serverResponse.ResponseObject);
    }

    public async Task<IActionResult> OrdersView()
    {
        var response = await _httpClient.SendRequestAsync(
            new RestRequestForm(ApiRoutes.Controllers.OrderContr + ApiRoutes.OrderActions.GetOrdersPath, HttpMethod.Get,
                jsonData: JsonConvert.SerializeObject(new Guid(
                    _httpContext.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "id")!.Value))));

        var serverResponse =
            await JsonHelper
                .GetTypeFromResponseAsync<LambdaResponse<List<Order>>>(response);

        if (!response.IsSuccessStatusCode)
        {
            MessageStorage.ErrorMessage = serverResponse.ErrorInfo ?? "";
            return View();
        }

        if (serverResponse.ResponseObject is null) throw new Exception("Response object is null");

        return View(serverResponse.ResponseObject);
    }
}