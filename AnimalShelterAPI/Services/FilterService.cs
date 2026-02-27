using AnimalShelterAPI.Infrastructure.Repositories;
using AnimalShelterAPI.Models.DTO;
using AnimalShelterAPI.Services.Interfaces;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnimalShelterAPI.Services
{
    public class FilterService : IFilterService
    {
        private readonly IFilterRepository _filterRepository;
        private readonly IMapper _mapper;

        public FilterService(IFilterRepository filterRepository, IMapper mapper)
        {
            _filterRepository = filterRepository;
            _mapper = mapper;
        }

        public async Task<ICollection<AnimalListItemDto>> GetAllFiltered(DateTime fromDate, DateTime toDate)
        {
            var animals = await _filterRepository.GetFilteredAll(fromDate, toDate);
            var animalDto = _mapper.Map<AnimalListItemDto[]>(animals);
            return animalDto;
        }
    }
}
