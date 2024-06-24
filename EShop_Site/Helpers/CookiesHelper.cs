using SharedLibrary.Models.Enums;

namespace EShop_Site.Helpers;

public static class CookiesHelper
{
    public static Guid GetUserId(IHttpContextAccessor httpContextAccessor)
    {
        var context = httpContextAccessor.HttpContext ??
                      throw new Exception("HttpContext is null, check user authorization!");

        return GetUserId(context);
    }
    public static Guid GetUserId(HttpContext httpContext)
    {
        return new Guid(httpContext.User.Claims.FirstOrDefault(c => c.Type == "id")!.Value);
    }

    public static RoleTag GetUserRoleTag(IHttpContextAccessor httpContextAccessor)
    {
        var context = httpContextAccessor.HttpContext ??
                      throw new Exception("HttpContext is null, check user authorization!");

        return GetUserRoleTag(context);
    }
    public static RoleTag GetUserRoleTag(HttpContext httpContext)
    {
        var isParseSuccessful = RoleTag.TryParse(httpContext.User.Claims.FirstOrDefault(c => c.Type == "role")!.Value,
            out RoleTag result);

        if (!isParseSuccessful)
        {
            throw new Exception("Can`t parse the role of cookies to enum type");
        }

        return result;
    }
    
    public static Guid? GetUserSellerId(IHttpContextAccessor httpContextAccessor)
    {
        var context = httpContextAccessor.HttpContext ??
                      throw new Exception("HttpContext is null, check user authorization!");

        return GetUserSellerId(context);
    }
    public static Guid? GetUserSellerId(HttpContext httpContext)
    {
        var isParseSuccessful = Guid.TryParse(httpContext.User.Claims.FirstOrDefault(c => c.Type == "sellerId")?.Value,
            out Guid result);

        if (!isParseSuccessful)
        {
            return null;
        }

        return result;
    }
}