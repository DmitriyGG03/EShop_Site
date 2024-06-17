using System.Security.Claims;
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
        var user = context.User;
        RoleTag userRole = RoleTag.Customer;

        if (context.User.Identity.IsAuthenticated)
        {
            var roleClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            if (roleClaim is not null)
            {
                RoleTag.TryParse(roleClaim.Value, out userRole);
            }
        }

        if (path.Equals("/") || !IsPathCorrect(userRole, path))
        {
            context.Response.Redirect($"/{userRole.ToString()}");
            return;
        }

        await _next(context);
    }

    private bool IsPathCorrect(RoleTag? userRole, string path)
    {
        foreach (RoleTag role in Enum.GetValues(typeof(RoleTag)))
        {
            if (role.Equals(userRole)) continue;

            if (ContainsSubstringAfterSlash(path, role.ToString().ToLower()))
            {
                return false;
            }
        }

        return true;
    }

    private bool ContainsSubstringAfterSlash(string input, string substringToFind)
    {
        int indexOfSlash = input.IndexOf('/');

        if (indexOfSlash == -1)
        {
            return false;
        }

        string partAfterSlash = input.Substring(indexOfSlash + 1);

        return partAfterSlash.Contains(substringToFind);
    }
}