using EShop_Site.Areas.Seller.Controllers;
using EShop_Site.Components;
using EShop_Site.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SharedLibrary.Models.DbModels.MainModels;
using SharedLibrary.Requests;
using SharedLibrary.Responses;
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

    public async Task<IActionResult> ProductsListAsync()
    {
        var response = await _httpClient.SendRequestAsync(
            new RestRequestForm(ApiRoutes.Controllers.ProductContr + ApiRoutes.ProductActions.GetAllPath, HttpMethod.Get));

        var serverResponse =
            await JsonHelper
                .GetTypeFromResponseAsync<LambdaResponse<List<Product>>>(response);

        if (!response.IsSuccessStatusCode)
        {
            MessageStorage.ErrorMessage = serverResponse.ErrorInfo ?? "";
            return View();
        }

        if (serverResponse.ResponseObject is null) throw new Exception("Response object is null");

        List<EditProductRequest> products = new();
        if (serverResponse.ResponseObject is not null && serverResponse.ResponseObject.Count > 0)
        {
            foreach (var product in serverResponse.ResponseObject)
            {
                products.Add(new EditProductRequest()
                {
                    ProductId = product.ProductId,

                    Name = product.Name,
                    WeightInGrams = product.WeightInGrams,
                    PricePerUnit = product.PricePerUnit,

                    Description = product.Description,
                    ImageUrl = product.ImageUrl,
                    InStock = product.InStock,
                    
                    SellerId = product.SellerId,
                });
            }
        }
        
        TempData["EditProductRequest"] = JsonConvert.SerializeObject(products);

        return View(products);
    }

    public async Task<IActionResult> ProductView(Guid id)
    {
        var sellerDataJson = TempData["EditProductRequest"] as string;
        if (sellerDataJson is null) throw new Exception("Edit data not found");

        var models = JsonConvert.DeserializeObject<List<EditProductRequest>>(sellerDataJson);
        var model = models.FirstOrDefault(m => m.ProductId.Equals(id));

        return View(model);
    }

    public async Task<IActionResult> DeleteProduct(string? id = null)
    {
        if (id is null) throw new Exception("Id is null");
        Guid receivedId = new Guid(id);

        var response = await _httpClient.SendRequestAsync(
            new RestRequestForm(ApiRoutes.Controllers.ProductContr + ApiRoutes.ProductActions.DeletePath, HttpMethod.Delete,
                jsonData: JsonConvert.SerializeObject(receivedId)));

        var serverResponse =
            await JsonHelper
                .GetTypeFromResponseAsync<LambdaResponse>(response);

        if (!response.IsSuccessStatusCode)
        {
            MessageStorage.ErrorMessage = serverResponse.ErrorInfo ?? "";
            return RedirectToAction("ProductsList");
        }

        MessageStorage.InfoMessage = serverResponse.Info ?? "";
        return RedirectToAction("ProductsList");
    }
}