using System;

namespace AnimalShelterAPI.Models.DTO
{
    public class UpdateEmployeeDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public DateTime EmploymentDate { get; set; }
    }

}
