using System.Collections.Generic;
using System.Threading.Tasks;
using AnimalShelterAPI.Models.DTO;
using Microsoft.AspNetCore.JsonPatch;

namespace AnimalShelterAPI.Services.Interfaces
{
    public interface IAnimalService
    {
        Task<EditAnimalDto> GetById(int id);
        Task<ICollection<AnimalListItemDto>> GetAll();
        Task<AnimalListItemDto> Create(NewAnimalDto newItem);
        Task Update(int id, EditAnimalDto updateData);
        Task<bool> PartialUpdate(int id, JsonPatchDocument<NewAnimalDto> itemPatch);
        Task<bool> Delete(int id);
    }
}
