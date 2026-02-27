using System;

namespace AnimalShelterAPI.Models.DTO
{
    public class AnimalListItemDto
    {
        public int Id { get; set; }
        public string SpecialID { get; set; }
        public DateTime AdmissionDate { get; set; }
        public DateTime? Birthday { get; set; }
        public DateTime? MicrochipIntegrationDate { get; set; }
        public DateTime? VaccinationDate { get; set; }
        public string AdmissionCity { get; set; }
        public string AdmissionRegion { get; set; }
        public AnimalType AnimalType { get; set; }
        public Gender Gender { get; set; }
        public FurType FurType { get; set; }
        public string FurColor { get; set; }
        public string SpecialTags { get; set; }
        public string HealthCondition { get; set; }
        public string AdmissionOrganisationContacts { get; set; }
        public string TransferOrganisationContacts { get; set; }
        public int StatusID { get; set; }
        public DateTime? StatusDate { get; set; }
        public int? AccommodationId { get; set; }
        public string CageNumber { get; set; } = string.Empty;
        public string AccommodationName { get; set; } = string.Empty;
        public string ImageUrl { get; set; }
    }

}
