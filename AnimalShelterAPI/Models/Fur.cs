using System.Collections.Generic;

namespace AnimalShelterAPI.Models
{
    public class Fur
    {
        public int ID { get; set; }
        public FurType FurType { get; set; }
        public string Color { get; set; }
        public ICollection<Animal> Animals { get; set; }
    }
}
