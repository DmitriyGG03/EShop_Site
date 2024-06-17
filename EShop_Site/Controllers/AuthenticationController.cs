using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EShop_Site.Components;
using EShop_Site.Helpers;
using EShop_Site.Models;
using EShop_Site.Services.Abstract;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SharedLibrary.Models.MainModels;
using SharedLibrary.Requests;
using SharedLibrary.Responses;
using SharedLibrary.Routes;

namespace EShop_Site.Controllers;

public class AuthenticationController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IHttpClientService _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthenticationController(ILogger<HomeController> logger, IHttpClientService httpClientService,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _httpClient = httpClientService;
    }

    public async Task<IActionResult> LoginAsync()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> LoginAsync(LoginRequest loginRequest)
    {
        if (!ModelState.IsValid) return View();

        var response = await _httpClient.SendRequestAsync(new RestRequestForm(
            ApiRoutes.Controllers.Authentication + ApiRoutes.Authentication.Login,
            HttpMethod.Post,
            jsonData: JsonConvert.SerializeObject(loginRequest)));

        return await ResponseHandler(response);
    }

    public async Task<IActionResult> RegisterAsync()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> RegisterAsync(RegisterRequest registerRequest)
    {
        if (!ModelState.IsValid) return View();

        var response = await _httpClient.SendRequestAsync(new RestRequestForm(
            ApiRoutes.Controllers.Authentication + ApiRoutes.Authentication.Register,
            HttpMethod.Post, jsonData: JsonConvert.SerializeObject(registerRequest)));

        return await ResponseHandler(response);
    }

    private async Task<IActionResult> ResponseHandler(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var testAuthResponse = await JsonHelper.GetTypeFromResponseAsync<LambdaResponse>(response);

            ModelState.AddModelError(string.Empty, testAuthResponse.Info
                                                   ?? throw new Exception("Info is null"));

            return View();
        }

        var authResponse = await JsonHelper.GetTypeFromResponseAsync<AuthenticationResponse>(response);

        var userId = User.FindFirst("id")?.Value;
        var name = User.FindFirst("name")?.Value;
        var lastName = User.FindFirst("lastName")?.Value;
        var email = User.FindFirst("email")?.Value;
        var phoneNumber = User.FindFirst("phoneNumber")?.Value;
        var role = User.FindFirst("role")?.Value;

        AddUserCookiesAsync(authResponse);

        return RedirectToAction("", "", new { area = "" });
    }

    private async void AddUserCookiesAsync(AuthenticationResponse authResponse)
    {
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(authResponse.Token) as JwtSecurityToken ??
                        throw new Exception("Invalid token!");

        var claims = new List<Claim>
        {
            new Claim("id", jsonToken.Claims.First(claim => claim.Type == "id").Value),
            new Claim("name", jsonToken.Claims.First(claim => claim.Type == "name").Value),
            new Claim("lastName", jsonToken.Claims.First(claim => claim.Type == "lastName").Value),
            new Claim(ClaimsIdentity.DefaultNameClaimType, jsonToken.Claims.First(claim => claim.Type == "email").Value),
            new Claim("phoneNumber", jsonToken.Claims.First(claim => claim.Type == "phoneNumber").Value),
            new Claim(ClaimsIdentity.DefaultRoleClaimType, jsonToken.Claims.First(claim => claim.Type == "role").Value),
        };
        var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        
        await _httpContextAccessor.HttpContext.SignInAsync(claimsPrincipal);
    }
    
    [Authorize]
    public async Task<IActionResult> Logout(string returnUrl = null)
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        returnUrl ??= Url.Content("~/");

        return LocalRedirect(returnUrl);
    }
}