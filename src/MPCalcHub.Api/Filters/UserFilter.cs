using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using MPCalcHub.Application.Interfaces;
using MPCalcHub.Domain.Entities;
using MPCalcHub.Domain.Settings;
using MPCalcHub.Infrastructure.Helper;

namespace MPCalcHub.Api.Filters;

public class UserFilter(UserData userData, ITokenApplicationService tokenApplicationService, IOptions<TokenSettings> options) : IAuthorizationFilter
{
    private readonly UserData _userData = userData;
    private readonly ITokenApplicationService _tokenApplicationService = tokenApplicationService;
    private readonly TokenSettings _settings = options.Value;

    public async void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context.Filters.Any(f => f is SkipUserFilterAttribute))
            return;
        
        var user = context.HttpContext.User;
        var userId = user?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var token = context.HttpContext.Request.Headers.Authorization.FirstOrDefault();
        if (token != null)
        {
            _userData.Set(TokenHelper.GetUserData(token, _settings.Key));
            var timeUntilExpiration = TokenHelper.GetTimeUntilExpiration(token, _settings.Key);

            if (timeUntilExpiration.HasValue && timeUntilExpiration.Value.TotalMinutes <= 5)
            {
                token = await _tokenApplicationService.GetTokenByAutorization(_userData.Email);
                context.HttpContext.Response.Cookies.Append("AuthToken", token);
            }

            if (context.HttpContext.User?.Claims?.Count() <= 0)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
        }
        else
        {
            context.Result = new UnauthorizedResult();
            return;
        }
    }
}
