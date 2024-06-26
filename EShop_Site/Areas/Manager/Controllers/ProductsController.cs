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

namespace EShop_Site.Areas.Manager.Controllers;

[Authorize(Policy = Policy.OnlyForManagers)]
[Area("Manager")]
public class ProductsController : Controller
{
    private readonly ILogger<ManagerHomeController> _logger;
    private readonly IHttpClientService _httpClient;
    
    public ProductsController(ILogger<ManagerHomeController> logger, IHttpClientService httpClient)
    {
        _httpClient = httpClient;
        _logger = logger;
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