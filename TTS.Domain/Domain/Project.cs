using System.ComponentModel.DataAnnotations;
using TTS.Domain.Enum;
using TTS.Domain.Shared;

namespace TTS.Domain.Domain
{
    public class Project : BaseEntity
    {
        [Display(Name = "Наслов")]
        public required string Title { get; set; }

        [Display(Name = "Експертиза")]
        public required Expertise Expertise { get; set; }

        [Display(Name = "Статус")]
        public required ProjectStatus Status { get; set; }

        [Display(Name = "Опис")]
        public string? Description { get; set; }

        [Display(Name = "Датум на креирање")]
        public required DateTime StartDate { get; set; }

        [Display(Name = "Краен рок")]
        public DateTime? EndDate { get; set; }

        public virtual required Client CreatedBy { get; set; }

        public virtual ICollection<ConsultantProject>? ConsultantProjects { get; set; }
    }
}
