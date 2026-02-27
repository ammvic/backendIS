using AnimalShelterAPI.Models.DTO;
using System.Threading.Tasks;

namespace AnimalShelterAPI.Infrastructure.Repositories
{
    public interface IReportRepository
    {
        Task<ReportDto> GetAnimalReport(int AnimalType, int Year);
    }
}
