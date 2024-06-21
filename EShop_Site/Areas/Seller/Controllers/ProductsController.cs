using EShop_Site.Components;
using EShop_Site.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SharedLibrary.Models.DbModels.MainModels;
using SharedLibrary.Requests;
using SharedLibrary.Responses;
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

    public async Task<IActionResult> ProductsListAsync(string? search = null)
    {
        var response = await _httpClient.SendRequestAsync(
            new RestRequestForm(ApiRoutes.Controllers.ProductContr + ApiRoutes.ProductActions.GetAllBySellerIdPath, HttpMethod.Get,
                jsonData: JsonConvert.SerializeObject(
                    _httpContext.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "id")!.Value)));

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

        if (!String.IsNullOrEmpty(search))
        {
            products = products.Where(p => p.Name.Contains(search)).ToList();
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