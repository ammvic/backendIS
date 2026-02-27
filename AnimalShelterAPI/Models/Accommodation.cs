using System;
using System.Collections.Generic;

namespace AnimalShelterAPI.Models
{
    public class Accommodation : BaseEntity
    {
        // Naziv i tip smeštaja (npr. Kavez A1, Prostorija za mačke itd.)
        public string Name { get; set; }
        public string Description { get; set; }

        // Maksimalni kapacitet smeštaja
        public int Capacity { get; set; }

        // Broj trenutno smeštenih životinja
        public int CurrentOccupancy { get; set; }

        // Tip životinja koji se ovde može smestiti
        public AnimalType AllowedAnimalType { get; set; }

        // Kolekcija životinja koje su trenutno u ovom smeštaju
        public ICollection<Animal> Animals { get; set; }
    }
}
