using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTS.Domain.Domain;
using TTS.Domain.Enum;

namespace TTS.Domain.DTO
{
    public class ConsultantApplicationsDto
    {
        public ApplicationStatus Status { get; set; }
        public List<ConsultantProject>? Applications { get; set; }

    }
}
