namespace AdminApplication.Models.DTO
{
    public class CreateActivityFormDto
    {
        public Guid ProjectId { get; set; }
        public Activity Activity { get; set; }
        public List<ConsultantProject> ConsultantProjects { get; set; }
    }
}
