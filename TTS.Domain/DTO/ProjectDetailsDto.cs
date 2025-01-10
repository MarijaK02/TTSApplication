using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTS.Domain.Domain;

namespace TTS.Domain.DTO
{
    public class ProjectDetailsDto
    {
        public required Project Project { get; set; }
        public List<ConsultantProject>? Applications { get; set; }
        public List<Consultant>? Responsibles{ get; set; }
    }
}
