using System;

namespace AnimalShelterAPI.Models.DTO
{
    public class TaskViewDto
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public string TaskDescription { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime? DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public string EmployeeFirstName { get; set; }
        public string EmployeeLastName { get; set; }
    }
}
