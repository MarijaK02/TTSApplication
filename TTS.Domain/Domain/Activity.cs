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
    public class Activity : BaseEntity
    {
        public required string Title { get; set; }
        public string? Description { get; set; }

        public required ActivityStatus Status { get; set; }

        public required DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public required virtual ConsultantProject ConsultantProject { get; set; }
        public ICollection<Comment>? Comments { get; set; }
    }
}
