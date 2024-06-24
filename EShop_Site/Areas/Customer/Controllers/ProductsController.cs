using EShop_Site.Components;
using EShop_Site.Extensions;
using EShop_Site.Helpers;
using EShop_Site.Models;
using EShop_Site.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SharedLibrary.Models.ClientDtoModels.MainModels;
using SharedLibrary.Routes;

namespace EShop_Site.Areas.Customer.Controllers;

[Area("Customer")]
public class ProductsController : Controller
{
    private readonly IHttpClientService _httpClient;

    public ProductsController(IHttpClientService httpClient)
    {
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
            requestResult.Result?.Select(pr => pr.ToProductForm()).ToList() ?? new();
        
        if (!String.IsNullOrEmpty(search))
        {
            products = products.Where(p => p.Name.Contains(search)).ToList();
        }

        return View(products);
    }

    /// <param name="id">Is a product id; just "id" because i use general JS script.</param>
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
}