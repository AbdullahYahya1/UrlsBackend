using UrlsBackend.Data.Models;
public interface IAuthService
{
    string GenerateJwtToken(User user);
    string GenerateRefreshToken();
    string HashPassword(string password);
    bool VerifyPassword(string enteredPassword, string storedHash);
    Task<string?> RefreshTokenAsync(string refreshToken);
    Task<User?> AuthenticateAsync(string email, string password);
}
