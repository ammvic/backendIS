using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AnimalShelterAPI.Models
{
    public class UserDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        public string Role { get; set; }
        public DateTime EmploymentDate { get; set; }
    }
}
