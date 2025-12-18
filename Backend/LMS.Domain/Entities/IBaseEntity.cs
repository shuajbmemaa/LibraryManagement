namespace LMS.Domain.Entities
{
    public interface IBaseEntity
    {
        Guid Id { get; set; }
        DateTime CreatedAt { get; set; }
        bool IsDeleted { get; set; }
    }
}
