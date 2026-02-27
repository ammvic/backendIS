using Microsoft.EntityFrameworkCore;
using System.Linq;
using AnimalShelterAPI.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace AnimalShelterAPI.Infrastructure.Repositories
{
    public class FilterRepository : IFilterRepository
    {
        private readonly ApiContext _context;

        public FilterRepository(ApiContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Animal>> GetFilteredAll(DateTime fromDate, DateTime toDate)
        {
            return await _context.Animals.Where(x => x.AdmissionDate >= fromDate && x.AdmissionDate <= toDate).ToArrayAsync();
        }
    }
}
