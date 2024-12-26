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
        public Guid? ActivityId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public ActivityStatus? ActivityStatus { get; set; }
    }
}
