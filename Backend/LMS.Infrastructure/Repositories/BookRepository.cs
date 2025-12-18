using LMS.Domain.Entities;
using LMS.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext _context;

        public BookRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Create(Book book)
        {
            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();
        }

        public async Task<int> Delete(Guid id)
        {
            return await _context.Books.Where(x => x.Id == id)
                .ExecuteUpdateAsync(x => x.SetProperty(b => b.IsDeleted, true));
        }

        public IQueryable<Book> GetAll()
        {
            return _context.Books;
        }

        public async Task<Book?> GetById(Guid id)
        {
            return await _context.Books
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task Update(Book book)
        {
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
        }
    }
}
