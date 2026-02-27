using AnimalShelterAPI.Models;
using AnimalShelterAPI.Models.DTO;
using System.Linq;
using System.Threading.Tasks;

namespace AnimalShelterAPI.Infrastructure.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly ApiContext _context;

        public ReportRepository(ApiContext context)
        {
            _context = context;
        }
        public async Task<ReportDto> GetAnimalReport(int AnimalType, int Year)
        {
            var animals = _context.Animals.Where(x => x.AnimalType == (AnimalType)AnimalType);

            return new ReportDto
            {
                AdmittedCount = animals.Where(x => x.AdmissionDate.Year == Year).Count(),
                GiftedCount = animals.Where(x => x.Status.Name == "Poklonjen" && x.StatusDate != null && x.StatusDate.Value.Year == Year).Count(),
                DeadCount = animals.Where(x => x.Status.Name == "Uginuo" && x.StatusDate != null && x.StatusDate.Value.Year == Year).Count(),
                LivingNowCount = animals.Where(x =>
     x.Status.Name == "Živi u azilu" &&
     (
         x.StatusDate.Value.Year == Year ||
         x.AdmissionDate.Year == Year
     )
).Count()

            };
        }
    }
}
