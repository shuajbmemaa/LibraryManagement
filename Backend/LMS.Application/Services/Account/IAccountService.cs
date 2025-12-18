using LMS.Application.DTO.Request.Account;
using LMS.Application.DTO.Response.Account;

namespace LMS.Application.Services.Account
{
    public interface IAccountService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
    }
}
