using AnimalShelterAPI.Infrastructure.Repositories;
using AnimalShelterAPI.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace AnimalShelterAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeAttr]
    public class ReportsController : ControllerBase
    {
        private readonly IReportRepository _reportRepository;

        public ReportsController(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromBody] ReportRequestDto request)
        {
            var res = await _reportRepository.GetAnimalReport(request.AnimalType, request.Year);
            return Ok(res);
        }
    }
}
