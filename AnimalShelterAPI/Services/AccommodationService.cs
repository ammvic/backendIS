using AnimalShelterAPI.Models;
using AnimalShelterAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnimalShelterAPI.Services
{
    public class AccommodationService : IAccommodationService
    {
        private readonly ApiContext _context;

        public AccommodationService(ApiContext context)
        {
            _context = context;
        }

        // 1️⃣ Prikaz svih smeštaja sa životinjama
        public async Task<List<Accommodation>> GetAllAsync()
        {
            return await _context.Accommodations
                .Include(a => a.Animals)
                .ToListAsync();
        }

        // 2️⃣ Pronađi slobodne smeštaje po tipu životinje
        public async Task<List<Accommodation>> GetAvailableForType(AnimalType type)
        {
            return await _context.Accommodations
                .Where(a => a.AllowedAnimalType == type && a.CurrentOccupancy < a.Capacity)
                .ToListAsync();
        }

        // 3️⃣ Dodaj životinju u smeštaj
        public async Task<bool> AssignAnimalToAccommodation(int animalId, int accommodationId)
        {
            var animal = await _context.Animals.FindAsync(animalId);
            var accommodation = await _context.Accommodations
                .Include(a => a.Animals)
                .FirstOrDefaultAsync(a => a.Id == accommodationId);

            if (animal == null || accommodation == null) return false;
            if (accommodation.CurrentOccupancy >= accommodation.Capacity) return false;
            if (accommodation.AllowedAnimalType != animal.AnimalType) return false;

            animal.AccommodationId = accommodation.Id;
            accommodation.CurrentOccupancy++;

            await _context.SaveChangesAsync();
            return true;
        }

        // 4️⃣ Ukloni životinju iz smeštaja
        public async Task<bool> RemoveAnimalFromAccommodation(int animalId)
        {
            var animal = await _context.Animals
                .Include(a => a.Accommodation)
                .FirstOrDefaultAsync(a => a.Id == animalId);

            if (animal == null || animal.Accommodation == null) return false;

            var accommodation = animal.Accommodation;
            animal.AccommodationId = null;
            if (accommodation.CurrentOccupancy > 0)
                accommodation.CurrentOccupancy--;

            await _context.SaveChangesAsync();
            return true;
        }

        // 5️⃣ Proveri da li ima slobodnih mesta
        public async Task<bool> HasAvailableSpace(int accommodationId)
        {
            var accommodation = await _context.Accommodations.FindAsync(accommodationId);
            return accommodation != null && accommodation.CurrentOccupancy < accommodation.Capacity;
        }

        // 6️⃣ Povećaj zauzetost smeštaja
        public async Task IncreaseOccupancy(int accommodationId)
        {
            var accommodation = await _context.Accommodations.FindAsync(accommodationId);
            if (accommodation != null)
            {
                accommodation.CurrentOccupancy++;
                await _context.SaveChangesAsync();
            }
        }

        // 7️⃣ Smanji zauzetost smeštaja
        public async Task DecreaseOccupancy(int accommodationId)
        {
            var accommodation = await _context.Accommodations.FindAsync(accommodationId);
            if (accommodation != null && accommodation.CurrentOccupancy > 0)
            {
                accommodation.CurrentOccupancy--;
                await _context.SaveChangesAsync();
            }
        }
    }
}
