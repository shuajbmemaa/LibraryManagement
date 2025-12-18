using LMS.Application.DTO.Request.Book;
using LMS.Application.DTO.Response.Book;

namespace LMS.Application.Services.Book
{
    public interface IBookService
    {
        Task<List<BookResponseDto>> GetAllAsync(BookFilterDto? filter = null);
        Task<BookResponseDto?> GetByIdAsync(Guid id);
        Task<BookResponseDto> CreateAsync(CreateBookDto dto);
        Task<BookResponseDto?> UpdateAsync(Guid id, UpdateBookDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
