using AnimalShelterAPI.Constants;
using AnimalShelterAPI.Infrastructure.Repositories;
using AnimalShelterAPI.Models;
using AnimalShelterAPI.Models.DTO;
using AnimalShelterAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using System.Linq;


namespace AnimalShelterAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimalsController : ControllerBase
    {
        private readonly IAnimalService _animalService;
        private readonly IReportService _reportService;
        private readonly IFilterService _filterService;
        private readonly ApiContext _context;
        private readonly IWebHostEnvironment _env;

        public AnimalsController(
            IAnimalService animalService,
            IReportService reportService,
            IFilterService filterService,
            ApiContext context,
            IWebHostEnvironment env)
        {
            _animalService = animalService;
            _reportService = reportService;
            _filterService = filterService;
            _context = context;
            _env = env;
        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var animals = await _animalService.GetAll();
                return Ok(animals);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableAnimals()
        {
            var animals = await _context.Animals
         .Include(a => a.Accommodation)
         .Include(a => a.Status)
         .Where(a => a.Status.Name == "Živi u azilu")
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
                    AccommodationName = a.Accommodation != null ? a.Accommodation.Name : "",
                    imageUrl = a.ImageUrl
                })
                .ToListAsync();

            return Ok(animals);
        }


        // GET: api/Animals/filter
        [HttpGet("filter")]
        public async Task<IActionResult> Get(string fromDate, string toDate)
        {
            try
            {
                var animals = await _filterService.GetAllFiltered(DateTime.Parse(fromDate), DateTime.Parse(toDate));
                return Ok(animals);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        // GET: api/Animals/{id}
        [HttpGet("{id}", Name = "GetAnimal")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var animal = await _animalService.GetById(id);
                if (animal == null) return NotFound();
                return Ok(animal);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        // POST: api/Animals
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] NewAnimalDto newAnimal)
        {
            try
            {
                var createdAnimal = await _animalService.Create(newAnimal);
                var animalUri = CreateResourceUri(createdAnimal.Id);
                return Created(animalUri, createdAnimal);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        private Uri CreateResourceUri(int id)
        {
            var url = Url.Link("GetAnimal", new { id });
            return new Uri(url);
        }


        // PUT: api/Animals/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] EditAnimalDto newAnimal)
        {
            try
            {
                await _animalService.Update(id, newAnimal);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        // PATCH: api/Animals/{id}
        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, [FromBody] JsonPatchDocument<NewAnimalDto> patch)
        {
            try
            {
                await _animalService.PartialUpdate(id, patch);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        // DELETE: api/Animals/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _animalService.Delete(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        // GET: api/Animals/Act/{id} - Preuzimanje Zapisnika o prijemu
        [HttpGet("Act/{id}")]
        public async Task<IActionResult> GetAdmissionAct(int id)
        {
            try
            {
                Stream act = await _reportService.GenerateAdmissionAct(id);
                if (act == null) return NotFound();

                string fileName = $"ZapisnikOPrijemu_{id}_{DateTime.Now:yyyyMMddHHmmss}.docx";

                return File(
                    act,
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    fileName
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        // GET: api/Animals/Report?Year=2025&Type=0 - Preuzimanje Godisnjeg izvestaja
        [HttpGet("Report")]
        public async Task<IActionResult> GetAnimalReport(string Year, string Type)
        {
            try
            {
                int animalType = Convert.ToInt32(Type);
                int year = Convert.ToInt32(Year);

                Stream report = await _reportService.GenerateYearReport(animalType, year);
                if (report == null) return NotFound();

                string fileName = $"GodisnjiIzvestaj_{year}_{animalType}_{DateTime.Now:yyyyMMddHHmmss}.docx";

                return File(
                    report,
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    fileName
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, stackTrace = ex.StackTrace });
            }
        }
    }
}
