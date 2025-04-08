using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using StreetSweepingReminder.Api.Entities;

namespace StreetSweepingReminder.Api.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;

    public AuthService(UserManager<User> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public static int TokenExpirationInMinutes => 30;

    public async Task<User?> FindByNameAsync(string username)
    {
        return await _userManager.FindByNameAsync(username);
    }

    public async Task<User?> FindByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<IdentityResult> CreateAsync(User user, string password)
    {
        return await _userManager.CreateAsync(user, password);
    }

    public async Task<bool> CheckPasswordAsync(User user, string password)
    {
        return await _userManager.CheckPasswordAsync(user, password);
    }

    public DateTime GetTokenExpirationTimeStamp()
    {
        return DateTime.Now.AddMinutes(TokenExpirationInMinutes);
    }

    public string GenerateJwtToken(User user)
    {
        var jwtKey = GetJwtKey();
        var jwtIssuer = GetJwtIssuer();
        var jwtAudience = GetJwtAudience();

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = GenerateClaims(user);

        var expiration = GetTokenExpirationTimeStamp();

        var token = GenerateJwtSecurityToken(jwtIssuer, jwtAudience, claims, expiration, credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GetJwtKey()
    {
        var jwtKey = _configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new InvalidOperationException("Error getting JWT Key from configuration.");
        }

        return jwtKey;
    }

    private string GetJwtIssuer()
    {
        var jwtIssuer = _configuration["Jwt:Issuer"];
        if (string.IsNullOrEmpty(jwtIssuer))
        {
            throw new InvalidOperationException("Error getting JWT Issuer from configuration.");
        }

        return jwtIssuer;
    }

    private string GetJwtAudience()
    {
        var jwtAudience = _configuration["Jwt:Audience"];
        if (string.IsNullOrEmpty(jwtAudience))
        {
            throw new InvalidOperationException("Error getting JWT Audience from configuration");
        }

        return jwtAudience;
    }
    
    private Claim[] GenerateClaims(User user)
    {
        return
        [
            new Claim(JwtRegisteredClaimNames.Sub,
                user.UserName ?? throw new InvalidOperationException("Username cannot be null.")),
            new Claim(JwtRegisteredClaimNames.Email,
                user.Email ?? throw new InvalidOperationException("Email cannot be null")),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        ];
    }

    private JwtSecurityToken GenerateJwtSecurityToken(string jwtIssuer, string jwtAudience, Claim[] claims, DateTime expires, SigningCredentials credentials)
    {
        return new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials);
    }
}