using System.IdentityModel.Tokens.Jwt;
using BeeApps.Common.Enumerations;
using BeeApps.Common.Models;
using BeeApps.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;

namespace BeeApps.Common.Filters;

public class ValidTokenFilter : ActionFilterAttribute
{
    public PermissionName[] PermissionsAnd { get; set; }
    public PermissionName[] PermissionsOr { get; set; }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var authService = context.HttpContext.RequestServices.GetService<IAuthService>();
        var authorizationHeader = context.HttpContext.Request.Headers[HeaderNames.Authorization];

        // If Authorization field is not found in headers
        if (authorizationHeader.Count == 0)
        {
            context.Result = new UnauthorizedResult();
        }
        // If Authorization does not specify a "Bearer" value
        else if (!authorizationHeader[0].StartsWith("Bearer "))
        {
            context.Result = new UnauthorizedResult();
        }
        else
        {
            // Extract token from header
            var token = authorizationHeader[0].Replace("Bearer ", "");

            // If token is invalidated by auth service
            if (!await authService.Authorize(token))
            {
                context.Result = new UnauthorizedResult();
            }
            else
            {
                if (PermissionsAnd != null || PermissionsOr != null)
                    try
                    {
                        // Extract permissions from token
                        var tokenHandler = new JwtSecurityTokenHandler();
                        var securityToken = tokenHandler.ReadJwtToken(token);
                        var permissionsClaim = securityToken.Claims.First(c => c.Type == "p").Value;
                        var tokenPermissions = Permission.FromShortInline(permissionsClaim);

                        // Check permissions
                        var authorizedAnd = checkPermissionsAnd(tokenPermissions);
                        var authorizedOr = checkPermissionsOr(tokenPermissions);

                        if (!authorizedAnd || !authorizedOr)
                            context.Result = new StatusCodeResult(403);
                    }
                    catch
                    {
                        context.Result = new StatusCodeResult(403);
                    }
            }
        }

        if (context.Result != null)
            await Task.CompletedTask;
        else
            await next();
    }

    private bool checkPermissionsAnd(IEnumerable<PermissionName> tokenPermissions)
    {
        if (PermissionsAnd == null || PermissionsAnd.Length == 0)
            return true;

        var authorized = true;
        foreach (var p in PermissionsAnd)
            if (!tokenPermissions.Contains(p))
            {
                authorized = false;
                break;
            }

        return authorized;
    }

    private bool checkPermissionsOr(IEnumerable<PermissionName> tokenPermissions)
    {
        if (PermissionsOr == null || PermissionsOr.Length == 0)
            return true;

        var authorized = false;
        foreach (var p in PermissionsOr)
            if (tokenPermissions.Contains(p))
            {
                authorized = true;
                break;
            }

        return authorized;
    }
}