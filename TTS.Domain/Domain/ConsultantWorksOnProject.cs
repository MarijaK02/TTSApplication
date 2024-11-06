using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTS.Domain.Shared;

namespace TTS.Domain.Domain
{
    public class ConsultantWorksOnProject : BaseEntity
    {
        [Required]
        public required Consultant Consultant { get; set; }
        [Required]
        public required Project Project { get; set; }
        [Required]
        public required DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        [Required]
        public required int TotalHoursSpentWorking { get; set; }
    }
}
