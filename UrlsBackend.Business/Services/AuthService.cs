using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UrlsBackend.Data.Models;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;
    private readonly byte[] _hmacSecretKey;

    public AuthService(IConfiguration configuration, IUnitOfWork unitOfWork)
    {
        _configuration = configuration;
        _unitOfWork = unitOfWork;
        var keyString = _configuration["Auth:SecretKey"]
    ?? throw new ArgumentNullException("Auth:SecretKey not configured");
       _hmacSecretKey = Encoding.UTF8.GetBytes(keyString);

    }

    public string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, user.Username ?? ""),
            new Claim("UserID", user.UserID.ToString()),
            new Claim("UserType", user.UserType.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public string HashPassword(string password)
    {
        using var hmac = new HMACSHA256(_hmacSecretKey);
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hash);
    }


    public bool VerifyPassword(string enteredPassword, string storedHash)
    {
        using var hmac = new HMACSHA256(_hmacSecretKey);
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(enteredPassword));
        return Convert.ToBase64String(hash) == storedHash;
    }


    public async Task<User?> AuthenticateAsync(string email, string password)
    {
        
        var user = await _unitOfWork.Users.FindAsync(u => u.Email == email);
        if (user != null && VerifyPassword(password, user.PasswordHash))
        {
            return user;
        }
        return null;
    }

    public async Task<string?> RefreshTokenAsync(string refreshToken)
    {
        var user = await _unitOfWork.Users.FindByRefreshTokenAsync(refreshToken);
        if (user == null) return null;

        var newAccessToken = GenerateJwtToken(user);
        var newRefreshToken = GenerateRefreshToken();

        user.RefreshToken = HashPassword(newRefreshToken);
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _unitOfWork.Users.UpdateAsync(user);

        return newAccessToken;
    }
}
