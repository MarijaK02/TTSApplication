using AdminApplication.Models.Enums;

namespace AdminApplication.Models
{
    public class ConsultantProject
    {
        public Guid Id { get; set; }
        public Guid ConsultantId { get; set; }
        public Consultant? Consultant { get; set; }
        public Guid ProjectId { get; set; }
        public Project? Project { get; set; }
        public ICollection<Activity>? Activites { get; set; }

        public DateTime DateApplied { get; set; }
        public DateTime? DateModified { get; set; }
        public ApplicationStatus ApplicationStatus { get; set; }
    }
}
