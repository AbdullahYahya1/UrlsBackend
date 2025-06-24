using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UrlsBackend.Data.Context;
using UrlsBackend.Data.IRepositories;


public class UnitOfWork : IUnitOfWork
    {
        private readonly UrlsDbContext _db;
        public IHttpContextAccessor HttpContextAccessor { get; }
        private readonly IMapper _mapper;
        public IUserRepository Users { get; }
        public IUrlRepository Urls{ get; }


    public UnitOfWork(UrlsDbContext context, IUserRepository userRepository, IHttpContextAccessor httpContextAccessor, IMapper mapper , IUrlRepository urls
)
        {
            HttpContextAccessor = httpContextAccessor;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _db = context ?? throw new ArgumentNullException(nameof(context));
            Users = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            Urls = urls ?? throw new ArgumentNullException(nameof(urls));
        }
        public IMapper Mapper => _mapper;
        public int? GetCurrentUserId()
        {
            var user = HttpContextAccessor.HttpContext?.User;
            var userIdClaim = user?.FindFirst("UserID")?.Value;

            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }

            return null; 
        }
    public async Task<int> SaveChangesAsync() => await _db.SaveChangesAsync();
        public async Task<IDbContextTransaction> BeginTransactionAsync() => await _db.Database.BeginTransactionAsync();

    }


