using EShop_Site.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("MyApiClient");
builder.Services.AddTransient<HttpClientService>();

// Add services to the container.
builder.Services.AddControllersWithViews().AddNewtonsoftJson(i =>
{
    
});

builder.Services.AddRazorPages();

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

app.UseAuthorization();

app.MapAreaControllerRoute(
    name: "manager_area",
    areaName:"manager",
    pattern: "manage/{controller=ManagerHome}/{action=Index}/{id?}");

// app.MapAreaControllerRoute(
//     name: "customer_area",
//     areaName: "customer",
//     pattern: "buy/{controller=ManagerHome}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Privacy}/{id?}");

app.MapRazorPages();

app.Run();