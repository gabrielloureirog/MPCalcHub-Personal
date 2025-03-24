using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MPCalcHub.Domain.Entities;
using MPCalcHub.Domain.Interfaces.Security;
using MPCalcHub.Domain.Settings;

namespace MPCalcHub.Domain.Services.Security;

public class TokenService(IOptions<TokenSettings> options, IMemoryCache cache) : ITokenService
{
    private readonly TokenSettings _settings = options.Value;
    private readonly IMemoryCache _cache = cache;

    public string GenerateToken(User user, bool force = false)
    {
        if (_cache.TryGetValue(user.Id, out string token) && force == false)
            return token;
        else
            _cache.Remove(user.Id);

        token = CreateToken(user);

        var cacheOptions = new MemoryCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(_settings.ExpirationTimeHour),
            SlidingExpiration = TimeSpan.FromMinutes(_settings.IncreaseExpirationTimeMinutes)
        };

        _cache.Set(user.Id, token, cacheOptions);

        return token;
    }

    private string CreateToken(User user)
    {
        var jwtKey = _settings.Key;
        if (string.IsNullOrEmpty(jwtKey))
            throw new Exception("JWT Key is not configured.");

        var key = Convert.FromBase64String(jwtKey);
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, ((int)user.PermissionLevel).ToString())
            }),
            Expires = DateTime.UtcNow.AddHours(_settings.ExpirationTimeHour),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}