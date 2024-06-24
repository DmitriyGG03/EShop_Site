using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EShop_Site.Components;
using EShop_Site.Services.Abstract;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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

    [HttpPost]
    public async Task<IActionResult> LoginAsync(LoginRequest? loginRequest = null)
    {
        if (!ModelState.IsValid) return View(loginRequest);

        var response = await _httpClient.SendRequestAsync(new RestRequestForm(
            ApiRoutes.Controllers.AuthenticationContr + ApiRoutes.AuthenticationActions.LoginAction,
            HttpMethod.Post,
            jsonData: JsonConvert.SerializeObject(loginRequest)));

        var loginResult = await Components.ResponseHandler.HandleUniversalResponseAsync<string>(response);

        if (!loginResult.IsSuccessful) return View(loginRequest);
        
        CookiesAuthorizationAsync(loginResult.Result);

        return Redirect("~/");
    }

    [HttpPost]
    public async Task<IActionResult> RegisterAsync(RegisterRequest? registerRequest = null)
    {
        if (!ModelState.IsValid) return View(registerRequest);

        var response = await _httpClient.SendRequestAsync(new RestRequestForm(
            ApiRoutes.Controllers.AuthenticationContr + ApiRoutes.AuthenticationActions.RegisterAction,
            HttpMethod.Post, jsonData: JsonConvert.SerializeObject(registerRequest)));
        
        var registerResult = await Components.ResponseHandler.HandleUniversalResponseAsync<string>(response);

        if (!registerResult.IsSuccessful) return View(registerRequest);
        
        CookiesAuthorizationAsync(registerResult.Result);

        return Redirect("~/");
    }

    private async void CookiesAuthorizationAsync(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(token) as JwtSecurityToken ??
                        throw new Exception("Invalid token!");

        var claims = new List<Claim>
        {
            new Claim("userId", jsonToken.Claims.First(claim => claim.Type == "userId").Value),
            new Claim("name", jsonToken.Claims.First(claim => claim.Type == "name").Value),
            new Claim("lastName", jsonToken.Claims.First(claim => claim.Type == "lastName").Value),
            new Claim(ClaimsIdentity.DefaultRoleClaimType, jsonToken.Claims.First(claim => claim.Type == "role").Value),
        };
        var sellerId = jsonToken.Claims.First(claim => claim.Type == "sellerId")?.Value;
        if (sellerId is not null)
        {
            claims.Add(new Claim("sellerId", sellerId));
        }
        
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