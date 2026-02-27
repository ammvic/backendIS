using AnimalShelterAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnimalShelterAPI.Infrastructure.Repositories.Interfaces
{
    public interface IAccommodationRepository
    {
        Task<IEnumerable<Accommodation>> GetAllAsync();
        Task<Accommodation?> GetByIdAsync(int id);
        Task AddAsync(Accommodation accommodation);
        Task UpdateAsync(Accommodation accommodation);
        Task DeleteAsync(Accommodation accommodation);
        Task<bool> ExistsAsync(int id);
    }
}
