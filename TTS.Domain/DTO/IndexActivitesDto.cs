using TTS.Domain.Domain;

namespace TTS.Domain.DTO
{
    public class IndexActivitesDto
    {
        public Guid ProjectId { get; set; }
        public string? ProjectTitle { get; set; }
        public List<Activity>? Activities { get; set; }
        public string? NewActivityTitle { get; set; }
        public string? NewActivityDescription { get; set; }
    }
}
