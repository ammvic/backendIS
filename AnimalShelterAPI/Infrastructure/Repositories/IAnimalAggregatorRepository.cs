using AnimalShelterAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnimalShelterAPI.Services.Interfaces
{
    public interface IAnimalAggregatorRepository
    {
        ICollection<Animal> GetBeforeVaccinationDate(DateTime beforeDate);
        int UpdateReminders(ICollection<Animal> animals);
    }
}
