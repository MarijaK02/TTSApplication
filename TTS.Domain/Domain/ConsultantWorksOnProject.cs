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
        public Consultant Consultant { get; set; }

        public Project Project { get; set; }

        public List<Activity>? Activites { get; set; }

        public required DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public required int TotalHoursSpentWorking { get; set; }
    }
}
