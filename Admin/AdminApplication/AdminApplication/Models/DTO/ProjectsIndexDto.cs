using AdminApplication.Models.Enums;

namespace AdminApplication.Models.DTO
{
    public class ProjectsIndexDto
    {
        public List<Project> Projects { get; set; }
        public string? SearchTerm { get; set; }
        public Expertise? SelectedExpertise { get; set; }
        public ProjectStatus? SelectedStatus { get; set; }
    }
}
