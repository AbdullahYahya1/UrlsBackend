
using Microsoft.Extensions.Configuration;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using UrlsBackend.Business.IService.cs;
using UrlsBackend.Data.dtos;
using UrlsBackend.Data.Models;

namespace UrlsBackend.Business.Service.cs
{
    public class UrlService: IUrlService
    {
        private readonly IUnitOfWork _unitOfWork;
        public readonly IAuthService _authService;
        private readonly string _backendUrl;
        private readonly string _frontUrl;

        public UrlService(IUnitOfWork unitOfWork, IAuthService authService , IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _authService = authService;
            _backendUrl = configuration["Backend:BaseUrl"];
            _frontUrl = configuration["FrontEnd:BaseUrl"];

        }

        public async Task<ResponseModel<GetUrlDto>> AddUrl(PostUrlDto postUrlDto)
        {
            if (string.IsNullOrWhiteSpace(postUrlDto.oldUrl))
            {
                return new ResponseModel<GetUrlDto>
                {
                    IsSuccess = false,
                    Message = "Original URL is required."
                };
            }

            if (string.IsNullOrWhiteSpace(postUrlDto.newUrl))
            {
                return new ResponseModel<GetUrlDto>
                {
                    IsSuccess = false,
                    Message = "Short URL is required."
                };
            }

            if (postUrlDto.ExpairDate.HasValue && postUrlDto.ExpairDate.Value < DateTime.Today.Date)
            {
                return new ResponseModel<GetUrlDto>
                {
                    IsSuccess = false,
                    Message = "Expiration date cannot be in the past."
                };
            }
            
            if (postUrlDto.MaxClicks.HasValue && postUrlDto.MaxClicks.Value < 1)
            {
                return new ResponseModel<GetUrlDto>
                {
                    IsSuccess = false,
                    Message = "MaxClicks must be at least 1 if provided."
                };
            }



            var IsUrlInDatabase = await _unitOfWork.Urls.GetUrl(subDomainName:postUrlDto.newUrl); 
            if(IsUrlInDatabase != null)
            {
                return new ResponseModel<GetUrlDto>
                {
                    IsSuccess = false,
                    Message = "SubDomain already acquired."
                };
            }
            var url = _unitOfWork.Mapper.Map<Url>(postUrlDto);

            var userId = _unitOfWork.GetCurrentUserId();
            if (userId != null) 
            {
                url.UserID = userId.Value;
            }

            url.CreatedAt = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(postUrlDto.Password))
            {
                url.Password= _authService.HashPassword(postUrlDto.Password);
            }
            await _unitOfWork.Urls.AddAsync(url);
            await _unitOfWork.SaveChangesAsync();
            var dto = _unitOfWork.Mapper.Map<GetUrlDto>(url);
            return new ResponseModel<GetUrlDto>
            {
                IsSuccess = true,
                Result = dto
            };
        }


        public async Task<ResponseModel<string>> GetUrl(string urlSubDomain, string? password = null)
        {
            var url = await _unitOfWork.Urls.GetUrl(subDomainName: urlSubDomain);
            if (url == null)
            {
                return new ResponseModel<string>
                {
                    IsSuccess = false,
                    Message = "URL not found.",
                    Result = $"{_frontUrl}/{urlSubDomain}/status/{Status.notFound}"
                };
            }

            if (url.ExpairDate.HasValue && url.ExpairDate.Value.Date < DateTime.UtcNow.Date)
            {
                return new ResponseModel<string>
                {
                    IsSuccess = false,
                    Message = "URL is disabled.",
                    Result = $"{_frontUrl}/{urlSubDomain}/status/{Status.expired}"
                };
            }

            if (url.MaxClicks.HasValue && url.ClicksCounter >= url.MaxClicks.Value)
            {
                return new ResponseModel<string>
                {
                    IsSuccess = false,
                    Message = "URL has reached its maximum number of clicks.",
                    Result = $"{_frontUrl}/{urlSubDomain}/status/{Status.maxClicks}"
                };
            }
            url.ClicksCounter++;
            await _unitOfWork.SaveChangesAsync();

            if (url.Password != null && password == null)
            {
                return new ResponseModel<string>
                {
                    IsSuccess = false,
                    Message = "Password is required to access this URL.",
                    Result = $"{_frontUrl}/{urlSubDomain}/protected"
                };
            }

            if (password != null && url.Password!=null && !string.IsNullOrWhiteSpace(url.Password))
            {
                if (!_authService.VerifyPassword(password, url.Password))
                {
                    return new ResponseModel<string>
                    {
                        IsSuccess = false,
                        Message = "Invalid password.",
                        Result = $"{_frontUrl}/{urlSubDomain}/protected"
                    };
                }
            }


            return new ResponseModel<string>
            {
                IsSuccess = true,
                Result = url.oldUrl
            };
        }

        public async Task<ResponseModel<ICollection<GetUrlDto>>> GetUrls()
        {
            var userId = _unitOfWork.GetCurrentUserId();
            if (userId == null)
            {
                return new ResponseModel<ICollection<GetUrlDto>>
                {
                    IsSuccess = false,
                    Message = "User not authenticated."
                };
            }
            var urls = await _unitOfWork.Urls.GetUrlsByUserId(userId.Value);
            var UrlsDto = _unitOfWork.Mapper.Map<Collection<GetUrlDto>>(urls);

            return new ResponseModel<ICollection<GetUrlDto>>
            {
                IsSuccess = true,
                Result = UrlsDto
            };  
        }

        public async Task<ResponseModel> Remove(int urlId)
        {
            var url = await _unitOfWork.Urls.GetUrl(id: urlId);
            if (url == null)
            {
                return new ResponseModel
                {
                    IsSuccess = false,
                    Message = "URL not found."
                };
            }
            if (url.UserID != _unitOfWork.GetCurrentUserId())
            {
                return new ResponseModel
                {
                    IsSuccess = false,
                    Message = "You do not have permission to delete this URL."
                };
            }
            await _unitOfWork.Urls.DeleteAsync(url.UrlId);
            await _unitOfWork.SaveChangesAsync();
            return new ResponseModel
            {
                IsSuccess = true,
                Message = "URL deleted successfully."
            };
        }

 
    }
}
public enum Status
{
    notFound,
    maxClicks,
    disabled,
    expired,
    deleted
}
