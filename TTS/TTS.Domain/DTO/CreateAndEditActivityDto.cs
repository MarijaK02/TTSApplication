using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTS.Domain.Enum;

namespace TTS.Domain.DTO
{
    public class CreateAndEditActivityDto
    {
        public required Guid ProjectId { get; set; }
        public required string ProjectTitle { get; set; }
        public Guid ActivityId { get; set; }
        public required string Title { get; set; }
        public required string? Description { get; set; }
        public ActivityStatus ActivityStatus { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
