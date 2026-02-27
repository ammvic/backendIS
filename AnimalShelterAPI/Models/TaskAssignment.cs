using System;

namespace AnimalShelterAPI.Models
{
    public class TaskAssignment
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; }    // FK na AspNetUsers.Id (string)
        public string TaskDescription { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime? DueDate { get; set; }
        public bool IsCompleted { get; set; } = false;
    }
}

