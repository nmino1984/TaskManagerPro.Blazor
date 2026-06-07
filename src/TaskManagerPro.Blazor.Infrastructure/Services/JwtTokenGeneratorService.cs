using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TaskManagerPro.Blazor.Application.Common.Interfaces;

namespace TaskManagerPro.Blazor.Infrastructure.Services;

/// <summary>
/// Generates HMAC-SHA256-signed JWTs. The jti claim (unique token ID) allows individual
/// tokens to be revoked via a blacklist in the future without changing the signing approach.
/// </summary>
public class JwtTokenGeneratorService : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration;

    public JwtTokenGeneratorService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // Reads Jwt:Key, Jwt:Issuer, Jwt:Audience, and Jwt:ExpirationMinutes from configuration
    public (string Token, DateTime ExpiresAt) GenerateToken(
        Guid userId, string email, string firstName, string lastName, bool isEmailVerified, string? avatarUrl)
    {
        var key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is not configured.");
        var issuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer is not configured.");
        var audience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience is not configured.");

        int expirationMinutes = int.TryParse(_configuration["Jwt:ExpirationMinutes"], out int parsed) ? parsed : 60;

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        Claim[] claims =
        [
            new Claim(JwtRegisteredClaimNames.Sub,        userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email,      email),
            new Claim(JwtRegisteredClaimNames.GivenName,  firstName),
            new Claim(JwtRegisteredClaimNames.FamilyName, lastName),
            new Claim(JwtRegisteredClaimNames.Jti,        Guid.NewGuid().ToString()),
            new Claim("email_verified",                   isEmailVerified.ToString().ToLower()),
            new Claim("avatar_url",                       avatarUrl ?? string.Empty)
        ];

        var expiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
