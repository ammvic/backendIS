using AnimalShelterAPI.Models.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnimalShelterAPI.Services.Interfaces
{
    public interface IFilterService
    {
        Task<ICollection<AnimalListItemDto>> GetAllFiltered(DateTime fromDate, DateTime toDate);
    }
}
