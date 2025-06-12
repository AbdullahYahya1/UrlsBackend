using Microsoft.AspNetCore.Mvc;
using Google.Apis.Auth;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace Translatron.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly string _googleClientId;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public AuthController(IConfiguration configuration, IUserService userService)
        {
            _configuration = configuration;
            _userService = userService;
            _googleClientId = _configuration["Authentication:Google:ClientId"]
                ?? throw new ArgumentNullException("Google Client ID not configured");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var response = await _userService.RegisterAsync(request.Username, request.Email, request.Password);
            return  Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await _userService.LoginAsync(request.Email, request.Password);
            return  Ok(response);
        }

        [HttpPost("loginGoogle")]
        public async Task<IActionResult> LoginWithGoogle([FromBody] GoogleAuthRequest request)
        {
            try
            {
                var payload = await ValidateGoogleToken(request.Credential);
                var response = await _userService.LoginWithGoogleAsync(payload.Email, payload.Name);
                return Ok(response);
            }
            catch (InvalidCredentialException ex)
            {
                return BadRequest(new ResponseModel<TokenResponse>
                {
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<TokenResponse>
                {
                    IsSuccess = false,
                    Message = "An error occurred while processing your request."
                });
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] refreshTokenDto refreshDto)
        {
            var response = await _userService.RefreshTokenAsync(refreshDto.refreshToken);
            return Ok(response);
        }

        private async Task<GoogleJsonWebSignature.Payload> ValidateGoogleToken(string credential)
        {
            try
            {
                return await GoogleJsonWebSignature.ValidateAsync(credential, new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _googleClientId }
                });
            }
            catch (Exception ex)
            {
                throw new InvalidCredentialException("Invalid Google token", ex);
            }
        }
    }
}