using LMS.Domain.Enums;

namespace LMS.Application.DTO.Request.Book
{
    public class UpdateBookDto
    {
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Genre { get; set; }
        public ReadingStatus? ReadingStatus { get; set; }
    }
}
