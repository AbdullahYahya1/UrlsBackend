using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using UrlsBackend.Data.Context;
using UrlsBackend.Data.Models;


    public class UserRepository : Repository<User>, IUserRepository
    {
        public IHttpContextAccessor HttpContextAccessor { get; }
        public UrlsDbContext dbContext { get; }
        public UserRepository(UrlsDbContext context, IHttpContextAccessor httpContextAccessor) : base(context)
        {
            HttpContextAccessor = httpContextAccessor;
            dbContext = context;
        }

        public async Task<User?> FindByEmailAsync(string email)
        {
            return await _dbSet.SingleOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> FindByRefreshTokenAsync(string refreshToken)
        {
            return await dbContext.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        }
        public string? GetUserEmail() => HttpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

        public int? GetUserId()
        {
            if (HttpContextAccessor?.HttpContext?.User?.FindFirst("UserID")?.Value is string userIdValue
                && int.TryParse(userIdValue, out int userId))
            {
                return userId;
            }

            return null;
        }

        public async Task<User?> GetCurrentUser()
        {
            return await _dbSet.FindAsync(GetUserId());
        }
    }
