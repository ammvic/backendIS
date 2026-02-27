namespace AnimalShelterAPI.Models.DTO
{
    public class AdoptionRequestDto
    {
        public int AnimalId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
