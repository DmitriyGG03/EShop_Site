using EShop_Site.Components;
using EShop_Site.Helpers;
using EShop_Site.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SharedLibrary.Models.ClientDtoModels.MainModels;
using SharedLibrary.Requests;
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

    /// <param name="id">Is a product id; just "id" because i use general JS script.</param>
    public async Task<IActionResult> AddProductToCartAsync(Guid id) 
    {
        var response = await _httpClient.SendRequestAsync(
            new RestRequestForm(
                endPoint: ApiRoutes.Controllers.OrderContr + ApiRoutes.OrderActions.AddProductToCartOrderAction, 
                requestMethod: HttpMethod.Post,
                jsonData: JsonConvert.SerializeObject(new ProductCartRequest(id, CookiesHelper.GetUserId(_httpContext)))));

        var requestResult = await ResponseHandler.HandleUniversalResponseAndGetStatusAsync(response);

        if (!requestResult)
        {
            return LocalRedirect($"~/customer/Products/ProductsList");
        }
        
        return LocalRedirect($"~/customer/Products/ProductsList");
    }

    /// <param name="id">Is a product id; just "id" because i use general JS script.</param>
    public async Task<IActionResult> DeleteProductFromCartAsync(Guid id)
    {
        var response = await _httpClient.SendRequestAsync(
            new RestRequestForm(
                endPoint: ApiRoutes.Controllers.OrderContr + ApiRoutes.OrderActions.DeleteProductFromCartOrderAction, 
                requestMethod: HttpMethod.Delete,
                jsonData: JsonConvert.SerializeObject(new ProductCartRequest(id, CookiesHelper.GetUserId(_httpContext)))));

        var requestResult = await ResponseHandler.HandleUniversalResponseAndGetStatusAsync(response);

        if (!requestResult)
        {
            return LocalRedirect($"~/customer/Products/ProductsList");
        }
        
        return LocalRedirect($"~/customer/Products/ProductsList");
    }

    public async Task<IActionResult> CartViewAsync()
    {
        var response = await _httpClient.SendRequestAsync(
            new RestRequestForm(
                endPoint: ApiRoutes.Controllers.OrderContr + ApiRoutes.OrderActions.GetCartOrderAction, 
                requestMethod: HttpMethod.Get,
                jsonData: JsonConvert.SerializeObject(CookiesHelper.GetUserId(_httpContext))));

        var requestResult = await ResponseHandler.HandleUniversalResponseAsync<OrderCDTO>(response);

        if (!requestResult.IsSuccessful)
        {
            return View();
        }

        return View(requestResult.Result);
    }

    public async Task<IActionResult> OrdersViewAsync()
    {
        var response = await _httpClient.SendRequestAsync(
            new RestRequestForm(
                endPoint: ApiRoutes.Controllers.OrderContr + ApiRoutes.OrderActions.GetAllOrdersNotCartAction, 
                requestMethod: HttpMethod.Get,
                jsonData: JsonConvert.SerializeObject(CookiesHelper.GetUserId(_httpContext))));

        var requestResult = await ResponseHandler.HandleUniversalResponseAsync<List<OrderCDTO>>(response);

        if (!requestResult.IsSuccessful)
        {
            return View();
        }

        return View(requestResult.Result);
    }
}