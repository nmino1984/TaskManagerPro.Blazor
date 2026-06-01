using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TaskManagerPro.Blazor.Application.Common.Interfaces;

namespace TaskManagerPro.Blazor.Infrastructure.Services;

/// <summary>
/// Generates HMAC-SHA256-signed JWTs using values from the Jwt section of appsettings.
/// The jti claim (unique token ID) allows individual tokens to be revoked if a
/// token blacklist is introduced in the future without changing the signing approach.
/// </summary>
public class JwtTokenGeneratorService : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Receives IConfiguration so JWT settings can be changed via environment variables
    /// or secrets without recompiling.
    /// </summary>
    public JwtTokenGeneratorService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Builds and signs a JWT containing identity claims for the given user.
    /// Reads Jwt:Key, Jwt:Issuer, Jwt:Audience, and Jwt:ExpirationMinutes from configuration.
    /// Returns both the compact token string and the UTC expiry timestamp.
    /// </summary>
    public (string Token, DateTime ExpiresAt) GenerateToken(
        Guid userId, string email, string firstName, string lastName)
    {
        string key = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Jwt:Key is not configured.");
        string issuer = _configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException("Jwt:Issuer is not configured.");
        string audience = _configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException("Jwt:Audience is not configured.");

        int expirationMinutes = int.TryParse(_configuration["Jwt:ExpirationMinutes"], out int parsed)
            ? parsed
            : 60;

        SymmetricSecurityKey signingKey = new(Encoding.UTF8.GetBytes(key));
        SigningCredentials credentials = new(signingKey, SecurityAlgorithms.HmacSha256);

        Claim[] claims =
        [
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.GivenName, firstName),
            new Claim(JwtRegisteredClaimNames.FamilyName, lastName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        ];

        DateTime expiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes);

        JwtSecurityToken token = new(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
