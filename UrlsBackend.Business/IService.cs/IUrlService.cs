using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlsBackend.Data.dtos;

namespace UrlsBackend.Business.IService.cs
{

    public interface IUrlService
    {
        Task<ResponseModel<GetUrlDto>> AddUrl(PostUrlDto postUrlDto);
        Task<ResponseModel<ICollection<GetUrlDto>>> GetUrls();
        Task<ResponseModel> Remove(int urlId);

        public Task<ResponseModel<string>> GetUrl(string urlSubDomain , string? password = null);

    }
}
