using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTS.Domain.Domain;
using TTS.Domain.Shared;

namespace TTS.Domain.DTO
{
    public class ActivityDto
    {
        public Guid ProjectId { get; set; }
        public required string ProjectTitle { get; set; }
        public Interval ProjectDeadline { get; set; }
        public string OwnerEmail { get; set; }
        public Activity Activity { get; set; }
        public List<Comment> Comments { get; set; }
        public int TotalActiveHours { get; set; }
        public int TotalExpectedHours { get; set; }
    }
}
