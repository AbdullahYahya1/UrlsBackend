
public interface IUserService
{
    Task<ResponseModel<GetUserDto>> GetCurrentUser();
    Task<ResponseModel<GetUserDto>> RegisterAsync(string username, string email, string password);
    Task<ResponseModel<TokenResponse>> LoginAsync(string email, string password);
    Task<ResponseModel<TokenResponse>> LoginWithGoogleAsync(string email, string name);
    Task<ResponseModel<TokenResponse>> RefreshTokenAsync(string refreshToken);

}

