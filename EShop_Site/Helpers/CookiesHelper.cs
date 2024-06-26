using System.Security.Claims;
using SharedLibrary.Models.Enums;

namespace EShop_Site.Helpers;

public static class CookiesHelper
{
    public static Guid GetUserId(HttpContext httpContext)
    {
        return new Guid(httpContext.User.Claims.FirstOrDefault(c => c.Type == Claims.UserId.ToString())!.Value);
    }
    
    public static string GetUserName(HttpContext httpContext)
    {
        return httpContext.User.Claims.FirstOrDefault(c => c.Type == Claims.Name.ToString())?.Value 
               ?? throw new Exception("Name was not found in cookies!");
    }
    
    public static string GetUserLastName(HttpContext httpContext)
    {
        return httpContext.User.Claims.FirstOrDefault(c => c.Type == Claims.LastName.ToString())?.Value 
               ?? throw new Exception("Lastname was not found in cookies!");
    }
    
    public static RoleTag GetUserRoleTag(HttpContext httpContext)
    {
        var roleString = httpContext.User.Claims.FirstOrDefault(c => c.Type.Equals(ClaimsIdentity.DefaultRoleClaimType))!.Value;
        
        var isParseSuccessful = RoleTag.TryParse(roleString, out RoleTag result);

        if (!isParseSuccessful)
        {
            throw new Exception("Can`t parse the role of cookies to enum type");
        }

        return result;
    }
    
    public static Guid? GetUserSellerId(HttpContext httpContext)
    {
        var isParseSuccessful = Guid.TryParse(httpContext.User.Claims.FirstOrDefault(c => c.Type == Claims.SellerId.ToString())?.Value,
            out Guid result);

        if (!isParseSuccessful)
        {
            return null;
        }

        return result;
    }
}