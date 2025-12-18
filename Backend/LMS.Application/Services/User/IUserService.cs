using LMS.Application.DTO.Request.User;
using LMS.Application.DTO.Response.User;

namespace LMS.Application.Services.User
{
    public interface IUserService
    {
        Task<string> CreateAsync(CreateUserDto dto);
        Task<List<UserResponseDto>> GetAllAsync();
        Task<bool> DeleteAsync(Guid id);
    }
}
