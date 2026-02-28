using AnimalShelterAPI.Models.DTO;
using AnimalShelterAPI.Models;
using AnimalShelterAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SendGrid.Helpers.Mail;
using SendGrid;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace AnimalShelterAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _config;
        private readonly ApiContext _dbContext;
        private readonly IUserService _userService;

        public UsersController(
            IConfiguration config,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IUserService userService,
            ApiContext dbContext)
        {
            _config = config;
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDTO userDTO)
        {
            var user = new User
            {
                FirstName = userDTO.FirstName,
                LastName = userDTO.LastName,
                UserName = userDTO.Email,
                Email = userDTO.Email,
                Role = "Korisnik",
                EmploymentDate = DateTime.UtcNow
            };

            var res = await _userManager.CreateAsync(user, userDTO.Password);
            if (!res.Succeeded)
                return BadRequest(res.Errors.First().Description);

            return Created("", new
            {
                token = GenerateJWTToken(user)
            });
        }

        [HttpPut("update-by-name")]
        public async Task<IActionResult> UpdateByName([FromBody] UpdateEmployeeDto dto)
        {
            // Pronađi korisnika po imenu i prezimenu
            var user = _dbContext.Users.FirstOrDefault(u => u.FirstName == dto.FirstName && u.LastName == dto.LastName);

            if (user == null)
                return NotFound("Zaposleni nije pronađen.");

            // Ažuriraj samo polja koja želimo
            user.Role = dto.Role;
            user.EmploymentDate = dto.EmploymentDate;

            await _dbContext.SaveChangesAsync();

            return Ok(new
            {
                user.FirstName,
                user.LastName,
                user.Role,
                user.EmploymentDate
            });
        }



        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserDTO userDTO)
        {
            var user = await _userManager.FindByEmailAsync(userDTO.Email);
            if (user == null)
                return BadRequest("User does not exist");

            var res = await _signInManager.PasswordSignInAsync(user, userDTO.Password, false, false);
            if (!res.Succeeded)
                return BadRequest("Invalid password");

            return Ok(new
            {
                token = GenerateJWTToken(user)
            });
        }

        private string GenerateJWTToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWTSecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            };

            var token = new JwtSecurityToken(
                issuer: _config["JWTIssuer"],
                audience: _config["JWTAudience"],
                claims: claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        [HttpPost("{id}/assign-task")]
        public async Task<IActionResult> AssignTask(string id, [FromBody] TaskAssignmentDto taskDto)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound("Korisnik nije pronađen.");

            var task = new TaskAssignment
            {
                EmployeeId = id,
                TaskDescription = taskDto.TaskDescription,
                AssignedDate = DateTime.UtcNow,
                DueDate = taskDto.DueDate
            };

            _dbContext.TaskAssignments.Add(task);
            await _dbContext.SaveChangesAsync();

            // 🔥 POŠALJI EMAIL AUTOMATSKI
            await SendTaskEmail(new TaskEmailDto
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Task = taskDto.TaskDescription
            });

            return Ok("Zadatak uspešno dodeljen i email poslat.");
        }
        [HttpGet("all-users")]
        public async Task<IActionResult> Get()
        {
            var users = await _userService.GetNonAdminUsers(); // Koristi metodu za filtrirane korisnike
            return Ok(users);
        }


        private async Task SendTaskEmail(TaskEmailDto dto)
        {
            try
            {
                var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
                var client = new SendGridClient(apiKey);

                var from = new EmailAddress("a.mmvic02@gmail.com", "Animal Shelter");
                var to = new EmailAddress(dto.Email);

                var subject = "Novi zadatak 📝";

                var plainTextContent =
        $@"Zdravo {dto.FirstName} {dto.LastName},

Dodeljen vam je novi zadatak:

{dto.Task}

IS Sistem";

                var htmlContent =
        $@"<div style='font-family:Arial;padding:20px'>
<h2>Novi zadatak 📝</h2>
<p>Zdravo <strong>{dto.FirstName} {dto.LastName}</strong>,</p>
<p>Dodeljen vam je novi zadatak:</p>
<p style='background:#f4f4f4;padding:10px;border-radius:5px'>
<strong>{dto.Task}</strong>
</p>
<p>IS Sistem</p>
</div>";

                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                await client.SendEmailAsync(msg);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Greška pri slanju email-a: " + ex.Message);
            }
        }

        public class TaskEmailDto
        {
            public string Email { get; set; }
            public string Task { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }


    }
}
