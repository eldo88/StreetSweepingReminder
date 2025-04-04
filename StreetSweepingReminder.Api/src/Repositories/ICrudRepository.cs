namespace StreetSweepingReminder.Api.Repositories;

public interface ICrudRepository<T>
{
    Task<int> CreateAsync(T obj);
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync(string userId);
    Task<bool> UpdateAsync(T obj);
    Task<bool> DeleteAsync(int id);
}