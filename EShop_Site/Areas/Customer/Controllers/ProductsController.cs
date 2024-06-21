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
public class ProductsController : Controller
{
    private readonly IHttpClientService _httpClient;
    
    public ProductsController(IHttpClientService httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IActionResult> ProductsListAsync(string? search = null)
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
    
}