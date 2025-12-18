using LMS.Domain.Enums;

namespace LMS.Application.DTO.Response.Book
{
    public class BookFilterDto
    {
        public string? Genre { get; set; }
        public string? Author { get; set; }
        public string? Title { get; set; }
        public ReadingStatus? ReadingStatus { get; set; }
    }
}
