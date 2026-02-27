using AnimalShelterAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnimalShelterAPI.Services.Interfaces
{
    public interface IAccommodationService
    {
        // Sve osnovne metode za rad sa smeštajem
        Task<List<Accommodation>> GetAllAsync();
        Task<List<Accommodation>> GetAvailableForType(AnimalType type);
        Task<bool> AssignAnimalToAccommodation(int animalId, int accommodationId);
        Task<bool> RemoveAnimalFromAccommodation(int animalId);

        // Metode koje koristi AnimalService
        Task<bool> HasAvailableSpace(int accommodationId);
        Task IncreaseOccupancy(int accommodationId);
        Task DecreaseOccupancy(int accommodationId);
    }
}
