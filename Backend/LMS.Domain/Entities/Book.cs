using LMS.Domain.Enums;

namespace LMS.Domain.Entities
{
    public class Book : BaseEntity
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public ReadingStatus ReadingStatus { get; set; } = ReadingStatus.Reading;
        public Guid UserId { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
