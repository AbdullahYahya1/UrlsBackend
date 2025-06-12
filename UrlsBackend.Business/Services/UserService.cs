using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UrlsBackend.Data.Models;


    public class UserService : IUserService
    {
        private readonly IAuthService _authService;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IAuthService authService, IUnitOfWork unitOfWork)
        {
            _authService = authService;
            _unitOfWork = unitOfWork;

        }

        public async Task<ResponseModel<GetUserDto>> RegisterAsync(string username, string email, string password)
        {
            var existingUser = await _unitOfWork.Users.FindAsync(u => u.Email == email);
            if (existingUser != null)
            {
                return new ResponseModel<GetUserDto>
                {
                    IsSuccess = false,
                    Message = "User already exists."
                };
            }

            var hashedPassword = _authService.HashPassword(password);
            var newUser = new User
            {
                Username = username,
                Email = email,
                PasswordHash = hashedPassword,
                UserType = UserType.Client
            };

            await _unitOfWork.Users.AddAsync(newUser);
            await _unitOfWork.SaveChangesAsync();

            var userDto = _unitOfWork.Mapper.Map<GetUserDto>(newUser);
            return new ResponseModel<GetUserDto>
            {
                IsSuccess = true,
                Result = userDto
            };
        }

        public async Task<ResponseModel<TokenResponse>> LoginAsync(string email, string password)
        {
            var user = await _authService.AuthenticateAsync(email, password);
            if (user == null || !_authService.VerifyPassword(password, user.PasswordHash))
            {
                return new ResponseModel<TokenResponse>
                {
                    IsSuccess = false,
                    Message = "Invalid credentials."
                };
            }

            var accessToken = _authService.GenerateJwtToken(user);
            var refreshToken = _authService.GenerateRefreshToken();

            user.RefreshToken = _authService.HashPassword(refreshToken);
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return new ResponseModel<TokenResponse>
            {
                IsSuccess = true,
                Result = new TokenResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                }
            };
        }

        public async Task<ResponseModel<TokenResponse>> LoginWithGoogleAsync(string email, string name)
        {
            string randomPassword = GenerateSecurePassword();
            string hashedPassword = _authService.HashPassword(randomPassword);

            var user = await _unitOfWork.Users.FindAsync(u => u.Email == email);
            if (user == null)
            {
                user = new User
                {
                    Username = name,
                    Email = email,
                    UserType = UserType.Client,
                    PasswordHash = hashedPassword
                };
                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();
            }

            var accessToken = _authService.GenerateJwtToken(user);
            var refreshToken = _authService.GenerateRefreshToken();
            user.RefreshToken = _authService.HashPassword(refreshToken);
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return new ResponseModel<TokenResponse>
            {
                IsSuccess = true,
                Result = new TokenResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                }
            };
        }

        public async Task<ResponseModel<TokenResponse>> RefreshTokenAsync(string refreshToken)
        {
            var hashedToken = _authService.HashPassword(refreshToken);

            var user = await _unitOfWork.Users.FindByRefreshTokenAsync(hashedToken);

            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return new ResponseModel<TokenResponse>
                {
                    IsSuccess = false,
                    Message = "Invalid or expired refresh token."
                };
            }

            var newAccessToken = _authService.GenerateJwtToken(user);
            var newRefreshToken = _authService.GenerateRefreshToken();

            user.RefreshToken = _authService.HashPassword(newRefreshToken);
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return new ResponseModel<TokenResponse>
            {
                IsSuccess = true,
                Result = new TokenResponse
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                }
            };
        }

        public async Task<int> GetCurrentUserID()
        {
            var httpContext = _unitOfWork.HttpContextAccessor.HttpContext;
            var user = httpContext.User;
            var userIdClaim = user.FindFirst("UserID")?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                throw new InvalidOperationException("UserID claim is not a valid integer.");
            }

            return userId;
        }

        public async Task<ResponseModel<GetUserDto>> GetCurrentUser()
        {
            var id = await GetCurrentUserID();
            if (id == null)
            {
                return new ResponseModel<GetUserDto>
                {
                    Message = "User ID not found",
                    IsSuccess = false
                };
            }

            var user = await _unitOfWork.Users.FindAsync(u => u.UserID == id);
            if (user == null)
            {
                return new ResponseModel<GetUserDto>
                {
                    Message = "User not found",
                    IsSuccess = false
                };
            }

            if (_unitOfWork.Mapper == null)
            {
                return new ResponseModel<GetUserDto>
                {
                    Message = "Mapper not initialized",
                    IsSuccess = false
                };
            }

            var userDto = _unitOfWork.Mapper.Map<GetUserDto>(user);
            return new ResponseModel<GetUserDto>
            {
                IsSuccess = true,
                Result = userDto
            };
        }
        private string GenerateSecurePassword(int length = 16)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_-+=<>?";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
