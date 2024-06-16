using SharedLibrary.Models.Enums;

namespace EShop_Site.Middlewares;

public class RedirectionMiddleware
{
    private readonly RequestDelegate _next;

    public RedirectionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var path = context.Request.Path.Value;

        if (context.User.Identity.IsAuthenticated)
        {
            var user = context.User;

            if (user.IsInRole(RoleTag.Manager.ToString()))
            {
                if (path.StartsWith("/seller", StringComparison.OrdinalIgnoreCase) ||
                    path.StartsWith("/customer", StringComparison.OrdinalIgnoreCase) ||
                    path.Equals("/"))
                {
                    context.Response.Redirect("/manage");
                    return;
                }
            }

            if (user.IsInRole(RoleTag.Seller.ToString()))
            {
                if (path.StartsWith("/manage", StringComparison.OrdinalIgnoreCase) ||
                    path.StartsWith("/customer", StringComparison.OrdinalIgnoreCase) ||
                    path.Equals("/"))
                {
                    context.Response.Redirect("/seller");
                    return;
                }
            }
        }
        else if ((context.User.Identity.IsAuthenticated && context.User.IsInRole(RoleTag.Customer.ToString())) ||
                 !context.User.Identity.IsAuthenticated)
        {
            if (path.StartsWith("/manage", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/seller", StringComparison.OrdinalIgnoreCase) ||
                path.Equals("/"))
            {
                context.Response.Redirect("/customer");
                return;
            }
        }

        await _next(context);
    }
}