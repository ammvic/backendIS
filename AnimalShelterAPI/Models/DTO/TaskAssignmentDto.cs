using System;

namespace AnimalShelterAPI.Models.DTO
{
    public class TaskAssignmentDto
    {
        public string TaskDescription { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
