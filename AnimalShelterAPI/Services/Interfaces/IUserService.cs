using AnimalShelterAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnimalShelterAPI.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetAll();
        Task<List<User>> GetNonAdminUsers(); 
    }
}
