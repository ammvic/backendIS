using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace AnimalShelterAPI.Services.Interfaces
{
    public interface IReportService
    {
        Task<MemoryStream> GenerateAdmissionAct(int id);
        Task<MemoryStream> GenerateYearReport(int AnimalType, int Year);
    }
}
