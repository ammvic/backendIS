using System;
using System.Linq;
using AnimalShelterAPI.Models;

namespace AnimalShelterAPI.Database
{
    public class DatabaseSeeder
    {
        public static void Initialize(ApiContext context)
        {
            context.Database.EnsureCreated();

            // 1️⃣ Statusi
            if (!context.Statuses.Any())
            {
                context.Statuses.AddRange(
                    new Status { Name = "Poklonjen" },
                    new Status { Name = "Uginuo" },
                    new Status { Name = "Živi u azilu" }
                );
                context.SaveChanges();
            }

            // 2️⃣ Smeštaji
            if (!context.Accommodations.Any())
            {
                context.Accommodations.AddRange(
                    new Accommodation { Name = "Kavez A1", Capacity = 5, CurrentOccupancy = 0, AllowedAnimalType = AnimalType.Pas },
                    new Accommodation { Name = "Prostorija M1", Capacity = 3, CurrentOccupancy = 0, AllowedAnimalType = AnimalType.Mačka },
                    new Accommodation { Name = "Kavez C3", Capacity = 4, CurrentOccupancy = 0, AllowedAnimalType = AnimalType.Pas },
                    new Accommodation { Name = "Prostorija M2", Capacity = 2, CurrentOccupancy = 0, AllowedAnimalType = AnimalType.Mačka },
                    new Accommodation { Name = "Kavez B2", Capacity = 6, CurrentOccupancy = 0, AllowedAnimalType = AnimalType.Pas }
                );
                context.SaveChanges();
            }

            // 3️⃣ Životinje
            if (!context.Animals.Any())
            {
                var poklonjen = context.Statuses.Single(s => s.Name == "Poklonjen");
                var uginuo = context.Statuses.Single(s => s.Name == "Uginuo");
                var zivi = context.Statuses.Single(s => s.Name == "Živi u azilu");

                var kavezA1 = context.Accommodations.Single(a => a.Name == "Kavez A1");
                var prostorijaM1 = context.Accommodations.Single(a => a.Name == "Prostorija M1");
                var kavezC3 = context.Accommodations.Single(a => a.Name == "Kavez C3");
                var prostorijaM2 = context.Accommodations.Single(a => a.Name == "Prostorija M2");
                var kavezB2 = context.Accommodations.Single(a => a.Name == "Kavez B2");

                context.Animals.AddRange(
                    new Animal
                    {
                        AdmissionDate = DateTime.Parse("2020-01-03"),
                        Birthday = DateTime.Parse("2019-01-11"),
                        MicrochipIntegrationDate = DateTime.Parse("2025-09-30"),
                        VaccinationDate = DateTime.Parse("2020-08-04"),
                        SpecialID = "a1s64529s",
                        AdmissionCity = "Beograd",
                        AdmissionRegion = "Novi Beograd",
                        AnimalType = AnimalType.Pas,
                        Gender = Gender.Ženski,
                        FurType = FurType.Kratkodlaki,
                        FurColor = "Bela",
                        Status = zivi,
                        StatusDate = DateTime.Parse("2025-10-09"),
                        SpecialTags = "Simpatičan, veliki",
                        HealthCondition = "Zdrav",
                        AdmissionOrganisationContacts = "Bulevar Zorana Đinđića 40, Beograd",
                        TransferOrganisationContacts = "Ana Anić, +381644445555",
                        Accommodation = kavezA1,
                        CageNumber = "Kavez A1-1",
                        LastReminder = DateTime.Parse("2025-10-09"),
                        ImageUrl = "/images/animals/pas5.jpeg"
                    },
                    new Animal
                    {
                        AdmissionDate = DateTime.Parse("2018-01-03"),
                        Birthday = DateTime.Parse("2017-01-02"),
                        MicrochipIntegrationDate = DateTime.Parse("2020-08-05"),
                        VaccinationDate = DateTime.Parse("2020-08-04"),
                        SpecialID = "a1s6a1ssa",
                        AdmissionCity = "Novi Sad",
                        AdmissionRegion = "Centar",
                        AnimalType = AnimalType.Mačka,
                        Gender = Gender.Muški,
                        FurType = FurType.Dugodlaki,
                        FurColor = "Siva",
                        Status = zivi,
                        StatusDate = DateTime.Parse("2025-10-09"),
                        SpecialTags = "Pametan, energičan",
                        HealthCondition = "Zdrav",
                        AdmissionOrganisationContacts = "Bulevar patrijarha Pavla 9, Novi Sad",
                        TransferOrganisationContacts = "Petar Petrović, +38164222444",
                        Accommodation = prostorijaM1,
                        CageNumber = "Prostorija M1-1",
                        LastReminder = DateTime.Parse("2025-10-09"),
                        ImageUrl = "/images/animals/macka3.jpeg"
                    },
                    new Animal
                    {
                        AdmissionDate = DateTime.Parse("2019-12-03"),
                        Birthday = DateTime.Parse("2015-01-02"),
                        MicrochipIntegrationDate = DateTime.Parse("2020-08-05"),
                        VaccinationDate = DateTime.Parse("2020-08-04"),
                        SpecialID = "a1saassd",
                        AdmissionCity = "Niš",
                        AdmissionRegion = "Centar",
                        AnimalType = AnimalType.Mačka,
                        Gender = Gender.Muški,
                        FurType = FurType.Dugodlaki,
                        FurColor = "Smeđa",
                        Status = poklonjen,
                        StatusDate = DateTime.Parse("2020-08-05"),
                        SpecialTags = "Debeljuškast, sladak",
                        HealthCondition = "Zdrav",
                        AdmissionOrganisationContacts = "Tvenkinio g., Sausinės k., Niš",
                        TransferOrganisationContacts = "Vladimir Petrovic, +38164222444",
                        Accommodation = prostorijaM2,
                        CageNumber = "Prostorija M2-2",
                        LastReminder = DateTime.Parse("2025-10-09"),
                        ImageUrl = "/images/animals/macka2.jpeg"
                    },
                    new Animal
                    {
                        AdmissionDate = DateTime.Parse("2016-02-03"),
                        Birthday = DateTime.Parse("2016-01-02"),
                        MicrochipIntegrationDate = DateTime.Parse("2020-08-05"),
                        VaccinationDate = DateTime.Parse("2020-08-04"),
                        SpecialID = "a1s6asd26",
                        AdmissionCity = "Novi Sad",
                        AdmissionRegion = "Šangaj",
                        AnimalType = AnimalType.Pas,
                        Gender = Gender.Muški,
                        FurType = FurType.SrednjeDugackeDlake,
                        FurColor = "Crna",
                        Status = zivi,
                        StatusDate = DateTime.Parse("2025-10-09"),
                        SpecialTags = "Simpatičan, veliki",
                        HealthCondition = "Zdrav",
                        AdmissionOrganisationContacts = "Tvenkinio g., Sausinės k., Novi Sad",
                        TransferOrganisationContacts = "Vladimir Petrovic, +38164222444",
                        Accommodation = kavezC3,
                        CageNumber = "Kavez C3-2",
                        LastReminder = DateTime.Parse("2025-10-09"),
                        ImageUrl = "/images/animals/pas4.jpeg"
                    },
                    new Animal
                    {
                        AdmissionDate = DateTime.Parse("2020-08-03"),
                        Birthday = DateTime.Parse("2014-01-02"),
                        MicrochipIntegrationDate = DateTime.Parse("2020-08-05"),
                        VaccinationDate = DateTime.Parse("2020-08-04"),
                        SpecialID = "a1s6515a",
                        AdmissionCity = "Beograd",
                        AdmissionRegion = "Novi Beograd",
                        AnimalType = AnimalType.Mačka,
                        Gender = Gender.Ženski,
                        FurType = FurType.Grubodlaki,
                        FurColor = "Siva",
                        Status = poklonjen,
                        StatusDate = DateTime.Parse("2020-08-05"),
                        SpecialTags = "Deka, bučna",
                        HealthCondition = "Zdrava",
                        AdmissionOrganisationContacts = "Antakalnio g. 38, Beograd",
                        TransferOrganisationContacts = "Vladimir Petrovic, +38164222444",
                        Accommodation = prostorijaM1,
                        CageNumber = "Prostorija M1-3",
                        LastReminder = DateTime.Parse("2025-10-09"),
                        ImageUrl = "/images/animals/macka1.jpeg"
                    },
                    new Animal
                    {
                        AdmissionDate = DateTime.Parse("2017-08-03"),
                        Birthday = DateTime.Parse("2013-01-02"),
                        MicrochipIntegrationDate = DateTime.Parse("2020-08-05"),
                        VaccinationDate = DateTime.Parse("2020-08-04"),
                        SpecialID = "a1s6a266as1s",
                        AdmissionCity = "Niš",
                        AdmissionRegion = "Centar",
                        AnimalType = AnimalType.Pas,
                        Gender = Gender.Ženski,
                        FurType = FurType.SrednjeDugackeDlake,
                        FurColor = "Smeđa",
                        Status = zivi,
                        StatusDate = DateTime.Parse("2025-10-09"),
                        SpecialTags = "Uopšte sjajan",
                        HealthCondition = "Zdrav",
                        AdmissionOrganisationContacts = "Tvenkinio g., Sausinės k., Niš",
                        TransferOrganisationContacts = "Vladimir Petrovic, +38164222444",
                        Accommodation = kavezC3,
                        CageNumber = "Kavez C3-3",
                        LastReminder = DateTime.Parse("2025-10-09"),
                        ImageUrl = "/images/animals/pas3.jpeg"
                    },
                    new Animal
                    {
                        AdmissionDate = DateTime.Parse("2016-07-03"),
                        Birthday = DateTime.Parse("2018-01-02"),
                        MicrochipIntegrationDate = DateTime.Parse("2020-08-05"),
                        VaccinationDate = DateTime.Parse("2020-08-04"),
                        SpecialID = "a1165s6a1s",
                        AdmissionCity = "Sarajevo",
                        AdmissionRegion = "Sarajevski kanton",
                        AnimalType = AnimalType.Pas,
                        Gender = Gender.Muški,
                        FurType = FurType.Kratkodlaki,
                        FurColor = "Crna",
                        Status = zivi,
                        StatusDate = DateTime.Parse("2025-10-09"),
                        SpecialTags = "Sladak, mali",
                        HealthCondition = "Zdrav",
                        AdmissionOrganisationContacts = "Tvenkinio g., Sausinės k., Kauno raj.",
                        TransferOrganisationContacts = "Vardenis Pavardenis, +866216815",
                        Accommodation = kavezB2,
                        CageNumber = "Kavez B2-4",
                        LastReminder = DateTime.Parse("2025-10-09"),
                        ImageUrl = "/images/animals/pas2.jpeg"
                    },
                    new Animal
                    {
                        AdmissionDate = DateTime.Parse("2020-03-03"),
                        Birthday = DateTime.Parse("2018-01-02"),
                        MicrochipIntegrationDate = DateTime.Parse("2020-08-05"),
                        VaccinationDate = DateTime.Parse("2020-08-04"),
                        SpecialID = "a1a5ss6a1s",
                        AdmissionCity = "Podgorica",
                        AdmissionRegion = "Glavni grad",
                        AnimalType = AnimalType.Pas,
                        Gender = Gender.Ženski,
                        FurType = FurType.Dugodlaki,
                        FurColor = "Smeđa",
                        Status = poklonjen,
                        StatusDate = DateTime.Parse("2020-08-05"),
                        SpecialTags = "Goofy, veliki",
                        HealthCondition = "Zdrava",
                        AdmissionOrganisationContacts = "Tvenkinio g., Sausinės k., Kauno raj.",
                        TransferOrganisationContacts = "Vardenis Pavardenis, +866216815",
                        Accommodation = null,
                        CageNumber = "",
                        LastReminder = DateTime.Parse("2025-10-09"),
                        ImageUrl = "/images/animals/pas1.jpeg"
                    },
                    new Animal
                    {
                        AdmissionDate = DateTime.Parse("2018-11-03"),
                        Birthday = DateTime.Parse("2014-01-02"),
                        MicrochipIntegrationDate = DateTime.Parse("2020-08-05"),
                        VaccinationDate = DateTime.Parse("2020-08-04"),
                        SpecialID = "a1s6a5s15a1s",
                        AdmissionCity = "Sofija",
                        AdmissionRegion = "Sofijska oblast",
                        AnimalType = AnimalType.Drugo,
                        Gender = Gender.Muški,
                        FurType = FurType.Grubodlaki,
                        FurColor = "Smeđa",
                        Status = zivi,
                        StatusDate = DateTime.Parse("2025-10-09"),
                        SpecialTags = "Sladak, mali",
                        HealthCondition = "Zdrav",
                        AdmissionOrganisationContacts = "Antakalnio g. 38, Vilnius",
                        TransferOrganisationContacts = "Vardenis Pavardenis, +866216815",
                        Accommodation = null,
                        CageNumber = "",
                        LastReminder = DateTime.Parse("2025-10-09"),
                        ImageUrl = "/images/animals/ptica.jpeg"
                    }
                );

                context.SaveChanges();
            }
        }
    }
}
