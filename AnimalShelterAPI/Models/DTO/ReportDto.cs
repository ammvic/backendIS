using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnimalShelterAPI.Models.DTO
{
    public class ReportDto
    {
        public int AdmittedCount { get; set; }
        public int GiftedCount { get; set; }
        public int DeadCount { get; set; }
        public int LivingNowCount { get; set; }
    }
}
