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


    public class UnitOfWork : IUnitOfWork
    {
        private readonly UrlsDbContext _db;
        public IHttpContextAccessor HttpContextAccessor { get; }
        private readonly IMapper _mapper;
        public IUserRepository Users { get; }


        public UnitOfWork(UrlsDbContext context, IUserRepository userRepository, IHttpContextAccessor httpContextAccessor, IMapper mapper
)
        {
            HttpContextAccessor = httpContextAccessor;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _db = context ?? throw new ArgumentNullException(nameof(context));
            Users = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }
        public IMapper Mapper => _mapper;

        public async Task<int> SaveChangesAsync() => await _db.SaveChangesAsync();
        public async Task<IDbContextTransaction> BeginTransactionAsync() => await _db.Database.BeginTransactionAsync();

    }


