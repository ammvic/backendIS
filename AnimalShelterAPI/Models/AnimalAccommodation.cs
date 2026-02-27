using System;

namespace AnimalShelterAPI.Models
{
    public class AnimalAccommodation : BaseEntity
    {
        public int AnimalId { get; set; }
        public Animal Animal { get; set; }

        public int AccommodationId { get; set; }
        public Accommodation Accommodation { get; set; }

        public DateTime AssignedDate { get; set; } = DateTime.Now;
        public DateTime? ReleasedDate { get; set; }
    }
}
