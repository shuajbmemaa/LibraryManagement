using LMS.Domain.Entities;

namespace LMS.Infrastructure.Repositories.Interfaces
{
    public interface IBookRepository
    {
        IQueryable<Book> GetAll();
        Task<Book?> GetById(Guid id);
        Task Create(Book book);
        Task Update(Book book);
        Task<int> Delete(Guid id);
    }
}