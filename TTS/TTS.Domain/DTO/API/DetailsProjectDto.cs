using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTS.Domain.Domain;

namespace TTS.Domain.DTO.API
{
    public class DetailsProjectDto
    {
        public Project Project { get; set; }
        public List<Consultant> Consultants { get; set; }
    }
}
