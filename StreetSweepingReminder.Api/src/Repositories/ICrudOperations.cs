namespace StreetSweepingReminder.Api.Repositories;

public interface ICrudOperations<T>
{
    Task<int> CreateAsync(T obj);
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<bool> UpdateAsync(T obj);
    Task<bool> DeleteAsync(int id);
}