using System;

namespace AnimalShelterAPI.Models
{
    public class Animal : BaseEntity
    {
        public DateTime AdmissionDate { get; set; }
        public DateTime? Birthday { get; set; }
        public DateTime? MicrochipIntegrationDate { get; set; }
        public DateTime? VaccinationDate { get; set; }
        public string SpecialID { get; set; }
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
        public Status Status { get; set; }
        public DateTime? StatusDate { get; set; }
        public DateTime LastReminder { get; set; }

        public int? AccommodationId { get; set; }
        public Accommodation Accommodation { get; set; }

        public string CageNumber { get; set; } // npr. Kavez A1-3

        public string ImageUrl { get; set; }

    }
}
