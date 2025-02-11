using AdminApplication.Models.Enums;

namespace AdminApplication.Models.DTO
{
    public class ActivityListDto
    {
        public Guid ProjectId { get; set; }
        public List<Activity> Activities { get; set; }
        public List<ConsultantProject> ProjectConsultants { get; set; }
        public string? SearchTerm { get; set; }
        public ActivityStatus? SelectedStatus { get; set; }
        public Guid? SelectedConsultantId { get; set; }
    }
}
