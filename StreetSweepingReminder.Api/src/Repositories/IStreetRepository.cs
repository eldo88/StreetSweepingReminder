using StreetSweepingReminder.Api.Entities;

namespace StreetSweepingReminder.Api.Repositories;

public interface IStreetRepository
{
    Task<int> CreateAsync(Street street);
    Task<Street?> GetByIdAsync(int id);
    Task<IEnumerable<Street>> GetByPartialStreetName(string partialStreetName);
}