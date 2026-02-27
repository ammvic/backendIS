using Microsoft.AspNetCore.Identity;
using System;

namespace AnimalShelterAPI.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; } 
        public string LastName { get; set; } 
        public string Role { get; set; } 
        public DateTime EmploymentDate { get; set; } 

    }
}