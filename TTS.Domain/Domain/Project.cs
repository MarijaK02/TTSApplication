using TTS.Domain.Enum;
using TTS.Domain.Shared;

namespace TTS.Domain.Domain
{
    public class Project : BaseEntity
    {
        public required string Title { get; set; }
        public required Expertise Expertise { get; set; }
        public required ProjectStatus Status { get; set; }
        public string? Description { get; set; }

        public required DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public required int TotalHours { get; set; }

        public virtual required Client CreatedBy { get; set; }

        public virtual ICollection<ConsultantWorksOnProject>? Consultants { get; set; }
    }
}
