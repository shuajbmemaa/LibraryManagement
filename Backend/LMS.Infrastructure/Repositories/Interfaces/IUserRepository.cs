using LMS.Domain.Entities;

namespace LMS.Infrastructure.Repositories.Interfaces
{
    public interface IUserRepository
    {
        IQueryable<ApplicationUser> GetAll();
        Task<ApplicationUser?> GetById(Guid id);
    }
}
