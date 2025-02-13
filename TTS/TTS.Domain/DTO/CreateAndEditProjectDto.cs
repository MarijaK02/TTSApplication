using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTS.Domain.Enum;

namespace TTS.Domain.DTO
{
    public class CreateAndEditProjectDto
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public required Expertise Expertise { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; } 
        public DateTime EndDate { get; set; } 
    }
}
