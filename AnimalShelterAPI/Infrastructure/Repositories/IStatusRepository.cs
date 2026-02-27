using AnimalShelterAPI.Models;
using System.Threading.Tasks;

namespace AnimalShelterAPI.Infrastructure.Repositories
{
    public interface IStatusRepository
    {
        public Task<Status> InShelterStatus { get; }
        public Task<Status> DeadStatus { get; }
        public Task<Status> GivenAwayStatus { get; }
        public Task<Status> GetById(int id);
    }
}
