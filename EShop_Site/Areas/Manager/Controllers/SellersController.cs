using EShop_Site.Components;
using EShop_Site.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SharedLibrary.Requests;
using SharedLibrary.Responses;
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

    public async Task<IActionResult> SellersListAsync()
    {
        var response = await _httpClient.SendRequestAsync(
            new RestRequestForm(ApiRoutes.Controllers.SellerContr + ApiRoutes.SellerActions.GetAllPath, HttpMethod.Get));

        var serverResponse =
            await JsonHelper
                .GetTypeFromResponseAsync<LambdaResponse<List<SharedLibrary.Models.DbModels.MainModels.Seller>>>(response);

        if (!response.IsSuccessStatusCode)
        {
            MessageStorage.ErrorMessage = serverResponse.ErrorInfo ?? "";
            return View();
        }

        if (serverResponse.ResponseObject is null) throw new Exception("Response object is null");

        List<EditSellerRequest> sellers = new();
        if (serverResponse.ResponseObject is not null && serverResponse.ResponseObject.Count > 0)
        {
            foreach (var seller in serverResponse.ResponseObject)
            {
                sellers.Add(new EditSellerRequest()
                {
                    SellerId = seller.SellerId,

                    CompanyName = seller.CompanyName,
                    ContactNumber = seller.ContactNumber,
                    EmailAddress = seller.EmailAddress,

                    CompanyDescription = seller.CompanyDescription,
                    ImageUrl = seller.ImageUrl,
                    AdditionNumber = seller.AdditionNumber,
                });
            }
        }
        
        TempData["EditSellerRequest"] = JsonConvert.SerializeObject(sellers);

        return View(sellers);
    }

    public async Task<IActionResult> SellerView(Guid id)
    {
        var sellerDataJson = TempData["EditSellerRequest"] as string;
        if (sellerDataJson is null) throw new Exception("Edit data not found");

        var models = JsonConvert.DeserializeObject<List<EditSellerRequest>>(sellerDataJson);
        var model = models.FirstOrDefault(m => m.SellerId.Equals(id));

        return View(model);
    }

    public async Task<IActionResult> DeleteSeller(string? id = null)
    {
        if (id is null) throw new Exception("Id is null");
        Guid receivedId = new Guid(id);

        var response = await _httpClient.SendRequestAsync(
            new RestRequestForm(ApiRoutes.Controllers.SellerContr + ApiRoutes.SellerActions.DeletePath, HttpMethod.Delete,
                jsonData: JsonConvert.SerializeObject(receivedId)));

        var serverResponse =
            await JsonHelper
                .GetTypeFromResponseAsync<LambdaResponse>(response);

        if (!response.IsSuccessStatusCode)
        {
            MessageStorage.ErrorMessage = serverResponse.ErrorInfo ?? "";
            return RedirectToAction("SellersList");
        }

        MessageStorage.InfoMessage = serverResponse.Info ?? "";
        return RedirectToAction("SellersList");
    }
}