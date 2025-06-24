
using UrlsBackend.Data.Models;

namespace UrlsBackend.Data.IRepositories
{
    public interface IUrlRepository:IRepository<Url>
    {
        Task<Url?> GetUrl(int? id=null , string? subDomainName = null);
        Task<ICollection<Url>> GetUrlsByUserId(int userId);
    }
}
