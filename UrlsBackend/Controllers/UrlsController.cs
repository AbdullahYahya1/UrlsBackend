using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Xml;
using UrlsBackend.Business.IService.cs;
using UrlsBackend.Data.dtos;
using UrlsBackend.Data.Models;

namespace UrlsBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UrlsController : ControllerBase
    {
        private readonly IUrlService _urlService;
        public UrlsController(IUrlService urlService)
        {
            
            _urlService = urlService;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUrls()
        {
            var response =await _urlService.GetUrls();
            return Ok(response);
        }
        [HttpPost]
        public async Task<IActionResult> AddUrl(PostUrlDto postUrlDto)
        {
            var response =await _urlService.AddUrl(postUrlDto);
            return Ok(response);
        }
        [HttpDelete("{urlId}")]
        [Authorize]
        public async Task<IActionResult> RemoveUrl(int urlId)
        {

            var response =await _urlService.Remove(urlId);
            return Ok(response);

        }
        [HttpGet("{subdomain}")]
        public async Task<IActionResult> RedirectUrl(string subdomain, [FromQuery] string? password = null)
        {

            var response = await _urlService.GetUrl(subdomain, password);
            return Redirect(response.Result);
        }

    }
}
