using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EShop_Site.Components;
using EShop_Site.Services.Abstract;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SharedLibrary.Models.Enums;
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
    
    [HttpGet]
    public async Task<IActionResult> LoginAsync()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> LoginAsync(LoginRequest loginRequest)
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

    [HttpGet]
    public async Task<IActionResult> RegisterAsync()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> RegisterAsync(RegisterRequest registerRequest)
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

    [HttpPost]
    private async void CookiesAuthorizationAsync(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(token) as JwtSecurityToken ??
                        throw new Exception("Invalid token!");

        var claims = new List<Claim>
        {
            new Claim(Claims.UserId.ToString(), jsonToken.Claims.First(claim => claim.Type == Claims.UserId.ToString()).Value),
            new Claim(Claims.Name.ToString(), jsonToken.Claims.First(claim => claim.Type == Claims.Name.ToString()).Value),
            new Claim(Claims.LastName.ToString(), jsonToken.Claims.First(claim => claim.Type == Claims.LastName.ToString()).Value),
            new Claim(ClaimsIdentity.DefaultRoleClaimType, jsonToken.Claims.First(claim => claim.Type == Claims.Role.ToString()).Value),
        };
        var sellerId = jsonToken.Claims.FirstOrDefault(claim => claim.Type == Claims.SellerId.ToString())?.Value;
        if (sellerId is not null)
        {
            claims.Add(new Claim(Claims.SellerId.ToString(), sellerId));
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