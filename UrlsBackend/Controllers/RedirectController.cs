using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc;
using UrlsBackend.Business.IService.cs;

namespace UrlsBackend.Controllers
{
    [ApiController]
    public class RedirectController : ControllerBase
    {
        private readonly IUrlService _urlService;

        public RedirectController(IUrlService urlService)
        {
            _urlService = urlService;
        }

        // This route matches: https://localhost:7126/anythingHere
        [HttpGet("{subdomain}")]
        public async Task<IActionResult> RedirectUrl(string subdomain, [FromQuery] string? password = null)
        {
            var response = await _urlService.GetUrl(subdomain, password);
            if (response == null || string.IsNullOrEmpty(response.Result))
            {
                return NotFound("URL not found or access denied.");
            }
            if (response != null && password != null)
            {
                return Ok(response);
            }

            return Redirect(response.Result);
        }
    }
}
