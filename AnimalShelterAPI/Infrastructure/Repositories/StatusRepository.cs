using AnimalShelterAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnimalShelterAPI.Infrastructure.Repositories
{
    public class StatusRepository : IStatusRepository
    {
        private readonly ApiContext _context;

        public async Task<Status> GetById(int id)
        {
            var item = await _context.Statuses.FirstOrDefaultAsync(x => x.ID == id);

            return item;
        }

        public StatusRepository(ApiContext context)
        {
            _context = context;
        }

        public virtual Task<Status> InShelterStatus => GetById(3);

        public virtual Task<Status> DeadStatus => GetById(2);

        public virtual Task<Status> GivenAwayStatus => GetById(1);
    }
}
