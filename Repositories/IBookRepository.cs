using GoogleBookAPI.Models.Entities;

namespace GoogleBookAPI.Repositories
{
    public interface IBookRepository
    {
        // Standard CRUD
        Task<IEnumerable<T>> GetAllAsync<T>() where T : class;
        Task<T?> GetByIdAsync<T>(int id) where T : class;
        Task AddAsync<T>(T entity) where T : class;
        void Update<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        Task SaveChangesAsync();

        // Book specific methods
        Task<IEnumerable<Book>> GetBooksByCategoryAsync(string category);
        Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm);
        Task<Book?> GetBookWithDetailsAsync(int id);
    }
}
