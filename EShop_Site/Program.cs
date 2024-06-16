using System.Security.Claims;
using EShop_Site.Middlewares;
using EShop_Site.Services;
using EShop_Site.Services.Abstract;
using Microsoft.AspNetCore.Authentication.Cookies;
using SharedLibrary.Models.Enums;
using SharedLibrary.Routes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("TradeWaveApiClient");
builder.Services.AddScoped<IHttpClientService, HttpClientService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/authentication/login";
    });

builder.Services.AddAuthorization(opts => {
 
    opts.AddPolicy(Policy.OnlyForManagers, policy => {
        policy.RequireRole(RoleTag.Manager.ToString());
    });
    opts.AddPolicy(Policy.OnlyForSellers, policy => {
        policy.RequireRole(RoleTag.Seller.ToString());
    });
});

// Add services to the container.
builder.Services.AddControllersWithViews().AddNewtonsoftJson(i => { });

builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<RedirectionMiddleware>();

app.MapAreaControllerRoute(
    name: "manager_area",
    areaName: "manager",
    pattern: "manage/{controller=ManagerHome}/{action=Home}/{id?}");

app.MapAreaControllerRoute(
    name: "customer_area",
    areaName: "seller",
    pattern: "seller/{controller=SellerHome}/{action=Home}/{id?}");

app.MapAreaControllerRoute(
    name: "customer_area",
    areaName: "customer",
    pattern: "customer/{controller=CustomerHome}/{action=Home}/{id?}");

app.MapControllerRoute(
    name: "default_area",
    pattern: "{controller=Home}/{action=Home}/{id?}");

app.MapRazorPages();

app.Run();