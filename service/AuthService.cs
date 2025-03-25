using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WebApplication6.Data;
using WebApplication6.Models.Entities;

public class AuthService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IConfiguration _configuretion;
    public AuthService(ApplicationDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuretion = configuration;
    }
    public async Task<string> GenerateAndSaveRefreshToken(Auth user)
    {
        var refreshToken = GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshExpriryToken = DateTime.UtcNow.AddDays(7);
        await _dbContext.SaveChangesAsync();

        return refreshToken;
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var randomNumberGenerate = RandomNumberGenerator.Create();
        randomNumberGenerate.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public string CreateToken(Auth user)
    {
        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuretion.GetValue<string>("AppSettings:Token")!));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var toKenDescriptor = new JwtSecurityToken(
                issuer: _configuretion.GetValue<string>("AppSettings:Issuer"),
                audience: _configuretion.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(toKenDescriptor);
    }
}