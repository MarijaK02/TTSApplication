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
        public required bool Disabled { get; set; }


    }
}
