using EShop_Site.Components;
using EShop_Site.Helpers;
using EShop_Site.Models;
using EShop_Site.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SharedLibrary.Routes;

namespace EShop_Site.Controllers;

[Authorize]
public class EditFormsController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHttpClientService _httpClient;

    private static string? _returnUrl;

    public EditFormsController(ILogger<HomeController> logger, IHttpClientService httpClientService,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClientService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUserAsync(string returnUrl)
    {
        SetReturnUrl(returnUrl);
        return RedirectToAction("EditUser", new { userForm = new UserForm(), returnUrl });
    }

    [HttpPost]
    public async Task<IActionResult> CreateSellerAsync(string returnUrl)
    {
        SetReturnUrl(returnUrl);
        return RedirectToAction("EditSeller", new { sellerForm = new SellerForm(), returnUrl });
    }

    [HttpPost]
    public async Task<IActionResult> CreateProductAsync(string returnUrl)
    {
        SetReturnUrl(returnUrl);
        return RedirectToAction("EditProduct", new { productForm = new ProductForm(), returnUrl });
    }

    [HttpPost]
    public async Task<IActionResult> EditUserAsync(UserForm userForm, string? returnUrl = null)
    {
        SetReturnUrl(returnUrl);
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Incorrect form filling");
            return View(userForm);
        }

        HttpResponseMessage response;

        if (userForm.UserId.Equals(Guid.Empty))
        {
            response = await _httpClient.SendRequestAsync(new RestRequestForm(
                endPoint: ApiRoutes.Controllers.UserContr + ApiRoutes.UniversalActions.CreateAction,
                requestMethod: HttpMethod.Put,
                jsonData: JsonConvert.SerializeObject(userForm)));
        }
        else
        {
            response = await _httpClient.SendRequestAsync(new RestRequestForm(
                endPoint: ApiRoutes.Controllers.UserContr + ApiRoutes.UniversalActions.EditAction,
                requestMethod: HttpMethod.Put,
                jsonData: JsonConvert.SerializeObject(userForm)));
        }

        if (!await ResponseHandler.HandleUniversalResponseAndGetStatusAsync(response))
        {
            _logger.LogError("Failed user creating/editing");
            return View(userForm);
        }
        else
        {
            _logger.LogInformation("Successful user creating/editing");
            return Redirect(_returnUrl ?? throw new Exception("Return URL is null"));
        }
    }

    [HttpPost]
    public async Task<IActionResult> EditSellerAsync(SellerForm sellerForm, string? returnUrl = null)
    {
        SetReturnUrl(returnUrl);
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Incorrect form filling");
            return View(sellerForm);
        }

        HttpResponseMessage response;

        if (sellerForm.SellerId.Equals(Guid.Empty))
        {
            response = await _httpClient.SendRequestAsync(new RestRequestForm(
                endPoint: ApiRoutes.Controllers.SellerContr + ApiRoutes.UniversalActions.CreateAction,
                requestMethod: HttpMethod.Put,
                jsonData: JsonConvert.SerializeObject(sellerForm)));
        }
        else
        {
            response = await _httpClient.SendRequestAsync(new RestRequestForm(
                endPoint: ApiRoutes.Controllers.SellerContr + ApiRoutes.UniversalActions.EditAction,
                requestMethod: HttpMethod.Put,
                jsonData: JsonConvert.SerializeObject(sellerForm)));
        }

        if (!await ResponseHandler.HandleUniversalResponseAndGetStatusAsync(response))
        {
            _logger.LogError("Failed seller creating/editing");
            return View(sellerForm);
        }
        else
        {
            _logger.LogInformation("Successful seller creating/editing");
            return Redirect(_returnUrl ?? throw new Exception("Return URL is null"));
        }
    }

    [HttpPost]
    public async Task<IActionResult> EditProductAsync(ProductForm productForm, string? returnUrl = null)
    {
        SetReturnUrl(returnUrl);
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Incorrect form filling");
            return View(productForm);
        }

        HttpResponseMessage response;

        if (productForm.SellerId.Equals(Guid.Empty))
        {
            productForm.SellerId = CookiesHelper.GetUserSellerId(_httpContextAccessor) ?? OwnSellerData.Id;
            response = await _httpClient.SendRequestAsync(new RestRequestForm(
                endPoint: ApiRoutes.Controllers.ProductContr + ApiRoutes.UniversalActions.CreateAction,
                requestMethod: HttpMethod.Put,
                jsonData: JsonConvert.SerializeObject(productForm)));
        }
        else
        {
            response = await _httpClient.SendRequestAsync(new RestRequestForm(
                endPoint: ApiRoutes.Controllers.ProductContr + ApiRoutes.UniversalActions.EditAction,
                requestMethod: HttpMethod.Put,
                jsonData: JsonConvert.SerializeObject(productForm)));
        }

        if (!await ResponseHandler.HandleUniversalResponseAndGetStatusAsync(response))
        {
            _logger.LogError("Failed product creating/editing");
            return View(productForm);
        }
        else
        {
            _logger.LogInformation("Successful product creating/editing");
            return Redirect(_returnUrl ?? throw new Exception("Return URL is null"));
        }
    }

    private void SetReturnUrl(string? returnUrl)
    {
        if (returnUrl is not null) _returnUrl = returnUrl;
    }
}