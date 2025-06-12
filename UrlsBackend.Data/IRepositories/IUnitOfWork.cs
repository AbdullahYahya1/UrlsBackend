using AutoMapper;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.AspNetCore.Http;

public interface IUnitOfWork
{
    IUserRepository Users { get; }
    IHttpContextAccessor HttpContextAccessor { get; }
    IMapper Mapper { get; }
    Task<int> SaveChangesAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();


}
