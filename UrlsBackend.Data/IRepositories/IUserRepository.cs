using UrlsBackend.Data.Models;

public interface IUserRepository:IRepository<User>
{
    Task<User?> FindByEmailAsync(string email);
    Task<User> FindByRefreshTokenAsync(string refreshToken);
    Task<User?> GetCurrentUser();
    int? GetUserId();
    string? GetUserEmail();

}
