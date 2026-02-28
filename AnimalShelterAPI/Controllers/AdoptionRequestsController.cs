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
     "Vaš zahtev za usvajanje je prihvaćen 🐾",
     request.FullName,
     "Sa velikim zadovoljstvom vas obaveštavamo da je vaš zahtev za usvajanje prihvaćen. Uskoro ćemo vas kontaktirati radi daljih koraka."
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
    request.FullName,
    "Nažalost, vaš zahtev za usvajanje trenutno nije odobren. Zahvaljujemo se na interesovanju i želimo vam sreću pri budućem usvajanju."
);

            return Ok(new { message = "Zahtev odbijen." });
        }

        private async Task SendEmailAsync(string toEmail, string subject, string fullName, string messageText)
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

                var plainTextContent =
        $@"Poštovani {fullName},

{messageText}

Srdačan pozdrav,
Animal Shelter tim";

                var htmlContent =
        $@"
<!DOCTYPE html>
<html>
<body style='margin:0; padding:0; font-family: Arial, sans-serif; background-color:#f4f6f8;'>
    <table align='center' width='100%' cellpadding='0' cellspacing='0' style='padding:40px 0;'>
        <tr>
            <td align='center'>
                <table width='600' cellpadding='0' cellspacing='0' style='background:#ffffff; border-radius:10px; overflow:hidden; box-shadow:0 4px 15px rgba(0,0,0,0.08);'>
                    
                    <tr>
                        <td style='background:#4CAF50; padding:20px; text-align:center; color:white;'>
                            <h2 style='margin:0;'>Animal Shelter 🐾</h2>
                        </td>
                    </tr>

                    <tr>
                        <td style='padding:30px; color:#333333;'>
                            <h3>Poštovani {fullName},</h3>

                            <p style='font-size:16px; line-height:1.6;'>
                                {messageText}
                            </p>

                            <p style='margin-top:30px;'>
                                Srdačan pozdrav,<br/>
                                <strong>Animal Shelter tim</strong>
                            </p>
                        </td>
                    </tr>

                    <tr>
                        <td style='background:#f1f1f1; padding:15px; text-align:center; font-size:12px; color:#777;'>
                            © 2026 Animal Shelter Sistem <br/>
                            Ova poruka je poslata automatski – molimo vas da ne odgovarate.
                        </td>
                    </tr>

                </table>
            </td>
        </tr>
    </table>
</body>
</html>";

                var msg = MailHelper.CreateSingleEmail(
                    from,
                    to,
                    subject,
                    plainTextContent,
                    htmlContent
                );

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