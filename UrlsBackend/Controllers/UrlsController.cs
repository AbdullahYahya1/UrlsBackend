using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UrlsBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UrlsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetUrls()
        {
            return Redirect("https://chatgpt.com");
        }
    }
}
