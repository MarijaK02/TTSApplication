using AdminApplication.Models.Enums;

namespace AdminApplication.Models.DTO.API
{
    public class CreateProjectDto
    {
        public string Title { get; set; }
        public Expertise Expertise { get; set; }

        public ProjectStatus Status { get; set; }

        public string? Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public Guid CreatedById { get; set; }
    }
}
