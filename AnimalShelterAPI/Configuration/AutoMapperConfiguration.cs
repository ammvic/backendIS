using AnimalShelterAPI.Models;
using AnimalShelterAPI.Models.DTO;
using AutoMapper;
using System;

namespace AnimalShelterAPI.Configuration
{
    public class AutoMapperConfiguration : Profile
    {
        public AutoMapperConfiguration() : this("AnimalShelterApi") { }

        protected AutoMapperConfiguration(string name) : base(name)
        {
            // -----------------------
            // Animal ↔ AnimalDto
            // -----------------------
            CreateMap<Animal, AnimalDto>();
            CreateMap<AnimalDto, Animal>();

            // -----------------------
            // Animal ↔ AnimalListItemDto
            // -----------------------
            CreateMap<Animal, AnimalListItemDto>()
                .ForMember(dest => dest.StatusID, opt => opt.MapFrom(src => src.Status != null ? src.Status.ID : 0))
                .ForMember(dest => dest.AccommodationId, opt => opt.MapFrom(src => src.AccommodationId))
                .ForMember(dest => dest.CageNumber, opt => opt.MapFrom(src => src.CageNumber ?? string.Empty))
                .ForMember(dest => dest.AccommodationName, opt => opt.MapFrom(src => src.Accommodation != null ? src.Accommodation.Name : string.Empty))
                 // Mapiranje svih ostalih polja
                 .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.SpecialID, opt => opt.MapFrom(src => src.SpecialID))
                .ForMember(dest => dest.AdmissionDate, opt => opt.MapFrom(src => src.AdmissionDate))
                .ForMember(dest => dest.Birthday, opt => opt.MapFrom(src => src.Birthday))
                .ForMember(dest => dest.MicrochipIntegrationDate, opt => opt.MapFrom(src => src.MicrochipIntegrationDate))
                .ForMember(dest => dest.VaccinationDate, opt => opt.MapFrom(src => src.VaccinationDate))
                .ForMember(dest => dest.AdmissionCity, opt => opt.MapFrom(src => src.AdmissionCity))
                .ForMember(dest => dest.AdmissionRegion, opt => opt.MapFrom(src => src.AdmissionRegion))
                .ForMember(dest => dest.AnimalType, opt => opt.MapFrom(src => src.AnimalType))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.FurType, opt => opt.MapFrom(src => src.FurType))
                .ForMember(dest => dest.FurColor, opt => opt.MapFrom(src => src.FurColor))
                .ForMember(dest => dest.SpecialTags, opt => opt.MapFrom(src => src.SpecialTags))
                .ForMember(dest => dest.HealthCondition, opt => opt.MapFrom(src => src.HealthCondition))
                .ForMember(dest => dest.AdmissionOrganisationContacts, opt => opt.MapFrom(src => src.AdmissionOrganisationContacts))
                .ForMember(dest => dest.TransferOrganisationContacts, opt => opt.MapFrom(src => src.TransferOrganisationContacts))
                .ForMember(dest => dest.StatusDate, opt => opt.MapFrom(src => src.StatusDate));


            CreateMap<Animal, NewAnimalDto>()
     .ForMember(dest => dest.Birthday, opt => opt.MapFrom(src => src.Birthday))
     .ForMember(dest => dest.AccommodationId, opt => opt.MapFrom(src => src.AccommodationId))
     .ForMember(dest => dest.CageNumber, opt => opt.MapFrom(src => src.CageNumber ?? string.Empty))
     .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
     .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
     .ForMember(dest => dest.StatusDate, opt => opt.MapFrom(src => src.StatusDate));

            CreateMap<NewAnimalDto, Animal>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.LastReminder, opt => opt.MapFrom(src => DateTime.Now))
                 .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
                .ForMember(dest => dest.TransferOrganisationContacts, opt => opt.Ignore());



            // -----------------------
            // Animal ↔ EditAnimalDto
            // -----------------------
            CreateMap<Animal, EditAnimalDto>()
                .ForMember(dest => dest.Years, opt => opt.MapFrom(src => src.Birthday.HasValue ? (DateTime.Now - src.Birthday.Value).Days / 365 : 0))
                .ForMember(dest => dest.Months, opt => opt.MapFrom(src => src.Birthday.HasValue ? ((DateTime.Now - src.Birthday.Value).Days % 365) / 30 : 0));

            CreateMap<EditAnimalDto, Animal>()
                .ForMember(dest => dest.Birthday, opt => opt.MapFrom(src => DateTime.Now.AddYears(-src.Years).AddMonths(-src.Months)));
        }
    }
}
