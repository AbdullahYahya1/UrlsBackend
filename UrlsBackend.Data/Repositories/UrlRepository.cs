using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlsBackend.Data.Context;
using UrlsBackend.Data.IRepositories;
using UrlsBackend.Data.Models;

namespace UrlsBackend.Data.Repositories
{
    public class UrlRepository : Repository<Url>, IUrlRepository
    {
        public UrlRepository(UrlsDbContext context) : base(context)
        {
            _context = context;
        }

        public UrlsDbContext _context { get; }

        public async Task<Url?> GetUrl(int? id, string? subDomainName)
        {
            Url? url = null;

            if (id != null)
            {
                url = await _context.Urls.FirstOrDefaultAsync(u => u.UrlId == id);
            }
            else if (!string.IsNullOrEmpty(subDomainName))
            {
                url = await _context.Urls.FirstOrDefaultAsync(u => u.newUrl == subDomainName);
            }

            return url;
        }

        public async Task<ICollection<Url>> GetUrlsByUserId(int userId)
        {
            var urls = await _context.Urls.Where(u=> u.UserID == userId).ToListAsync();
            return urls;
        }
    }
}
