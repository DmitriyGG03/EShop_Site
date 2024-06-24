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
public class SellersController : Controller
{
    private readonly ILogger<ManagerHomeController> _logger;
    private readonly IHttpClientService _httpClient;

    public SellersController(ILogger<ManagerHomeController> logger, IHttpClientService httpClient)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IActionResult> SellersListViewAsync()
    {
        var response = await _httpClient.SendRequestAsync(
            new RestRequestForm(
                endPoint: ApiRoutes.Controllers.SellerContr + ApiRoutes.UniversalActions.GetAllAction, 
                requestMethod: HttpMethod.Get));

        var requestResult = await ResponseHandler.HandleUniversalResponseAsync<List<SellerCDTO>>(response);

        if (!requestResult.IsSuccessful)
        {
            return View();
        }

        List<SellerForm> sellers = 
            requestResult.Result?.Select(pr => pr.ToSellerForm()).ToList() ?? new();

        return View(sellers);
    }

    public async Task<IActionResult> SellerViewAsync(Guid id)
    {
        var response = await _httpClient.SendRequestAsync(
            new RestRequestForm(
                endPoint: ApiRoutes.Controllers.SellerContr + ApiRoutes.UniversalActions.GetByIdAction, 
                requestMethod: HttpMethod.Get,
                jsonData: JsonConvert.SerializeObject(id)));

        var requestResult = await ResponseHandler.HandleUniversalResponseAsync<SellerCDTO>(response);

        if (!requestResult.IsSuccessful)
        {
            return View();
        }

        return View(requestResult.Result!.ToSellerForm());
    }

    public async Task<IActionResult> DeleteSellerAsync(string? id = null)
    {
        if (id is null) throw new Exception("Id is null");
        Guid receivedId = new Guid(id);

        var response = await _httpClient.SendRequestAsync(
            new RestRequestForm(
                endPoint: ApiRoutes.Controllers.SellerContr + ApiRoutes.UniversalActions.DeleteAction, 
                requestMethod: HttpMethod.Delete,
                jsonData: JsonConvert.SerializeObject(receivedId)));

        var requestResult = await ResponseHandler.HandleUniversalResponseAndGetStatusAsync(response);

        return RedirectToAction("SellersListView");
    }
}