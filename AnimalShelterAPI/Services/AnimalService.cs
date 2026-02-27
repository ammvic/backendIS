using AnimalShelterAPI.Infrastructure.Repositories;
using AnimalShelterAPI.Models;
using AnimalShelterAPI.Models.DTO;
using AnimalShelterAPI.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnimalShelterAPI.Services
{
    public class AnimalService : IAnimalService
    {
        private readonly IRepository<Animal> _repository;
        private readonly IStatusRepository _statusRepository;
        private readonly IFilterRepository _filterRepository;
        private readonly IAccommodationService _accommodationService;
        private readonly IMapper _mapper;

        public AnimalService(
            IRepository<Animal> repository,
            IStatusRepository statusRepository,
            IMapper mapper,
            IFilterRepository filterRepository,
            IAccommodationService accommodationService)
        {
            _repository = repository;
            _mapper = mapper;
            _statusRepository = statusRepository;
            _filterRepository = filterRepository;
            _accommodationService = accommodationService;
        }

        public async Task<EditAnimalDto> GetById(int id)
        {
            var animal = await _repository.GetById(id);
            var animalDto = _mapper.Map<EditAnimalDto>(animal);

            animalDto.AnimalTimeInShelterCounter = FormatAnimalAge((DateTime.Today - animal.AdmissionDate).TotalDays);
            if (animal.Status.Name != "Uginuo")
            {
                var CalculationDate = animal.Birthday == null ? animal.AdmissionDate : animal.Birthday.Value;

                if (animal.Status.Name == "Poklonjen")
                    animalDto.AnimalAgeCounter = FormatAnimalAge((animal.StatusDate.Value - CalculationDate).TotalDays);
                else
                    animalDto.AnimalAgeCounter = FormatAnimalAge((DateTime.Now - CalculationDate).TotalDays);
            }
            else
                animalDto.AnimalAgeCounter = FormatAnimalAge((DateTime.Now - animal.StatusDate.Value).TotalDays);

            return animalDto;
        }

        public async Task<ICollection<AnimalListItemDto>> GetAll()
        {
            var animals = await _repository.GetAll(); 
            var animalDto = _mapper.Map<AnimalListItemDto[]>(animals);
            return animalDto;
        }


        public async Task<ICollection<AnimalListItemDto>> GetFilteredAnimals(DateTime fromDate, DateTime toDate)
        {
            var animals = await _filterRepository.GetFilteredAll(fromDate, toDate);
            var animalDto = _mapper.Map<AnimalListItemDto[]>(animals);
            return animalDto;
        }

        public async Task<AnimalListItemDto> Create(NewAnimalDto newItem)
        {
            if (newItem == null) throw new ArgumentNullException(nameof(newItem));

            var animal = await CreateAnimalPoco(newItem);

            // 🔹 NOVO: Upravljanje smeštajem — proveri ima li mesta
            if (animal.AccommodationId.HasValue)
            {
                bool hasSpace = await _accommodationService.HasAvailableSpace(animal.AccommodationId.Value);
                if (!hasSpace)
                    throw new InvalidOperationException("Nema slobodnih mesta u izabranom smeštaju!");

                await _accommodationService.IncreaseOccupancy(animal.AccommodationId.Value);
            }

            await _repository.Create(animal);

            var animalDto = _mapper.Map<AnimalListItemDto>(animal);
            return animalDto;
        }

        public async Task Update(int id, EditAnimalDto updateData)
        {
            if (updateData == null) throw new ArgumentNullException(nameof(updateData));

            var itemToUpdate = await _repository.GetById(id);
            if (itemToUpdate == null)
                throw new InvalidOperationException($"Animal {id} was not found");

            itemToUpdate.Status = await _statusRepository.GetById(updateData.StatusID);
            if (itemToUpdate.Status == null)
                throw new InvalidOperationException($"Status {updateData.StatusID} was not found");

            // 🔹 NOVO: Ako se promenio smeštaj, ažuriraj kapacitet
            if (itemToUpdate.AccommodationId != updateData.AccommodationId)
            {
                if (itemToUpdate.AccommodationId.HasValue)
                    await _accommodationService.DecreaseOccupancy(itemToUpdate.AccommodationId.Value);

                if (updateData.AccommodationId.HasValue)
                {
                    bool hasSpace = await _accommodationService.HasAvailableSpace(updateData.AccommodationId.Value);
                    if (!hasSpace)
                        throw new InvalidOperationException("Nema slobodnih mesta u novom smeštaju!");

                    await _accommodationService.IncreaseOccupancy(updateData.AccommodationId.Value);
                }
            }

            _mapper.Map(updateData, itemToUpdate);
            await _repository.Update(itemToUpdate);
        }

        public async Task<bool> PartialUpdate(int id, JsonPatchDocument<NewAnimalDto> itemPatch)
        {
            if (itemPatch == null) throw new ArgumentNullException(nameof(itemPatch));

            var itemToUpdate = await _repository.GetById(id);
            if (itemToUpdate == null)
                throw new InvalidOperationException($"Animal {id} was not found");

            var updateData = _mapper.Map<NewAnimalDto>(itemToUpdate);
            itemPatch.ApplyTo(updateData);
            _mapper.Map(updateData, itemToUpdate);

            var updated = await _repository.Update(itemToUpdate);
            return updated;
        }

        public async Task<bool> Delete(int id)
        {
            var item = await _repository.GetById(id);
            if (item == null) return false;

            // 🔹 NOVO: Oslobodi mesto u smeštaju
            if (item.AccommodationId.HasValue)
                await _accommodationService.DecreaseOccupancy(item.AccommodationId.Value);

            var deleted = await _repository.Delete(item);
            return deleted;
        }

        private async Task<Animal> CreateAnimalPoco(NewAnimalDto newItem)
        {
            var animal = _mapper.Map<Animal>(newItem);
            animal.Status = await _statusRepository.InShelterStatus;
            animal.StatusDate = DateTime.Now;
            return animal;
        }

        private string FormatAnimalAge(double DayCount)
        {
            double years = Math.Truncate(DayCount / 365);
            double months = Math.Truncate((DayCount % 365) / 30);
            double days = Math.Truncate((DayCount % 365) % 30);
            return string.Format("{0} godina {1} mesec {2} dana", years, months, days);
        }
    }
}
