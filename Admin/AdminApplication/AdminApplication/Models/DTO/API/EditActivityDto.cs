using AdminApplication.Models.Enums;

namespace AdminApplication.Models.DTO.API
{
    public class EditActivityDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public ActivityStatus Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? CompletedOn { get; set; }
    }
}
