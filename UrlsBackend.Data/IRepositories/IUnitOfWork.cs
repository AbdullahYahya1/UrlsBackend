using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;
using UrlsBackend.Data.IRepositories;

public interface IUnitOfWork
{
    IUserRepository Users { get; }
    public IUrlRepository Urls { get; }
    IHttpContextAccessor HttpContextAccessor { get; }
    IMapper Mapper { get; }
    Task<int> SaveChangesAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();

    int? GetCurrentUserId();
}
