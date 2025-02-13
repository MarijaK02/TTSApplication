using AdminApplication.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace AdminApplication.Models
{
    public class Project
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public Expertise Expertise { get; set; }

        public ProjectStatus Status { get; set; }

        public string? Description { get; set; }
        public DateTime CreatedOn { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
        public DateTime? CompletedOn { get; set; }

        public Guid CreatedById { get; set; }
        public Client? CreatedBy { get; set; }
        public List<ConsultantProject>? ConsultantProjects { get; set; }
    }
}
