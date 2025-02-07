using AdminApplication.Models.Enums;

namespace AdminApplication.Models
{
    public class Activity
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public ActivityStatus Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ICollection<Comment>? Comments { get; set; }

        public Guid ConsultantProjectId { get; set; }
        public ConsultantProject? ConsultantProject { get; set; }
    }
}
