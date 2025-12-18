using LMS.Application.DTO.Request.Book;
using LMS.Application.DTO.Response.Account;
using LMS.Application.DTO.Response.Book;
using LMS.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LMS.Application.Services.Book
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BookService(IBookRepository bookRepository, IHttpContextAccessor httpContextAccessor)
        {
            _bookRepository = bookRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<BookResponseDto> CreateAsync(CreateBookDto dto)
        {
            var book = MapToBook(dto);

            await _bookRepository.Create(book);

            return MapToDto(book);
        }

        private BookResponseDto MapToDto(Domain.Entities.Book book)
        {
            return new BookResponseDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Genre = book.Genre,
                ReadingStatus = book.ReadingStatus,
                User = new UserInfoDto
                {
                    Id = book.User?.Id ?? Guid.Empty,
                    UserName = book.User?.Name ?? string.Empty
                }
            };
        }

        private Domain.Entities.Book MapToBook(CreateBookDto dto)
        {
            var userId = GetCurrentUserId();
            return new Domain.Entities.Book
            {
                Title = dto.Title,
                Author = dto.Author,
                Genre = dto.Genre,
                UserId = userId,
                CreatedBy = userId.ToString()
            };
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var book = await _bookRepository.GetById(id);
            if (book == null) return false;

            if (GetCurrentUserRole() != "Admin" && book.UserId != GetCurrentUserId())
                throw new UnauthorizedAccessException();

            return await _bookRepository.Delete(id) > 0;
        }

        public async Task<List<BookResponseDto>> GetAllAsync(BookFilterDto? filter = null)
        {
            IQueryable<Domain.Entities.Book> query = _bookRepository.GetAll()
                .Include(b => b.User);

            if (GetCurrentUserRole() != "Admin")
            {
                var userId = GetCurrentUserId();
                query = query.Where(b => b.UserId == userId);
            }

            query = ApplyFilters(query, filter);

            var books = await query.ToListAsync();
            return books.Select(MapToDto).ToList();
        }

        private IQueryable<Domain.Entities.Book> ApplyFilters(IQueryable<Domain.Entities.Book> query, BookFilterDto? filter)
        {
            if (filter == null) return query;

            if (!string.IsNullOrWhiteSpace(filter.Title))
                query = query.Where(b => b.Title.Contains(filter.Title));

            if (!string.IsNullOrWhiteSpace(filter.Author))
                query = query.Where(b => b.Author.Contains(filter.Author));

            if (!string.IsNullOrWhiteSpace(filter.Genre))
                query = query.Where(b => b.Genre.Contains(filter.Genre));

            if (filter.ReadingStatus != null)
                query = query.Where(b => b.ReadingStatus == filter.ReadingStatus);

            return query;
        }

        public async Task<BookResponseDto?> GetByIdAsync(Guid id)
        {
            var book = await _bookRepository.GetAll().Include(b => b.User).FirstOrDefaultAsync(b => b.Id == id);
            if (book == null) return null;

            if (GetCurrentUserRole() != "Admin" && book.UserId != GetCurrentUserId())
                throw new UnauthorizedAccessException();

            return MapToDto(book);
        }

        public async Task<BookResponseDto?> UpdateAsync(Guid id, UpdateBookDto dto)
        {
            var book = await _bookRepository.GetById(id);
            if (book == null) return null;

            if (GetCurrentUserRole() != "Admin" && book.UserId != GetCurrentUserId())
                throw new UnauthorizedAccessException();

            MapUpdate(book, dto);

            await _bookRepository.Update(book);
            return MapToDto(book);
        }

        private void MapUpdate(Domain.Entities.Book book, UpdateBookDto dto)
        {
            book.Title = dto.Title ?? book.Title;
            book.Author = dto.Author ?? book.Author;
            book.Genre = dto.Genre ?? book.Genre;
            book.ReadingStatus = dto.ReadingStatus ?? book.ReadingStatus;
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) throw new UnauthorizedAccessException();
            return Guid.Parse(userIdClaim);
        }

        private string GetCurrentUserRole()
        {
            return _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role).Value;
        }
    }
}