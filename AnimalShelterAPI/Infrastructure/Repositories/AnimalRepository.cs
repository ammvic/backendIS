using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using AnimalShelterAPI.Models;
using System.Collections.Generic;

namespace AnimalShelterAPI.Infrastructure.Repositories
{
    public class AnimalRepository : RepositoryBase<Animal>
    {
        protected override DbSet<Animal> ItemSet { get; }

        public AnimalRepository(ApiContext context) : base(context)
        {
            ItemSet = context.Animals;
        }

        protected override IQueryable<Animal> IncludeDependencies(IQueryable<Animal> queryable)
        {
            return queryable
                .Include(x => x.Status)
                .Include(x => x.Accommodation);
        }

        // Vraća sve životinje u DTO formatu
        public async Task<ICollection<object>> GetAllDto()
        {
            var animals = await IncludeDependencies(ItemSet)
                .Select(a => new
                {
                    a.Id,
                    a.SpecialID,
                    a.AdmissionDate,
                    a.Birthday,
                    a.MicrochipIntegrationDate,
                    a.VaccinationDate,
                    a.AdmissionCity,
                    a.AdmissionRegion,
                    a.AnimalType,
                    a.Gender,
                    a.FurType,
                    a.FurColor,
                    a.SpecialTags,
                    a.HealthCondition,
                    a.AdmissionOrganisationContacts,
                    a.TransferOrganisationContacts,
                    StatusID = a.Status.ID,
                    a.StatusDate,
                    a.AccommodationId,
                    a.CageNumber,
                    a.ImageUrl,
                    AccommodationName = a.Accommodation != null ? a.Accommodation.Name : ""
                })
                .ToListAsync();

            return animals.Cast<object>().ToList();
        }

        // Vraća jednu životinju po ID-u u DTO formatu
        public async Task<object> GetByIdDto(int id)
        {
            var animal = await IncludeDependencies(ItemSet)
                .Where(a => a.Id == id)
                .Select(a => new
                {
                    a.Id,
                    a.SpecialID,
                    a.AdmissionDate,
                    a.Birthday,
                    a.MicrochipIntegrationDate,
                    a.VaccinationDate,
                    a.AdmissionCity,
                    a.AdmissionRegion,
                    a.AnimalType,
                    a.Gender,
                    a.FurType,
                    a.FurColor,
                    a.SpecialTags,
                    a.HealthCondition,
                    a.AdmissionOrganisationContacts,
                    a.TransferOrganisationContacts,
                    StatusID = a.Status.ID,
                    a.StatusDate,
                    a.AccommodationId,
                    a.CageNumber,
                    a.ImageUrl,
                    AccommodationName = a.Accommodation != null ? a.Accommodation.Name : ""
                })
                .FirstOrDefaultAsync();

            return animal;
        }
    }
}
