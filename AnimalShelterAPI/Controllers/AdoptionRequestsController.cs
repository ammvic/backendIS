using AnimalShelterAPI.Models.DTO;
using AnimalShelterAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AnimalShelterAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdoptionRequestsController : ControllerBase
    {
        private readonly ApiContext _context;
        private readonly string _sendGridApiKey;

        public AdoptionRequestsController(ApiContext context)
        {
            _context = context;
            // Čita API key iz Render environment variable
            _sendGridApiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
        }

        [HttpPost] public IActionResult CreateAdoptionRequest([FromBody] AdoptionRequestDto dto) {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var animal = _context.Animals.Find(dto.AnimalId);
            if (animal == null) return NotFound("Životinja nije pronađena.");
            var request = new AdoptionRequest { 
                AnimalId = dto.AnimalId, 
                FullName = dto.FullName, 
                Email = dto.Email, 
                Phone = dto.Phone, 
                SentAt = DateTime.Now }; 
            _context.AdoptionRequests.Add(request);
            _context.SaveChanges();
            return Ok(new { message = "Zahtev uspešno poslat." }); }
        [HttpGet] public IActionResult GetAllRequests() {
            var requests = _context.AdoptionRequests .Include(x => x.Animal) 
                .ThenInclude(a => a.Status) 
                .OrderByDescending(x => x.SentAt)
                .Select(x => new { x.Id, x.FullName, x.Email, x.Phone, x.SentAt,
                    Animal = new { x.Animal.Id, x.Animal.AnimalType, Status = x.Animal.Status.Name } }) .ToList(); 
            return Ok(requests); }

        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApproveRequest(int id)
        {
            var request = await _context.AdoptionRequests
                .Include(r => r.Animal)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request == null)
                return NotFound("Zahtev nije pronađen.");

            var adoptedStatus = _context.Statuses.FirstOrDefault(s => s.Name == "Poklonjen");
            if (adoptedStatus == null)
                return BadRequest("Status 'Poklonjen' nije definisan.");

            request.Animal.Status = adoptedStatus;
            _context.AdoptionRequests.Remove(request);
            await _context.SaveChangesAsync();

            // Pošalji mejl
            await SendEmailAsync(
                request.Email,
                "Vaš zahtev za usvajanje je prihvaćen",
                $"Poštovani {request.FullName},\n\nVaš zahtev za usvajanje je prihvaćen.\n\nAnimal Shelter tim"
            );

            return Ok(new { message = "Zahtev prihvaćen." });
        }

        [HttpPost("{id}/reject")]
        public async Task<IActionResult> RejectRequest(int id)
        {
            var request = await _context.AdoptionRequests
                .Include(r => r.Animal)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request == null)
                return NotFound("Zahtev nije pronađen.");

            _context.AdoptionRequests.Remove(request);
            await _context.SaveChangesAsync();

            // Pošalji mejl
            await SendEmailAsync(
                request.Email,
                "Vaš zahtev za usvajanje je odbijen",
                $"Poštovani {request.FullName},\n\nNažalost, vaš zahtev je odbijen.\n\nAnimal Shelter tim"
            );

            return Ok(new { message = "Zahtev odbijen." });
        }

        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            if (string.IsNullOrEmpty(_sendGridApiKey))
            {
                Console.WriteLine("SendGrid API key nije definisan!");
                return;
            }

            try
            {
                var client = new SendGridClient(_sendGridApiKey);
                var from = new EmailAddress("a.mmvic02@gmail.com", "Animal Shelter");
                var to = new EmailAddress(toEmail);
                var msg = MailHelper.CreateSingleEmail(from, to, subject, body, body);

                var response = await client.SendEmailAsync(msg);
                Console.WriteLine($"SendGrid response: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška pri slanju mejla: {ex.Message}");
            }
        }
    }
}