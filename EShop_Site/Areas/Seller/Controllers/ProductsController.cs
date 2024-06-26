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

namespace EShop_Site.Areas.Seller.Controllers;

[Authorize(Policy = Policy.OnlyForSellers)]
[Area("Seller")]
public class ProductsController : Controller
{
    private readonly IHttpClientService _httpClient;
    private readonly IHttpContextAccessor _httpContext;

    public ProductsController(IHttpClientService httpClient, IHttpContextAccessor httpContext)
    {
        _httpContext = httpContext;
        _httpClient = httpClient;
    }

    public async Task<IActionResult> ProductsListViewAsync(string? search = null)
    {
        var response = await _httpClient.SendRequestAsync(
            new RestRequestForm(
                endPoint: ApiRoutes.Controllers.ProductContr + ApiRoutes.UniversalActions.GetAllAction,
                requestMethod: HttpMethod.Get));

        var requestResult = await ResponseHandler.HandleUniversalResponseAsync<List<ProductCDTO>>(response);

        if (!requestResult.IsSuccessful)
        {
            return View();
        }

        List<ProductForm> products =
            requestResult.Result
                ?.Where(p => p.SellerCDtoId.Equals(CookiesHelper.GetUserSellerId(_httpContext.HttpContext)))
                .Select(pr => pr.ToProductForm()).ToList() ?? new();

        if (!String.IsNullOrEmpty(search))
        {
            products = products.Where(p => p.Name.Contains(search)).ToList();
        }

        return View(products);
    }

    public async Task<IActionResult> ProductView(Guid id)
    {
        var response = await _httpClient.SendRequestAsync(
            new RestRequestForm(
                endPoint: ApiRoutes.Controllers.ProductContr + ApiRoutes.UniversalActions.GetByIdAction,
                requestMethod: HttpMethod.Get,
                jsonData: JsonConvert.SerializeObject(id)));

        var requestResult = await ResponseHandler.HandleUniversalResponseAsync<ProductCDTO>(response);

        if (!requestResult.IsSuccessful)
        {
            return View();
        }

        return View(requestResult.Result!.ToProductForm());
    }

    public async Task<IActionResult> DeleteProductAsync(string? id = null)
    {
        if (id is null) throw new Exception("Id is null");
        Guid receivedId = new Guid(id);

        var response = await _httpClient.SendRequestAsync(
            new RestRequestForm(
                endPoint: ApiRoutes.Controllers.ProductContr + ApiRoutes.UniversalActions.DeleteAction,
                requestMethod: HttpMethod.Delete,
                jsonData: JsonConvert.SerializeObject(receivedId)));

        var requestResult = await ResponseHandler.HandleUniversalResponseAndGetStatusAsync(response);

        return RedirectToAction("ProductsListView");
    }
}