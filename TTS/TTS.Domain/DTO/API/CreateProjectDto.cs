using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTS.Domain.Enum;

namespace TTS.Domain.DTO.API
{
    public class CreateProjectDto
    {
        public string Title { get; set; }
        public Expertise Expertise { get; set; }

        public ProjectStatus Status { get; set; }

        public string? Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public Guid CreatedById { get; set; }
    }
}
