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
    private readonly IHttpContextAccessor _httpContextAccessor;

    public OrderController(IHttpClientService httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _httpClient = httpClient;
    }

    /// <param name="id">Is a product id; just "id" because i use general JS script.</param>
    public async Task<IActionResult> AddProductToCartAsync(Guid id) 
    {
        var response = await _httpClient.SendRequestAsync(
            new RestRequestForm(
                endPoint: ApiRoutes.Controllers.OrderContr + ApiRoutes.OrderActions.AddProductToCartOrderAction, 
                requestMethod: HttpMethod.Post,
                jsonData: JsonConvert.SerializeObject(new ProductCartRequest(id, CookiesHelper.GetUserId(_httpContextAccessor.HttpContext)))));

        var requestResult = await ResponseHandler.HandleUniversalResponseAndGetStatusAsync(response);

        if (!requestResult)
        {
            return LocalRedirect($"~/customer/Products/ProductsListView");
        }
        
        return LocalRedirect($"~/customer/Products/ProductsListView");
    }
    
    public async Task<IActionResult> CreateOrderAsync() 
    {
        var response = await _httpClient.SendRequestAsync(
            new RestRequestForm(
                endPoint: ApiRoutes.Controllers.OrderContr + ApiRoutes.OrderActions.CreateOrder, 
                requestMethod: HttpMethod.Post,
                jsonData: JsonConvert.SerializeObject(CookiesHelper.GetUserId(_httpContextAccessor.HttpContext))));

        var requestResult = await ResponseHandler.HandleUniversalResponseAndGetStatusAsync(response);

        if (!requestResult)
        {
            return LocalRedirect($"~/customer/Order/OrdersView");
        }
        
        return LocalRedirect($"~/customer/Order/OrdersView");
    }

    /// <param name="id">Is a product id; just "id" because i use general JS script.</param>
    public async Task<IActionResult> DeleteProductFromCartAsync(string? id = null)
    {
        if (id is null) throw new Exception("Id is null");
        Guid receivedId = new Guid(id);
        
        var response = await _httpClient.SendRequestAsync(
            new RestRequestForm(
                endPoint: ApiRoutes.Controllers.OrderContr + ApiRoutes.OrderActions.DeleteProductFromCartOrderAction, 
                requestMethod: HttpMethod.Delete,
                jsonData: JsonConvert.SerializeObject(new ProductCartRequest(receivedId, CookiesHelper.GetUserId(_httpContextAccessor.HttpContext)))));

        var requestResult = await ResponseHandler.HandleUniversalResponseAndGetStatusAsync(response);

        if (!requestResult)
        {
            return RedirectToAction("CartView");
        }
        
        return RedirectToAction("CartView");
    }

    public async Task<IActionResult> CartViewAsync()
    {
        var response = await _httpClient.SendRequestAsync(
            new RestRequestForm(
                endPoint: ApiRoutes.Controllers.OrderContr + ApiRoutes.OrderActions.GetCartOrderAction, 
                requestMethod: HttpMethod.Get,
                jsonData: JsonConvert.SerializeObject(CookiesHelper.GetUserId(_httpContextAccessor.HttpContext))));

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
                jsonData: JsonConvert.SerializeObject(CookiesHelper.GetUserId(_httpContextAccessor.HttpContext))));

        var requestResult = await ResponseHandler.HandleUniversalResponseAsync<List<OrderCDTO>>(response);

        if (!requestResult.IsSuccessful)
        {
            return View();
        }

        return View(requestResult.Result);
    }
}