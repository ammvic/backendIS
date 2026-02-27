using AnimalShelterAPI.Models;
using AnimalShelterAPI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnimalShelterAPI.Infrastructure.Repositories
{
    public class AnimalAggregatorRepository : IAnimalAggregatorRepository
    {
        private readonly ApiContext _context;

        public AnimalAggregatorRepository(ApiContext context)
        {
            _context = context;
        }

        public ICollection<Animal> GetBeforeVaccinationDate(DateTime beforeDate)
        {
            return _context.Animals.Where(s => s.VaccinationDate < beforeDate && s.LastReminder < beforeDate).ToList();
        }

        public int UpdateReminders(ICollection<Animal> animals)
        {
            int changes = 0;
            foreach (Animal animal in animals)
            {
                animal.LastReminder = DateTime.Now;
                try
                {
                    changes += _context.SaveChanges();
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException(e.Message);
                }
            }
            return changes;
        }
    }
}
