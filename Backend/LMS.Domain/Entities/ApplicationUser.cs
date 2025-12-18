using Microsoft.AspNetCore.Identity;

namespace LMS.Domain.Entities
{
    public class ApplicationUser : IdentityUser<Guid>, IBaseEntity
    {
        public string Name { get; set; }
        public ICollection<Book> Books { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
        DateTime IBaseEntity.CreatedAt { get; set; }
    }
}
