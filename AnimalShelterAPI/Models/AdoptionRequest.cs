using System;

namespace AnimalShelterAPI.Models
{
    public class AdoptionRequest : BaseEntity
    {
        public int AnimalId { get; set; }
        public Animal Animal { get; set; }

        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public DateTime SentAt { get; set; } = DateTime.Now;
    }
}
