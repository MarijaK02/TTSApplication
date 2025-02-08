using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTS.Domain.DTO.API
{
    public class AddConsultantToProjectDto
    {
        public Guid ProjectId { get; set; }
        public Guid ConsultantId { get; set; }
    }
}
