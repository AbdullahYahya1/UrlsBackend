using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlsBackend.Data.Models;

namespace UrlsBackend.Data.Context
{
    public class UrlsDbContext : DbContext
    {

        public UrlsDbContext(DbContextOptions<UrlsDbContext> options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
    }
}
