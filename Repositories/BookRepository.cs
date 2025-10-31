using GoogleBookAPI.Data;
using GoogleBookAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace GoogleBookAPI.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly BookDbContext _dbContext;
        public BookRepository(BookDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Standard CRUD
        public async Task<IEnumerable<T>> GetAllAsync<T>() where T : class
            => await _dbContext.Set<T>().ToListAsync();

        public async Task<T?> GetByIdAsync<T>(int id) where T : class
            => await _dbContext.Set<T>().FindAsync(id);

        public async Task AddAsync<T>(T entity) where T : class
            => await _dbContext.Set<T>().AddAsync(entity);
        public void Update<T>(T entity) where T : class
            => _dbContext.Set<T>().Update(entity);

        public void Delete<T>(T entity) where T : class
            => _dbContext.Set<T>().Remove(entity);

        public async Task SaveChangesAsync()
            => await _dbContext.SaveChangesAsync();

        // Book specific methods
        public async Task<IEnumerable<Book>> GetBooksByCategoryAsync(string category)
        {
            return await _dbContext.Books
                .Include(b => b.Authors)
                .Include(b => b.Categories)
                .Include(b => b.IndustryIdentifiers)
                .Where(b => b.Categories.Any(c => c.Name.ToLower() == category.ToLower()))
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm)
        {
            return await _dbContext.Books
                .Include(b => b.Authors)
                .Include(b => b.Categories)
                .Where(b =>
                    b.Title.Contains(searchTerm) ||
                    b.Authors.Any(a => a.Name.Contains(searchTerm)) ||
                    b.Categories.Any(c => c.Name.Contains(searchTerm)))
                .ToListAsync();
        }

        public async Task<Book?> GetBookWithDetailsAsync(int id)
        {
            return await _dbContext.Books
                .Include(b => b.Authors)
                .Include(b => b.Categories)
                .Include(b => b.IndustryIdentifiers)
                .FirstOrDefaultAsync(b => b.Id == id);
        }
    }
}
