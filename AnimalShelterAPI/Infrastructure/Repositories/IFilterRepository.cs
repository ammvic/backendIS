using AnimalShelterAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnimalShelterAPI.Infrastructure.Repositories
{
    public interface IFilterRepository
    {
        Task<ICollection<Animal>> GetFilteredAll(DateTime fromDate, DateTime toDate);
    }
}
