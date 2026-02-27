using AnimalShelterAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnimalShelterAPI.Infrastructure.Repositories
{
    public interface IUserRepository
    {
        Task<List<User>> GetAll();
    }
}
