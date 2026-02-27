using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnimalShelterAPI.Models.DTO
{
    public class ReportRequestDto
    {
        public int AnimalType { get; set; }
        public int Year { get; set; }
    }
}
