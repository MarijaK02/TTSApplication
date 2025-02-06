using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTS.Domain.Domain;
using TTS.Domain.Enum;

namespace TTS.Domain.DTO
{
    public class GetProjectDto
    {
        public required Project Project { get; set; }
        public int TotalActivites { get; set; }
        public int NumConsultants { get; set; }
        public int? NumOfConsultantActivites { get; set; }
        public DateTime? EndDate { get; set; }
        public int TotalHours { get; set; }
        public int TotalExpectedHours { get; set; }

    }
}
