using LMS.Domain.Entities;
using LMS.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public UserRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<ApplicationUser> GetAll()
        {
            return _dbContext.Users;
        }

        public async Task<ApplicationUser?> GetById(Guid id)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
