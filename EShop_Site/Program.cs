using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using EShop_Site.Middlewares;
using EShop_Site.Services;
using EShop_Site.Services.Abstract;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using SharedLibrary.Models.Enums;
using SharedLibrary.Routes;

var builder = WebApplication.CreateBuilder(args);

// Настройка Kestrel для поддержки mTLS
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ConfigureHttpsDefaults(httpsOptions =>
    {
        httpsOptions.ServerCertificate = new X509Certificate2("path/to/server.pfx", "password");

        httpsOptions.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
        httpsOptions.ClientCertificateValidation = (cert, chain, errors) =>
        {
            // Добавьте вашу логику проверки сертификата клиента здесь
            // Например, проверка цепочки сертификатов с использованием корневого сертификата (CA)
            return chain.Build(cert);
        };
    });
});

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
    pattern: "manager/{controller=ManagerHome}/{action=Home}/{id?}");

app.MapAreaControllerRoute(
    name: "seller_area",
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