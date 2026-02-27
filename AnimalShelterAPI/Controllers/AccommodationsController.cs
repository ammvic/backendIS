using Microsoft.AspNetCore.Mvc;
using AnimalShelterAPI.Models;
using System.Threading.Tasks;
using AnimalShelterAPI.Services.Interfaces;

namespace AnimalShelterAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccommodationController : ControllerBase
    {
        private readonly IAccommodationService _service;

        public AccommodationController(IAccommodationService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.GetAllAsync();
            return Ok(data);
        }

        [HttpGet("available/{type}")]
        public async Task<IActionResult> GetAvailable(AnimalType type)
        {
            var result = await _service.GetAvailableForType(type);
            return Ok(result);
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignAnimal(int animalId, int accommodationId)
        {
            var success = await _service.AssignAnimalToAccommodation(animalId, accommodationId);
            if (!success) return BadRequest("Greška pri smeštanju životinje.");
            return Ok("Životinja uspešno smeštena.");
        }

        [HttpPost("remove/{animalId}")]
        public async Task<IActionResult> RemoveAnimal(int animalId)
        {
            var success = await _service.RemoveAnimalFromAccommodation(animalId);
            if (!success) return BadRequest("Greška pri uklanjanju životinje.");
            return Ok("Životinja uklonjena iz smeštaja.");
        }
    }
}