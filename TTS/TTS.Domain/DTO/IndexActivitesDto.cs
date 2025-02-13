using TTS.Domain.Domain;
using TTS.Domain.Enum;

namespace TTS.Domain.DTO
{
    public class IndexActivitesDto
    {
        public Guid ProjectId { get; set; }
        public string ProjectTitle { get; set; }
        public List<Activity>? Activites { get; set; }
        public List<Consultant> Consultants { get; set; }
        public string? SearchTerm { get; set; }
        public Guid? SelectedConsultantId { get; set; }
        public ActivityStatus? SelectedStatus { get; set; }
    }
}
