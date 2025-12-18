using LMS.Application.DTO.Response.Account;
using LMS.Domain.Enums;

namespace LMS.Application.DTO.Response.Book
{
    public class BookResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public ReadingStatus ReadingStatus { get; set; }
        public UserInfoDto User { get; set; }
    }
}
