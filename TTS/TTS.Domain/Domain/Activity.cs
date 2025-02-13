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
        public Guid ConsultantProjectId { get; set; }
        public ConsultantProject? ConsultantProject { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public ActivityStatus Status { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? CompletedOn { get; set; }

        public virtual ICollection<Comment>? Comments { get; set; }
    }
}
