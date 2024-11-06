using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTS.Domain.Enum;
using TTS.Domain.Shared;

namespace TTS.Domain.Domain
{
    public class Project : BaseEntity
    {
        [Required]
        public required string Title { get; set; }
        [Required]
        public required Expertise Expertise { get; set; }
        [Required]
        public required ProjectStatus Status { get; set; }
        public string? Description { get; set; }

        [Required]
        public required DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [Required]
        public required int TotalHours { get; set; }

        public virtual Client? Client { get; set; }
        public virtual List<ConsultantWorksOnProject>? Consultants { get; set; }
    }
}
